using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    public class UserReport
    {
        private OktaConfig _oktaConfig;
        private FileInfo _fileInfo;
        private ICollection<string> _attrs;

        public UserReport(OktaConfig config, FileInfo fileInfo, string attrs)
        {
            _oktaConfig = config;
            _fileInfo = fileInfo;
            _attrs = attrs.Trim().Split(",").Select(attr => attr.Trim()).ToHashSet();
        }

        public void Run()
        {
            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey,
                RateLimitPreservationPercent = 10
            });

            Console.WriteLine("userid " + string.Join(' ', _attrs));

            File.ReadLines(_fileInfo.FullName)
                .ToList()
                .AsParallel()
                .ForAll(line =>
                {
                    var userName = line.Trim().Split(',').First();

                    try
                    {
                        var user = oktaClient.Users.GetUserAsync(userName).Result;
                        Console.WriteLine(userName + " " + string.Join(' ', _attrs.Select(attr => user.Profile[attr])));
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException is OktaApiException)
                        {
                            var oktaException = e.InnerException as OktaApiException;

                            if (oktaException.Message.StartsWith("Not found:"))
                            {
                                Console.WriteLine(userName + " !!!!! user not found");
                            }
                            else
                            {
                                Console.WriteLine(userName + " !!!!! exception processing the user");
                                Console.WriteLine(e);
                            }
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