using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Okta.Sdk;

namespace reporting_tool
{
    /// <inheritdoc />
    /// <summary>
    /// Class to run User report based on search
    /// </summary>
    public class UserSearchReport : OktaAction
    {
        private readonly string _ofs;
        private readonly string _search;
        private readonly IEnumerable<string> _attrs;
        private readonly Func<IUser, bool> _filter;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="search">User search expression in Okta API language</param>
        /// <param name="filter">User filter</param>
        /// <param name="attrs">List of attributes to output for each user (CSV)</param>
        /// <param name="ofs">Output field separator</param>
        public UserSearchReport(OktaConfig config, string search, string filter, string attrs, string ofs = " ") :
            base(config)
        {
            _search = search;
            _ofs = ofs;

            _attrs = string.IsNullOrEmpty(attrs)
                ? Enumerable.Empty<string>()
                : attrs.Split(",").ToHashSet();

            _filter = new UserFilter(filter).F;
        }

        /// <inheritdoc />
        /// <summary>
        /// Report main entry
        /// </summary>
        public override void Run()
        {
            Console.WriteLine("id" +
                              string.Join(_ofs,
                                  _attrs.Where(attr => UserAttributes.NonProfileAttribute.Contains(attr))) +
                              _ofs +
                              string.Join(_ofs,
                                  _attrs.Where(attr => !UserAttributes.NonProfileAttribute.Contains(attr))));

            var userBase = string.IsNullOrEmpty(_search)
                ? OktaClient.Users.Where(user => _filter(user))
                : OktaClient.Users.ListUsers(search: _search).Where(user => _filter(user));

            userBase
                .ForEachAsync(user =>
                {
                    var values = new[]
                    {
                        new[] {$"{user.Id}"},
                        _attrs
                            .Where(attr => UserAttributes.NonProfileAttribute.Contains(attr))
                            .Select(user.GetNonProfileAttribute),
                        _attrs
                            .Where(attr => !UserAttributes.NonProfileAttribute.Contains(attr))
                            .Select(attr => user.Profile[attr]?.ToString())
                    }
                        .Where(lst => lst.Any())
                        .SelectMany(x => x)
                        .Select(attr => !string.IsNullOrEmpty(attr) && attr.Contains(_ofs) ? $"\"{attr}\"" : attr);
                    
                    Console.WriteLine(string.Join(_ofs, values));
                })
                .Wait();
        }
    }
}