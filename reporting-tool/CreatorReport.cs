using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly string _ofs;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">Okta Configuration class</param>
        /// <param name="fileInfo">File path to use as a source of user entries to run the report for</param>
        /// <param name="attrs">Attributes to output for a found creator</param>
        /// <param name="ofs">Output field separator</param>
        public CreatorReport(OktaConfig config, FileInfo fileInfo, string attrs, string ofs = " ") : base(config)
        {
            _fileInfo = fileInfo;

            _attrs = string.IsNullOrEmpty(attrs)
                ? Enumerable.Empty<string>()
                : attrs.Trim().Split(',').Select(attr => attr.Trim()).ToHashSet();

            _ofs = ofs;
        }

        /// <summary>
        /// Main method to execute report
        /// </summary>
        public override async Task Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            var semaphore = new SemaphoreSlim(8);
            
            Console.WriteLine("userid" + _ofs + UserExtensions.PrintUserAttributesHeader(_attrs, _ofs));

            var tasks = lines
                .Select(
                    async line =>
                    {
                        await semaphore.WaitAsync();

                        var userId = "";
                        try
                        {
                            userId = line.Trim().Split(' ', ',').First();

                            Console.WriteLine(
                                userId + _ofs +
                                await OktaClient.Logs
                                    .GetLogs(
                                        filter:
                                        $"eventType eq \"user.lifecycle.create\" and target.id eq \"{userId}\"",
                                        since: DateTime.Now.Add(TimeSpan.FromDays(-180d)).ToString("yyyy-MM-dd"))
                                    .Select(async ev =>
                                    {
                                        var user = await OktaClient.Users.GetUserAsync(ev.Actor.Id);

                                        return await user.PrintAttributesAsync(_attrs, OktaClient, _ofs);
                                    })
                                    .FirstOrDefaultAsync()
                                    // .Unwrap());
                                    );
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"{userId} - exception searching logs!!! {e.Message}");
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });
            await Task.WhenAll(tasks);
        }
    }
}