using System;
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
    /// Class to run a report for a specific Okta group and apply an additional filter to users from the group
    /// </summary>
    public class AppUserLifecycle : OktaAction
    {
        private readonly string _appLabel;
        private readonly string _action;
        private readonly IEnumerable<string> _attrs;
        private readonly string _ofs;
        private readonly FileInfo _input;
        
        private Regex guidRegex = new Regex("^00.{15}297$");

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="appLabel">Application label</param>
        /// <param name="input">Input file with list of users</param>
        /// <param name="action">Add/remove user to the application</param>
        /// <param name="ofs">Output field separator</param>
        public AppUserLifecycle(OktaConfig config, string appLabel, FileInfo input, string action, string attrs, string ofs = ",") :
            base(config)
        {
            _ofs = ofs;
            _appLabel = appLabel;
            _action = action;
            _input = input;
            _attrs = attrs.Split(",");
        }

        /// <inheritdoc />
        /// <summary>
        /// Report's main entry
        /// </summary>
        /// <returns></returns>
        public override async Task Run()
        {
            var appId = await OktaClient.Applications
                .ListApplications(q: _appLabel)
                .Select(app => app.Id)
                .First();

            if (appId == null)
            {
                throw new Exception($"Application {_appLabel} doesn't exist");
            }
            
            var lines = _input == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_input.FullName);

            var semaphor = new SemaphoreSlim(8);
            
            var tasks = lines
                .Select(async line =>
                {
                    var userId = line.Split(new[] {' ', ','}, 2)[0];

                    await semaphor.WaitAsync();

                    string userGuid;

                    try
                    {
                        if (!guidRegex.IsMatch(userId))
                        {
                            var user = await OktaClient.Users.GetUserAsync(userId);
                            userGuid = user.Id;
                        }
                        else
                        {
                            userGuid = userId;
                        }

                        switch (_action)
                        {
                            case "delete" :
                                await OktaClient.Applications.DeleteApplicationUserAsync(appId, userGuid);
                                Console.WriteLine($"{userGuid} removed from {_appLabel}");
                                break;
                            
                            case "display" :
                                var appUser = await OktaClient.Applications
                                    .GetApplicationUserAsync(appId, userGuid);

                                Console.WriteLine($"{appUser.Id}{_ofs}{appUser.ExternalId}" + OutputAppProfile(appUser));
                                break;
                            
                            default:
                                Console.WriteLine($"{_action} not supported");
                                break;
                        }
                    }
                    catch (OktaApiException e)
                    {
                        Console.WriteLine(e.Message.Contains("Not found")
                            ? $"{userId} !!! user not found"
                            : $"{userId} !!! exception fetching the user: {e.Message}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{userId} !!! exception fetching the user: {e}");
                    }
                    finally
                    {
                        semaphor.Release();
                    }
                });

            await Task.WhenAll(tasks);
        }
        private string OutputAppProfile(IAppUser appUser)
        {
            return appUser.Profile.GetData()
                .Where(pair => _attrs.Contains(pair.Key) || !_attrs.Any())
                .Aggregate("{}",
                    (s, pair) => s.Replace("}",
                        pair.Value == null ? $",{pair.Key}:null}}" : $",{pair.Key}:{pair.Value}}}"));
        }
    }
}