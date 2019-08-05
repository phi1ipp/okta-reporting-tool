using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    public class GroupMembersReport
    {
        private readonly OktaConfig _oktaConfig;
        private readonly string _grpName;

        public GroupMembersReport(OktaConfig config, string grpName)
        {
            _oktaConfig = config;
            _grpName = grpName;
        }

        public void Run()
        {
            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey
            });

            var grpId = oktaClient.Groups.ListGroups(q: _grpName).Select(grp => grp.Id).First().Result;
            
            var cnt = oktaClient.Groups
                .ListGroupUsers(grpId)
                .Count(u => u.Profile["LOA"]?.ToString() == "3")
                .Result;
            
            Console.WriteLine($"List of users in group {_grpName} with LOA == 3 - {cnt}");
        }
    }
}