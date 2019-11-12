using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Okta.Sdk;

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

            Console.WriteLine("userid" + _ofs + UserExtensions.PrintUserAttributesHeader(_attrs, _ofs));

            var channel = Channel.CreateUnbounded<string>();

            var readers = Task.Run(() => { StartReaders(channel.Reader); });

            lines
                .Select(line => line.Trim().Split(' ', ',').First())
                .AsParallel()
                .ForAll(userId => { channel.Writer.TryWrite(userId); });

            channel.Writer.Complete();
            await readers;
        }

        private void StartReaders(object channelReader)
        {
            if (!(channelReader is ChannelReader<string> reader))
            {
                throw new Exception("Reader is null");
            }

            var readers = Enumerable.Range(1, 4)
                .Select(async j =>
                {
                    while (await reader.WaitToReadAsync())
                    {
                        var userId = "";

                        try
                        {
                            userId = await reader.ReadAsync();

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
                                    .FirstOrDefault());
                        }
                        catch (Exception e)
                        {
                            if (e.InnerException is OktaApiException oktaException &&
                                oktaException.Message.StartsWith($"Not found:"))
                                Console.WriteLine($"{userId}: !!! not found");
                            else if (!(e is ChannelClosedException))
                            {
                                Console.WriteLine(userId + " !!!!! exception processing the user");
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                });

            Task.WhenAll(readers).Wait();
        }
    }
}