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
    public class GroupList
    {
        private OktaConfig _oktaConfig;

        public GroupList(OktaConfig config)
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

            oktaClient.Groups.ListGroups().ForEach(grp =>
            {
                Console.WriteLine($"uuid: {grp.Id} name: {grp.Profile.Name}");
            });
        }
    }
}