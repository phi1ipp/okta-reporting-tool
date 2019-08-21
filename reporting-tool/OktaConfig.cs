using System.IO;
using Microsoft.Extensions.Configuration;

namespace reporting_tool
{
    /// <summary>
    /// Configuration class for Okta client
    /// </summary>
    public class OktaConfig
    {
        /// <summary>
        /// Return back OktaConfig class
        /// </summary>
        /// <returns></returns>
        public static OktaConfig BuildConfig()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = configBuilder.Build();

            return config.GetSection("Okta").Get<OktaConfig>();
        }

        /// <summary>
        /// Okta Api Token
        /// </summary>
        public string ApiKey { get; set; }
        
        /// <summary>
        /// Okta domain
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Percents of rate limit to preserve while running requests
        /// </summary>
        public int? RateLimiPreservationPct { get; set; }
    }
}
