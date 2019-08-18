using System.Net.Http;
using Okta.Sdk;
using Okta.Sdk.Configuration;

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
        }

        /// <summary>
        /// Property to get Okta client
        /// </summary>
        protected OktaClient OktaClient => new OktaClient(
            new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey,
                RateLimitPreservationPercent = 10
            },
            new HttpClient(),
            null,
            new DefaultRetryStrategy(OktaClientConfiguration.DefaultMaxRetries,
                OktaClientConfiguration.DefaultRequestTimeout)
        );

        /// <summary>
        /// Entry point for execution of action
        /// </summary>
        public abstract void Run();
    }
}