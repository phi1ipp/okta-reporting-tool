using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace reporting_tool
{
    /// <summary>
    /// Class to generate a report on all groups
    /// </summary>
    public class GroupRename : OktaAction
    {
        private readonly FileInfo _input;
        private readonly bool _useIds;

        private static readonly Regex Regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">OktaConfig instance</param>
        /// <param name="input">File to be used as input</param>
        /// <param name="useIds">Indicate that groups will be searched by their GUIDs</param>
        public GroupRename(OktaConfig config, FileInfo input, bool useIds = true) : base(config)
        {
            _input = input;
            _useIds = useIds;
        }

        /// <summary>
        /// The report entry point
        /// </summary>
        public override async Task Run()
        {
            var lines = _input == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_input.FullName);

            var semaphore = new SemaphoreSlim(8);

            var tasks =
                lines.Select(async line =>
                {
                    await semaphore.WaitAsync();
                    var parts = Regex.Split(line);

                    try
                    {
                        var group = _useIds
                            ? await OktaClient.Groups.GetGroupAsync(parts[0])
                            : await OktaClient.Groups.ListGroups(parts[0]).First();

                        group.Profile.Name = parts[1];
                        await group.UpdateAsync();

                        var res = $"{parts[0]} has been updated to {parts[1]}";
                        Console.WriteLine(res);
                        
                        return res;
                    }
                    catch (Exception e)
                    {
                        var res = $"{e.Message}";
                        
                        Console.WriteLine(res);
                        return res;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            await Task.WhenAll(tasks.ToArray());
        }
    }
}