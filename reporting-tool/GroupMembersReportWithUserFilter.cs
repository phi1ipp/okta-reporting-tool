using System;
using System.Collections.Generic;
using System.Linq;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class to run a report for a specific Okta group and apply an additional filter to users from the group
    /// </summary>
    public class GroupMembersReportWithUserFilter : OktaAction
    {
        private readonly string _grpName;
        private readonly IEnumerable<string> _usrAttrs;
        private readonly Func<IUser, bool> _filter;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="grpName">Group name</param>
        /// <param name="userFilter">not implemented yet</param>
        /// <param name="userAttrList">List of attributes to output for each user</param>
        public GroupMembersReportWithUserFilter(OktaConfig config, string grpName, string userFilter,
            string userAttrList) : base(config)
        {
            _grpName = grpName;

            _usrAttrs = string.IsNullOrEmpty(userAttrList)
            ? Enumerable.Empty<string>()
            : userAttrList.Trim().Split(",");

            _filter = new UserFilter(userFilter).F;
        }

        /// <inheritdoc />
        /// <summary>
        /// Report's main entry
        /// </summary>
        /// <returns></returns>
        public override void Run()
        {
            var grpId = OktaClient.Groups
                .ListGroups(q: _grpName)
                .Select(grp => grp.Id)
                .First().Result;

            if (grpId == null)
            {
                Console.WriteLine($"Group {_grpName} doesn't exist");
                return;
            }

            Console.WriteLine("id " +
                              string.Join(" ",
                                  _usrAttrs.Where(attr => UserAttributes.NonProfileAttribute.Contains(attr))) + " " +
                              string.Join(" ",
                                  _usrAttrs.Where(attr => !UserAttributes.NonProfileAttribute.Contains(attr)))
            );

            OktaClient.Groups
                .ListGroupUsers(grpId)
                .Where(user => _filter(user))
                .ForEachAsync(user =>
                    Console.WriteLine($"{user.Id} " +
                                      string.Join(" ",
                                          _usrAttrs
                                              .Where(attr => UserAttributes.NonProfileAttribute.Contains(attr))
                                              .Select(user.GetNonProfileAttribute)) + " " +
                                      string.Join(" ",
                                          _usrAttrs
                                              .Where(attr => !UserAttributes.NonProfileAttribute.Contains(attr))
                                              .Select(attr => user.Profile[attr]?.ToString()))))
                .Wait();
        }
    }
}