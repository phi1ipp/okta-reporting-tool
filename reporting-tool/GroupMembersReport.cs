using System;
using System.Linq;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    /// <summary>
    /// Class to run a report on Okta group membership information
    /// </summary>
    public class GroupMembersReport : OktaAction
    {
        private readonly OktaConfig _oktaConfig;
        private readonly string _grpName;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="grpName">Group name</param>
        public GroupMembersReport(OktaConfig config, string grpName) : base(config)
        {
            _oktaConfig = config;
            _grpName = grpName;
        }

        /// <summary>
        /// Report's main entry
        /// </summary>
        public override void Run()
        {
            var grpId = OktaClient.Groups.ListGroups(q: _grpName).Select(grp => grp.Id).First().Result;
            
            var cnt = OktaClient.Groups
                .ListGroupUsers(grpId)
                .Count(u => u.Profile["LOA"]?.ToString() == "3")
                .Result;
            
            Console.WriteLine($"List of users in group {_grpName} with LOA == 3 - {cnt}");
        }
    }
}