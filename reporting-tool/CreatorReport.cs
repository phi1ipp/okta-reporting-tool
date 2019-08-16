using System;
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Okta Configuration class</param>
        /// <param name="fileInfo">File path to use as a source of user entries to run the report for</param>
        public CreatorReport(OktaConfig config, FileInfo fileInfo) : base(config)
        {
            _fileInfo = fileInfo;
        }

        /// <summary>
        /// Main method to execute report
        /// </summary>
        public void Run()
        {
            File.ReadLines(_fileInfo.FullName)
                .Select(line => line.Trim().Split(' ', ',').First())
                .AsParallel()
                .ForAll(userId =>
                    Console.WriteLine(userId + " " + OktaClient.Logs
                        .GetLogs(
                            filter: $"eventType eq \"user.lifecycle.create\" and target.id eq \"{userId}\"",
                            since: DateTime.Now.Add(TimeSpan.FromDays(-180d)).ToString("yyyy-MM-dd"))
                        .Select(ev => ev.Actor.AlternateId + " " + ev.Actor.Id)
                        .FirstOrDefault().Result)
                );

        }
    }
}