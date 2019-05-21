using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    public class CreatorReport
    {
        private OktaConfig _oktaConfig;
        private FileInfo _fileInfo;

        public CreatorReport(OktaConfig config, FileInfo fileInfo)
        {
            _oktaConfig = config;
            _fileInfo = fileInfo;
        }

        public void Run()
        {
            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey
            });

            foreach (var s in File.ReadLines(_fileInfo.FullName)
                .Select(line => line.Trim())
                .Select(uid => oktaClient.Logs.GetLogs(
                        filter: $"eventType eq \"user.lifecycle.create\" and target.id eq \"{uid}\"",
                        since: DateTime.Now.Add(TimeSpan.FromDays(-180d)).ToString("yyyy-MM-dd"))
                    .Select(ev => ev.Target.First().Id + " " + ev.Actor.AlternateId)
                    .First().Result
                ))
            {
                Console.WriteLine(s);
            }
        }
    }
}