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
            var uids = File.ReadLines(_fileInfo.FullName).Select(line => line.Trim()).ToList();

            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey
            });

            var logs = oktaClient.Logs.GetLogs(filter: "eventType eq \"user.lifecycle.create\"", limit: 1000);

            logs
                .Where(ev => uids.Intersect(ev.Target.Select(tgt => tgt.Id)).Any())
                .Select(ev => ev.Target.Select(tgt => tgt.Id).First() + " " + ev.Actor.Id)
                .ForEachAsync(str => Console.WriteLine(str));
        }
    }
}