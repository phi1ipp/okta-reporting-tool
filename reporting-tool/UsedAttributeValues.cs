using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    public class UsedAttributeValues
    {
        private OktaConfig _oktaConfig;
        private string _attrName { get; }

        public UsedAttributeValues(OktaConfig config, string attrName)
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

            try
            {
                oktaClient.Users
                    .ListUsers(search: $"profile.{_attrName} pr")
                    .Select(u => u.Profile[_attrName].ToString())
                    .Distinct()
                    .ForEachAsync(val => Console.WriteLine(string.IsNullOrEmpty(val) ? "<Empty string>" : val))
                    .Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}