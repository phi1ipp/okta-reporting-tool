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
    public class GroupCreate : OktaAction
    {
        private readonly string _ofs;
        private readonly FileInfo _fileInfo;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">OktaConfig instance</param>
        /// <param name="fileInfo">Input file info</param>
        /// <param name="ofs">Output field separator</param>
        public GroupCreate(OktaConfig config, FileInfo fileInfo, string ofs = ",") : base(config)
        {
            _fileInfo = fileInfo;
            _ofs = ofs;
        }
        /// <summary>
        /// The report entry point
        /// </summary>
        public override async Task Run()
        {
            var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            Console.WriteLine($"uuid{_ofs}name");

            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            var semaphore = new SemaphoreSlim(16);

            var tasks = lines.Select(async line => {
                await semaphore.WaitAsync();

                var input = regex.Split(line.Trim());
                var groupName = input[0];
                var groupDescription = input[1];
                
                // prepare group parameters
                // clear if required from starting/ending double quotes
                var oktaGroupOptions = new CreateGroupOptions()
                {
                    Name = groupName.Substring(0,1) == "\"" ? groupName.Substring(1, groupName.Length - 2) : groupName,
                    Description = groupDescription.Substring(0,1) == "\"" ? groupDescription.Substring(1, groupDescription.Length - 2) : groupDescription
                };
                
                try {

                    var grp = await OktaClient.Groups.CreateGroupAsync(oktaGroupOptions);

                    var outputLine = grp.Profile.Name.Contains(_ofs)
                        ? $"{grp.Id}{_ofs}\"{grp.Profile.Name}\""
                        : $"{grp.Id}{_ofs}{grp.Profile.Name}";

                    await Console.Out.WriteLineAsync(outputLine);
                } catch (Exception e) {
                    if (e.Message.Contains("already exists"))
                        await Console.Out.WriteLineAsync($"GROUP {oktaGroupOptions.Name} already exists");
                    else 
                        await Console.Out.WriteLineAsync($"EXCEPTION creating group {oktaGroupOptions.Name}: {e.Message}");

                } finally {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}