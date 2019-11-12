using System;
using System.Net.Http;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;
using reporting_tool.Infra;

namespace reporting_tool
{
    /// <summary>
    /// Base class for all Okta reports and actions
    /// Encapsulates Okta client with required settings
    /// </summary>
    public abstract class OktaAction
    {
        private readonly OktaConfig _oktaConfig;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oktaConfig">Config params to use while instantiating Okta client</param>
        protected OktaAction(OktaConfig oktaConfig)
        {
            _oktaConfig = oktaConfig;

            if (string.IsNullOrEmpty(_oktaConfig.ApiKey))
                _oktaConfig.ApiKey = PromptAndReadToken();
        }

        /// <summary>
        /// Property to get Okta client
        /// </summary>
        protected OktaClient OktaClient => new OktaClient(
            new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey,
                RateLimitPreservationPercent = _oktaConfig.RateLimiPreservationPct ?? 10,
            },
            new HttpClient(),
            null,
            new ExceptionRetryStrategy(OktaClientConfiguration.DefaultMaxRetries,
                OktaClientConfiguration.DefaultRequestTimeout)
        );

        /// <summary>
        /// Entry point for execution of action
        /// </summary>
        public abstract Task Run();

        private static string PromptAndReadToken()
        {
            var token = "";
            Console.WriteLine("Enter token: ");

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.Write('\n');
                    break;
                }

                Console.Write('*');
                token += key.KeyChar;
            }

            return token;
        }
    }
}