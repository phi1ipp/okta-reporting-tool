using System;
using System.Linq;

namespace reporting_tool
{
    /// <inheritdoc />
    /// <summary>
    /// Class to run a report to discover users with an empty attribute
    /// </summary>
    public class EmptyAttributeReport : OktaAction
    {
        private readonly string _attrName;
        private readonly DateTime _since;

        /// <inheritdoc />
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Configuration object</param>
        /// <param name="attrName">Attribute name to check against nullability</param>
        /// <param name="since">Optional string representing a date of user creation since which to start inspection</param>
        public EmptyAttributeReport(OktaConfig config, string attrName, string since = null) : base(config)
        {
            _attrName = attrName;
            _since = since == null ? DateTime.Parse("1990-01-01") : DateTime.Parse(since);
        }

        /// <summary>
        /// Report execution entry point
        /// </summary>
        public override void Run()
        {
            OktaClient.Users
                .ListUsers(search: $"created gt \"{_since:yyyy-MM-ddT00:00:00.000Z}\"")
                .Where(u => string.IsNullOrEmpty(u.Profile[_attrName]?.ToString()))
                .Select(async u =>
                    {

                        var lstGroups = await OktaClient.Users
                            .ListUserGroups(u.Id)
                            .Select(gr => gr.Profile.Name)
                            .ToList();
                        
                        return $"{u.Id} {string.Join(',', lstGroups)}";
                    })
                .ForEachAsync(async str => Console.WriteLine(await str))
                .Wait();
        }
    }
}