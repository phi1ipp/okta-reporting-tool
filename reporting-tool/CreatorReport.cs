using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace reporting_tool
{
    /// <inheritdoc />
    /// <summary>
    /// Class to execute report to find creators of user entries
    /// </summary>
    public class CreatorReport : OktaAction
    {
        private readonly FileInfo _fileInfo;
        private readonly IEnumerable<string> _attrs;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Okta Configuration class</param>
        /// <param name="fileInfo">File path to use as a source of user entries to run the report for</param>
        public CreatorReport(OktaConfig config, FileInfo fileInfo, string attrs) : base(config)
        {
            _fileInfo = fileInfo;

            _attrs = string.IsNullOrEmpty(attrs)
                ? Enumerable.Empty<string>()
                : attrs.Trim().Split(',').Select(attr => attr.Trim()).ToHashSet();
        }

        /// <summary>
        /// Main method to execute report
        /// </summary>
        public override void Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            Console.WriteLine("userid id " +
                              string.Join(" ",
                                  _attrs.Where(attr => UserAttributes.NonProfileAttribute.Contains(attr))) + " " +
                              string.Join(" ",
                                  _attrs.Where(attr => !UserAttributes.NonProfileAttribute.Contains(attr)))
            );

            lines
                .Select(line => line.Trim().Split(' ', ',').First())
                .AsParallel()
                .ForAll(userId =>
                    Console.WriteLine(
                        userId + " " +
                        OktaClient.Logs
                            .GetLogs(
                                filter:
                                $"eventType eq \"user.lifecycle.create\" and target.id eq \"{userId}\"",
                                since: DateTime.Now.Add(TimeSpan.FromDays(-180d)).ToString("yyyy-MM-dd"))
                            .Select(ev =>
                            {
                                var user = OktaClient.Users.GetUserAsync(ev.Actor.Id).Result;

                                return ev.Actor.Id + " " +
                                       string.Join(" ",
                                           _attrs
                                               .Where(attr =>
                                                   UserAttributes.NonProfileAttribute.Contains(attr))
                                               .Select(user.GetNonProfileAttribute)) + " " +
                                       string.Join(" ",
                                           _attrs
                                               .Where(attr =>
                                                   !UserAttributes.NonProfileAttribute.Contains(attr))
                                               .Select(attr => user.Profile[attr]?.ToString()));
                            })
                            .FirstOrDefault()
                            .Result)
                );
        }
    }
}