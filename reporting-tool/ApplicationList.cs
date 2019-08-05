using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    public class ApplicationList
    {
        private OktaConfig _oktaConfig;

        public ApplicationList(OktaConfig config)
        {
            _oktaConfig = config;
        }

        public void Run()
        {
            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey
            });

            oktaClient.Applications.ListApplications().ForEach(app =>
            {
                Console.WriteLine($"uuid: {app.Id} name: {app.Name} label: {app.Label}");
            });
        }
    }
}