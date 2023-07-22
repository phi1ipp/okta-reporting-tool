using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class to generate a report on all groups
    /// </summary>
    public class GroupDelete : OktaAction
    {
        private readonly string _ofs;
        private readonly FileInfo _fileInfo;
        private readonly bool _groupIdUsed;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">OktaConfig instance</param>
        /// <param name="fileInfo">Input file info</param>
        /// <param name="groupIdUsed">true if group Id is used instead of group name</param>
        /// <param name="ofs">Output field separator</param>
        public GroupDelete(OktaConfig config, FileInfo fileInfo, bool groupIdUsed = false, string ofs = ",") : base(config)
        {
            _fileInfo = fileInfo;
            _groupIdUsed = groupIdUsed;
            _ofs = ofs;
        }
        /// <summary>
        /// The report entry point
        /// </summary>
        public override async Task Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            var semaphore = new SemaphoreSlim(16);

            var tasks = lines.Select(async line => {
                await semaphore.WaitAsync();

                string grpId;

                if (_groupIdUsed) {
                    grpId = line;
                } else {
                    try {
                        var oktaGrp = await OktaClient.Groups.ListGroups(line).FirstAsync();
                        grpId = oktaGrp.Id;
                    } catch (Exception e) {
                        await Console.Out.WriteLineAsync($"EXCEPTION getting group {line}: {e.Message}");
                        semaphore.Release();
                        return;
                    }
                }
                    
                try {
                    await OktaClient.Groups.DeleteGroupAsync(grpId);

                    await Console.Out.WriteLineAsync($"{line} group deleted");
                } catch (Exception e) {
                        await Console.Out.WriteLineAsync($"EXCEPTION creating group {line}: {e.Message}");

                } finally {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}