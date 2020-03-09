using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class to run activate users operation
    /// </summary>
    public class ManageMembership : OktaAction
    {
        private readonly FileInfo _fileInfo;
        private readonly string _action;
        private readonly string _grpName;
        private readonly bool _grpIdUsed;
        private readonly ConcurrentDictionary<string, string> _dictGroupId = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Configuration object</param>
        /// <param name="fileInfo">File with UUID "comma separated list of groups"</param>
        /// <param name="action">String [add | remove] to indicate the operation for the given list of groups</param>
        /// <param name="grpName">String representing group(s) to be added or removed to every user</param>
        /// <param name="groupIdUsed">true if group UUID used instead of name</param>
        public ManageMembership(OktaConfig config, FileInfo fileInfo, string action, string grpName = null, bool groupIdUsed = false) : base(config)
        {
            _fileInfo = fileInfo;

            if (!new[] {"add", "remove", "display"}.Contains(action))
                throw new InvalidOperationException("action can be either 'add', 'remove' or 'display'");

            _action = action;
            _grpName = grpName;
            _grpIdUsed = groupIdUsed;
        }

        /// <summary>
        /// Main executable method to execute management operations
        /// </summary>
        public override async Task Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            var semaphore = new SemaphoreSlim(10);

            var tasks = lines
                .Select(async line =>
                {
                    var parts = line.Split(new[] {' ', ','}, 2);
                    var userId = parts[0];

                    await semaphore.WaitAsync();
                    try
                    {
                        var user = await OktaClient.Users.GetUserAsync(parts[0]);

                        if (_action == "display")
                        {
                            Console.WriteLine(await GetUserGroups(user));
                        }
                        else
                        {
                            var groups = _grpName == null
                                ? regex.Split(parts[1]).Select(grp => grp.Replace("\"", ""))
                                : regex.Split(_grpName).Select(grp => grp.Replace("\"", ""));

                            var lstGroups = groups.ToList();
                            
                            if (lstGroups.ToList().Any())
                                Console.WriteLine(await AddRemoveGroups(user, lstGroups));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e is OktaApiException && e.Message.Contains("Not found")
                            ? $"{userId} !!! user not found"
                            : $"{userId} !!! exception fetching the user: {e.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            await Task.WhenAll(tasks);
        }

        private async Task<string> AddRemoveGroups(IUser user, IEnumerable<string> groups)
        {
            var tasks =
                groups.Select(async grp =>
                {
                    string grpId;
                    
                    if (_grpIdUsed)
                        grpId = grp;
                        
                    else if (!_dictGroupId.TryGetValue(grp, out grpId))
                    {
                        grpId = await OktaClient.Groups
                            .ListGroups(q: grp)
                            .Select(g => g.Id)
                            .FirstOrDefault();

                        _dictGroupId[grp] = grpId;
                    }

                    if (grpId == null)
                    {
                        return $"{grp} doesn't exist in Okta";
                    }

                    try
                    {
                        switch (_action)
                        {
                            case "add":
                                await OktaClient.Groups.AddUserToGroupAsync(grpId, user.Id);
                                return $"{grp} added to {user.Profile.Login}";

                            case "remove":
                                await OktaClient.Groups.RemoveGroupUserAsync(grpId, user.Id);
                                return $"{grp} removed from {user.Profile.Login}";

                            default:
                                return $"unknown action: {_action}";
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is OktaApiException && e.Message.StartsWith("Not found"))
                            return $"{user.Id} not found";

                        return $"{user.Id} - exception: {e.Message}";
                    }
                });

            var allResults = await Task.WhenAll(tasks);
            return string.Join('\n', allResults);
        }

        private async Task<string> GetUserGroups(IUser user)
        {
            try
            {
                return user.Profile.Login + " " +
                       string.Join(',',
                           await OktaClient.Users
                               .ListUserGroups(user.Id)
                               .Select(g =>
                                   g.Profile.Name.Contains(' ')
                                       ? $"\"{g.Profile.Name}\""
                                       : g.Profile.Name)
                               .ToList());
            }
            catch (Exception e)
            {
                return $"{user.Profile.Login} - exception: {e.Message}";
            }
        }
    }
}