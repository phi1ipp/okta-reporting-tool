using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
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

            var channel = Channel.CreateUnbounded<IUser>();

            var readers =
                Enumerable.Range(1, 8)
                    .Select(async j => { await StartReader(channel); });
            
            Console.WriteLine(UserExtensions.PrintUserAttributesHeader(_attrs, _ofs));

            OktaClient.Groups
                .ListGroupUsers(grpId)
                .Where(user => _filter(user))
                .ForEachAsync(user =>
                    channel.Writer.TryWrite(user))
                .Wait();
            
            channel.Writer.Complete();

            Task.WhenAll(readers).Wait();
        }
        
        private async Task StartReader(Channel<IUser> channel)
        {
            var reader = channel.Reader;

            while (await reader.WaitToReadAsync())
            {
                var user = await reader.ReadAsync();

                Console.WriteLine(await user.PrintAttributesAsync(_attrs, OktaClient, _ofs));
            }
        }
    }
}