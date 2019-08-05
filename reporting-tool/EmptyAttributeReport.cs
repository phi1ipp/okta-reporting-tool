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
        private readonly DateTime _since;

        public EmptyAttributeReport(OktaConfig config, string attrName, string since = null)
        {
            _oktaConfig = config;
            _attrName = attrName;
            _since = since == null ? DateTime.Parse("1990-01-01") : DateTime.Parse(since);
        }

        public void Run()
        {
            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey
            });

            oktaClient.Users
                .ListUsers(search: $"created gt \"{_since:yyyy-MM-ddT00:00:00.000Z}\"")
                .Where(u => string.IsNullOrEmpty(u.Profile[_attrName]?.ToString()) )
                .Select(u =>
                    u.Id + " " +
                    string.Join(",",
                        oktaClient.Users
                            .ListUserGroups(u.Id)
                            .Select(gr => gr.Profile.Name)
                            .ToList().Result))
                .ForEachAsync(line => Console.WriteLine(line))
                .Wait();
        }
    }
}