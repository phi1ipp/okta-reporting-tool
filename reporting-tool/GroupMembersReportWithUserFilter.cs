using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class to run a report for a specific Okta group and apply an additional filter to users from the group
    /// </summary>
    public class GroupMembersReportWithUserFilter : OktaAction
    {
        private readonly string _grpName;
        private readonly IEnumerable<string> _attrs;
        private readonly Func<IUser, bool> _filter;
        private readonly string _ofs;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="grpName">Group name</param>
        /// <param name="userFilter">not implemented yet</param>
        /// <param name="userAttrList">List of attributes to output for each user</param>
        /// <param name="ofs">Output field separator</param>
        public GroupMembersReportWithUserFilter(OktaConfig config, string grpName, string userFilter,
            string userAttrList, string ofs = ",") : base(config)
        {
            _grpName = grpName;
            _ofs = ofs;

            _attrs = string.IsNullOrEmpty(userAttrList)
                ? Enumerable.Empty<string>()
                : userAttrList.Trim().Split(",");

            _filter = new UserFilter(userFilter).F;
        }

        /// <inheritdoc />
        /// <summary>
        /// Report's main entry
        /// </summary>
        /// <returns></returns>
        public override async Task Run()
        {
            var grpId = await OktaClient.Groups
                .ListGroups(q: _grpName)
                .Select(grp => grp.Id)
                .FirstOrDefault();

            if (grpId == null)
            {
                Console.WriteLine($"Group \"{_grpName}\" doesn't exist");
                return;
            }

            Console.WriteLine(UserExtensions.PrintUserAttributesHeader(_attrs, _ofs));

            var tasks = OktaClient.Groups
                .ListGroupUsers(grpId)
                .Where(user => _filter(user))
                .Select(async user =>
                    Console.WriteLine(await user.PrintAttributesAsync(_attrs, OktaClient, _ofs)))
                .ToEnumerable();

            await Task.WhenAll(tasks);
        }
    }
}