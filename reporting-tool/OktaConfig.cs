using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace reporting_tool
{
    public class OktaConfig
    {
        public OktaConfig()
        {
        }

        public static OktaConfig BuildConfig()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = configBuilder.Build();

            return config.GetSection("Okta").Get<OktaConfig>();
        }

        public string ApiKey { get; set; }
        public string Domain { get; set; }
    }
}
