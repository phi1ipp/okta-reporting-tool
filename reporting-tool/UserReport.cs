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
    /// <inheritdoc />
    /// <summary>
    /// Class to run user report based on user Ids
    /// </summary>
    public class UserReport : OktaAction
    {
        private readonly FileInfo _fileInfo;
        private readonly IEnumerable<string> _attrs;
        private readonly string _attrName;
        private readonly string _ofs;

        /// <inheritdoc />
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="fileInfo">FileInfo to take user ids from</param>
        /// <param name="attrName">Specifies a profile attribute which to identify user with against given list of values</param>
        /// <param name="attrs">User profile attributes to output</param>
        /// <param name="ofs">OFS string</param>
        public UserReport(OktaConfig config, FileInfo fileInfo, string attrName, string attrs, string ofs = ",") :
            base(config)
        {
            _fileInfo = fileInfo;
            _attrName = attrName;
            _attrs = string.IsNullOrEmpty(attrs)
                ? Enumerable.Empty<string>()
                : attrs.Trim().Split(',').Select(attr => attr.Trim()).ToHashSet();
            _ofs = ofs;
        }

        /// <inheritdoc />
        /// <summary>
        /// Report's main entry
        /// </summary>
        public override void Run()
        {
            Console.WriteLine(UserExtensions.PrintUserAttributesHeader(_attrs, _ofs));

            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            var channel = Channel.CreateUnbounded<string>();

            var processingThread = new Thread(StartReaders);
            processingThread.Start(channel.Reader);

            lines
                .AsParallel()
                .ForAll(line =>
                {
                    var userName = line.Trim().Split(' ', ',').First();
                    channel.Writer.TryWrite(userName);
                });

            channel.Writer.Complete();

            while (processingThread.IsAlive)
            {
                Task.Delay(100).Wait(); 
            }
        }

        void StartReaders(object channelReader)
        {
            if (!(channelReader is ChannelReader<string> reader))
            {
                throw new Exception("Reader is null");
            }

            var readers =
                Enumerable.Range(1, 8)
                    .Select(async j =>
                    {
                        while (await reader.WaitToReadAsync())
                        {
                            var userName = await reader.ReadAsync();

                            try
                            {
                                var users = string.IsNullOrWhiteSpace(_attrName)
                                    ? new List<IUser> {await OktaClient.Users.GetUserAsync(userName)}
                                    : await OktaClient.Users.ListUsers(search: $"profile.{_attrName} eq \"{userName}\"")
                                        .ToList();

                                if (users.Count == 0)
                                {
                                    Console.WriteLine(userName + " !!!!! user not found");
                                    continue;
                                }

                                var tasks = users.Select(async user =>
                                {
                                    Console.WriteLine(await user.PrintAttributesAsync(_attrs, OktaClient, _ofs));
                                });

                                await Task.WhenAll(tasks);
                            }
                            catch (Exception e)
                            {
                                if (e.InnerException is OktaApiException oktaException &&
                                    oktaException.Message.StartsWith("Not found:"))
                                {
                                    Console.WriteLine(userName + " !!!!! user not found");
                                }
                                else
                                {
                                    Console.WriteLine(userName + " !!!!! exception processing the user");
                                    Console.WriteLine(e);
                                }
                            }
                        }
                    });

            Task.WhenAll(readers).Wait();
        }
    }
}