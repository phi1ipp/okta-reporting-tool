using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    public class EmptyAttributeReport
    {
        private OktaConfig _oktaConfig;
        private string _attrName;

        public EmptyAttributeReport(OktaConfig config, string attrName)
        {
            _oktaConfig = config;
            _attrName = attrName;
        }

        public void Run()
        {
            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey
            });

            oktaClient.Users
                .ListUsers()
                .Where(u => u.Profile[_attrName] == null)
                .Select(u =>
                    u.Id + " " +
                    string.Join(",",
                        oktaClient.Users
                            .ListUserGroups(u.Id)
                            .Select(gr => gr.Profile.Name)
                            .ToList().Result))
                .ForEachAsync(line => Console.WriteLine(line));
        }
    }
}