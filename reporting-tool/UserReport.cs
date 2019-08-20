using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Okta.Sdk;

namespace reporting_tool
{
    /// <inheritdoc />
    /// <summary>
    /// Class to run user report based on user Ids
    /// </summary>
    public class UserReport : OktaAction
    {
        private static readonly IEnumerable<string> Empty = Enumerable.Empty<string>();
        private readonly FileInfo _fileInfo;
        private readonly IEnumerable<string> _attrs;

        /// <inheritdoc />
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="fileInfo">FileInfo to take user ids from</param>
        /// <param name="attrs">User profile attributes to output</param>
        public UserReport(OktaConfig config, FileInfo fileInfo, string attrs) : base(config)
        {
            _fileInfo = fileInfo;
            _attrs = string.IsNullOrEmpty(attrs)
                ? Empty
                : attrs.Trim().Split(',').Select(attr => attr.Trim()).ToHashSet();
        }

        /// <inheritdoc />
        /// <summary>
        /// Report's main entry
        /// </summary>
        public override void Run()
        {
            Console.WriteLine("id " +
                              string.Join(" ",
                                  _attrs.Where(attr => UserAttributes.NonProfileAttribute.Contains(attr))) + " " +
                              string.Join(" ",
                                  _attrs.Where(attr => !UserAttributes.NonProfileAttribute.Contains(attr)))
            );

            var lines = _fileInfo == null ? Program.ReadConsoleLines() : File.ReadLines(_fileInfo.FullName);

            lines
                .AsParallel()
                .ForAll(line =>
                {
                    var userName = line.Trim().Split(' ').First();

                    try
                    {
                        var user = OktaClient.Users.GetUserAsync(userName).Result;
                        Console.WriteLine($"{user.Id} " +
                                          string.Join(" ",
                                              _attrs
                                                  .Where(attr => UserAttributes.NonProfileAttribute.Contains(attr))
                                                  .Select(user.GetNonProfileAttribute)) + " " +
                                          string.Join(" ",
                                              _attrs
                                                  .Where(attr => !UserAttributes.NonProfileAttribute.Contains(attr))
                                                  .Select(attr => user.Profile[attr]?.ToString())));
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
                });
        }
    }
}