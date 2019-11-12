using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class to run activate users operation
    /// </summary>
    public class ManageGroups : OktaAction
    {
        private readonly FileInfo _fileInfo;
        private readonly string _action;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Configuration object</param>
        /// <param name="fileInfo">File with UUID "comma separated list of groups"</param>
        /// <param name="action">String [add | remove] to indicate the operation for the given list of groups</param>
        public ManageGroups(OktaConfig config, FileInfo fileInfo, string action) : base(config)
        {
            _fileInfo = fileInfo;

            if (!new[] {"add", "remove", "display"}.Contains(action))
                throw new InvalidOperationException("action can be either 'add', 'remove' or 'display'");

            _action = action;
        }

        /// <summary>
        /// Main executable method to execute management operations
        /// </summary>
        public override async Task Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            var channel = Channel.CreateUnbounded<Tuple<string, IEnumerable<string>>>();

            var readers =
                Enumerable.Range(1, 8)
                    .Select(async j => { await StartReader(channel); });

            var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            lines
                .AsParallel()
                .ForAll(line =>
                {
                    var parts = line.Split(' ', 2);
                    var groups = regex.Split(parts[1]);

                    channel.Writer.TryWrite(
                        new Tuple<string, IEnumerable<string>>(
                            parts[0],
                            groups.Select(grp => grp.Replace("\"", "")).ToList()));
                });
            channel.Writer.Complete();

            await Task.WhenAll(readers);
        }

        private async Task StartReader(Channel<Tuple<string, IEnumerable<string>>> channel)
        {
            var reader = channel.Reader;

            while (await reader.WaitToReadAsync())
            {
                var (uuid, groups) = await reader.ReadAsync();

                if (_action == "display")
                {
                    Console.WriteLine(await GetUserGroups(uuid));
                }
                else
                {
                    Console.WriteLine(await AddRemoveGroups(uuid, groups));
                }
            }
        }

        private async Task<string> AddRemoveGroups(string uuid, IEnumerable<string> groups)
        {
            foreach (var grp in groups)
            {
                var grpId = await OktaClient.Groups
                    .ListGroups(q: grp)
                    .Select(g => g.Id)
                    .FirstOrDefault();

                if (grpId == null)
                {
                    return $"{grp} doesn't exist in Okta";
                }

                try
                {
                    switch (_action)
                    {
                        case "add":
                            await OktaClient.Groups.AddUserToGroupAsync(grpId, uuid);
                            return $"{grp} added to {uuid}";

                        case "remove":
                            await OktaClient.Groups.RemoveGroupUserAsync(grpId, uuid);
                            return $"{grp} removed from {uuid}";

                        default:
                            return $"unknown action: {_action}";
                    }
                }
                catch (Exception e)
                {
                    if (e is OktaApiException && e.Message.StartsWith("Not found"))
                        return $"{uuid} not found";

                    return $"{uuid} - exception: {e.Message}";
                }
            }

            throw new NotImplementedException();
        }

        private async Task<string> GetUserGroups(string uuid)
        {
            try
            {
                return uuid + " " +
                       string.Join(',',
                           await OktaClient.Users
                               .ListUserGroups(uuid)
                               .Select(g =>
                                   g.Profile.Name.Contains(' ')
                                       ? $"\"{g.Profile.Name}\""
                                       : g.Profile.Name)
                               .ToList());
            }
            catch (Exception e)
            {
                if (e.InnerException is OktaApiException oktaApiException &&
                    oktaApiException.Message.StartsWith("Not found"))
                    return $"{uuid} not found";

                return $"{uuid} - exception: {e.Message}";
            }
        }
    }
}