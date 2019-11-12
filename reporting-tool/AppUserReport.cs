using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
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
            var appId = OktaClient.Applications
                .ListApplications(q: _appLabel)
                .Select(app => app.Id)
                .First().Result;

            if (appId == null)
            {
                throw new Exception($"Application {_appLabel} doesn't exist");
            }

            var channel = Channel.CreateUnbounded<Tuple<string, string>>();

            var readers = Task.Run(() => { StartReaders(channel.Reader); });

            var lines = _input == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_input.FullName);

            lines
                .AsParallel()
                .ForAll(line =>
                    channel.Writer.TryWrite(
                        new Tuple<string, string>(appId, line.Split(new[] {' ', ','}, 1)[0])));

            channel.Writer.Complete();
            await readers;
        }

        private void StartReaders(object channelReader)
        {
            if (!(channelReader is ChannelReader<Tuple<string, string>> reader))
            {
                throw new Exception("Reader is null");
            }

            var readers = Enumerable.Range(1, 4)
                .Select(async j =>
                {
                    while (await reader.WaitToReadAsync())
                    {
                        var (appId, userId) = await reader.ReadAsync();

                        try
                        {
                            var appUser = await OktaClient.Applications
                                .GetApplicationUserAsync(appId, userId);
                            
                            Console.WriteLine($"{appUser.Id}{_ofs}{appUser.ExternalId}");
                        }
                        catch (OktaApiException e)
                        {
                            if (e.ErrorSummary.StartsWith("Not found"))
                                Console.WriteLine($"{userId} is not assigned to application");
                        }
                    }
                });

            Task.WhenAll(readers).Wait();
        }
    }
}