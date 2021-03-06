using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk;

namespace reporting_tool.Infra
{
    /// <summary>
    /// Retry strategy based on Okta Default one with exceptions retry
    /// </summary>
    public class ExceptionRetryStrategy : IRetryStrategy
    {
        /// <summary>
        /// The default delta used in the back-off formula to account for some clock skew in our service
        /// </summary>
        private const int DefaultBackoffSecondsDelta = 1;

        private readonly int _maxRetries;
        private readonly int _requestTimeout;
        private readonly int _backoffSecondsDelta;

        // Now we are only managing 429 errors, but we can accept other codes in the future
        private readonly IList<HttpStatusCode> _retryableStatusCodes = new List<HttpStatusCode> {(HttpStatusCode) 429};

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRetryStrategy"/> class.
        /// </summary>
        /// <param name="maxRetries">the number of times to retry</param>
        /// <param name="requestTimeout">The request timeout in seconds</param>
        /// <param name="backoffSecondsDelta">The delta of seconds included the back-off calculation</param>
        public ExceptionRetryStrategy(int maxRetries, int requestTimeout,
            int backoffSecondsDelta = DefaultBackoffSecondsDelta)
        {
            if (requestTimeout > 0 && backoffSecondsDelta > requestTimeout)
            {
                throw new ArgumentException("The backoff delta cannot be greater than the request timeout");
            }

            _maxRetries = maxRetries;
            _requestTimeout = requestTimeout;
            _backoffSecondsDelta = backoffSecondsDelta;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> WaitAndRetryAsync(HttpRequestMessage request,
            CancellationToken cancellationToken,
            Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> operation)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            var numberOfRetries = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            do
            {
                var response = new HttpResponseMessage();
                var exceptionHappened = false;
                
                try
                {
                    response = await operation(request, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    if (e is HttpRequestException && e.InnerException is SocketException)
                        exceptionHappened = true;
                    else
                        throw;
                }

                if (exceptionHappened || IsRetryable(response) && numberOfRetries < _maxRetries &&
                    (_requestTimeout <= 0 || stopwatch.Elapsed.Seconds < _requestTimeout))
                {
                    numberOfRetries++;
                    var delayTimeSpan = CalculateDelay(response);
                    if (delayTimeSpan > TimeSpan.Zero)
                    {
                        await Task.Delay(delayTimeSpan, cancellationToken).ConfigureAwait(false);
                        response.Headers.TryGetValues("X-Okta-Request-Id", out var requestId);
                        request = AddRetryOktaHeaders(request, requestId.FirstOrDefault(), numberOfRetries);
                    }
                    else
                    {
                        return response;
                    }
                }
                else
                {
                    return response;
                }
            } while (true);
        }

        /// <summary>
        /// Checks if a http response message should be retried
        /// </summary>
        /// <param name="response">The http response message</param>
        /// <returns>True if the value is must be retried, otherwise false.</returns>
        private bool IsRetryable(HttpResponseMessage response)
            => response != null && _retryableStatusCodes.Contains(response.StatusCode);

        private HttpRequestMessage AddRetryOktaHeaders(HttpRequestMessage request, string requestId, int numberOfRetries)
        {
            var newRequest = CloneHttpRequestMessageAsync(request).Result;

            if (!newRequest.Headers.Contains("X-Okta-Retry-For"))
            {
                newRequest.Headers.Add("X-Okta-Retry-For", requestId);
            }

            if (newRequest.Headers.Contains("X-Okta-Retry-Count"))
            {
                newRequest.Headers.Remove("X-Okta-Retry-Count");
            }

            newRequest.Headers.Add("X-Okta-Retry-Count", numberOfRetries.ToString());

            return newRequest;
        }

        private TimeSpan CalculateDelay(HttpResponseMessage response)
        {
            DateTime? requestTime = null;
            DateTime? retryDate = null;
            var backoffSeconds = TimeSpan.Zero;

            if (response.Headers.TryGetValues("Date", out var dates) && dates != null)
            {
                requestTime = DateTimeOffset.Parse(dates.First()).UtcDateTime;
            }

            if (response.Headers.TryGetValues("x-rate-limit-reset", out var rateLimits) && rateLimits != null)
            {
                // If there are multiple headers, choose the smallest one
                retryDate = DateTimeOffset.FromUnixTimeSeconds(rateLimits.Min(long.Parse)).UtcDateTime;
            }

            if (requestTime.HasValue && retryDate.HasValue)
            {
                var backoffSecondsAux = retryDate.Value.Subtract(requestTime.Value)
                    .Add(new TimeSpan(0, 0, _backoffSecondsDelta));
                // _requestTimeout <= 0 means no timeout
                if (_requestTimeout <= 0 || (_requestTimeout > 0 && backoffSecondsAux.Seconds <= _requestTimeout))
                {
                    backoffSeconds = backoffSecondsAux;
                }
            }

            return backoffSeconds;
        }

        /// <summary>
        /// Method to do deep copying for the http request object
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            var clone = new HttpRequestMessage(req.Method, req.RequestUri);

            // Copy the request's content (via a MemoryStream) into the cloned object
            var ms = new MemoryStream();
            if (req.Content != null)
            {
                await req.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                // Copy the content headers
                if (req.Content.Headers != null)
                {
                    foreach (var (key, value) in req.Content.Headers)
                    {
                        clone.Content.Headers.Add(key, value);
                    }
                }
            }


            clone.Version = req.Version;

            foreach (KeyValuePair<string, object> prop in req.Properties)
            {
                clone.Properties.Add(prop);
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }
    }
}
