using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        public override void Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            lines
                .AsParallel()
                .Select(line =>
                {
                    var parts = line.Split(' ', 2);
                    var groups = regex.Split(parts[1]);

                    return new Tuple<string, List<string>>(
                        parts[0],
                        groups.Select(grp => grp.Replace("\"", "")).ToList());
                })
                .ForAll(tuple =>
                {
                    var (uuid, groups) = tuple;

                    var tasks = groups.Select(grp => Task.Run(() =>
                    {
                        var grpId = OktaClient.Groups
                            .ListGroups(q: grp)
                            .Select(g => g.Id)
                            .FirstOrDefault()
                            .Result;

                        if (grpId == null)
                        {
                            return $"{grp} doesn't exist in Okta";
                        }

                        switch (_action)
                        {
                            case "add":
                                OktaClient.Groups.AddUserToGroupAsync(grpId, uuid).Wait();
                                return $"{grp} added to {uuid}";

                            case "remove":
                                OktaClient.Groups.RemoveGroupUserAsync(grpId, uuid).Wait();
                                return ($"{grp} removed from {uuid}");

                            case "display":
                                return uuid + " " +
                                       string.Join(',',
                                           OktaClient.Users
                                               .ListUserGroups(uuid)
                                               .Select(g =>
                                                   g.Profile.Name.Contains(' ')
                                                       ? $"\"{g.Profile.Name}\""
                                                       : g.Profile.Name)
                                               .ToList()
                                               .Result);
                            default:
                                return $"unknown action: {_action}";
                        }
                    }));

                    var operationsResults = Task.WhenAll(tasks).Result;
                    Console.WriteLine(string.Join('\n', operationsResults));
                });
        }
    }
}