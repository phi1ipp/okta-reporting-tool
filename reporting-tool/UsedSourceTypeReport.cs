using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    public class UsedSourceTypeReport
    {
        private OktaConfig _oktaConfig;

        public UsedSourceTypeReport(OktaConfig config)
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

            oktaClient.Users
                .ListUsers(search: "profile.SourceType pr")
                .Select(u => u.Profile["SourceType"].ToString())
                .Distinct()
                .ForEach(uid => Console.WriteLine(uid));
        }
    }
}