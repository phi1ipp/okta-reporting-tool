using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class to run a report for a specific Okta group and apply an additional filter to users from the group
    /// </summary>
    public class AppUserReport : OktaAction
    {
        private readonly string _appLabel;
        private readonly string _ofs;
        private readonly FileInfo _input;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="appLabel">Application label</param>
        /// <param name="input">Input file with list of users</param>
        /// <param name="ofs">Output field separator</param>
        public AppUserReport(OktaConfig config, string appLabel, FileInfo input, string ofs = ",") : base(config)
        {
            _ofs = ofs;
            _appLabel = appLabel;
            _input = input;
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
                .AsParallel()
                .Select(async line =>
                {
                    var userId = line.Split(new[] {' ', ','}, 1)[0];

                    await semaphor.WaitAsync();

                    try
                    {
                        var appUser = await OktaClient.Applications
                            .GetApplicationUserAsync(appId, userId);

                        Console.WriteLine($"{appUser.Id}{_ofs}{appUser.ExternalId}");
                    }
                    catch (OktaApiException e)
                    {
                        Console.WriteLine(e.Message.Contains("Not found")
                            ? $"{userId} !!! user not found"
                            : $"{userId} !!! exception fetching the user: {e.Message}");
                        return;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{userId} !!! exception fetching the user: {e}");
                        return;
                    }
                    finally
                    {
                        semaphor.Release();
                    }
                });

            await Task.WhenAll(tasks);
        }
    }
}