using System;
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
        private readonly string _search;
        private readonly ICollection<string> _attrs;
        private readonly Func<IUser, bool> _filter;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="search">User search expression in Okta API language</param>
        /// <param name="filter">User filter</param>
        /// <param name="attrs">List of attributes to output for each user (CSV)</param>
        public UserSearchReport(OktaConfig config, string search, string filter, string attrs) : base(config)
        {
            _search = search;

            _attrs = attrs?.Split(",").ToHashSet();

            _filter = new UserFilter(filter).F;
        }

        /// <summary>
        /// Report main entry
        /// </summary>
        public override void Run()
        {
            Console.WriteLine("id " +
                              string.Join(" ",
                                  _attrs.Where(attr => UserAttributes.NonProfileAttribute.Contains(attr))) +
                              " " +
                              string.Join(" ",
                                  _attrs.Where(attr => !UserAttributes.NonProfileAttribute.Contains(attr))));

            var userBase = string.IsNullOrEmpty(_search)
                ? OktaClient.Users.Where(user => _filter(user))
                : OktaClient.Users.ListUsers(search: _search).Where(user => _filter(user));
            
            userBase
                .ForEachAsync(user =>
                {
                    Console.WriteLine($"{user.Id} " +
                                      string.Join(" ",
                                          _attrs
                                              .Where(attr => UserAttributes.NonProfileAttribute.Contains(attr))
                                              .Select(user.GetNonProfileAttribute)) + " " +
                                      string.Join(" ",
                                          _attrs
                                              .Where(attr => !UserAttributes.NonProfileAttribute.Contains(attr))
                                              .Select(attr => user.Profile[attr]?.ToString())));
                })
                .Wait();
        }
    }
}