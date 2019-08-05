using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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

            var enumerator = File.ReadLines(_fileInfo.FullName).Select(line => line.Trim().Split(' ').First()).GetEnumerator();
            for (int i = 1; enumerator.MoveNext(); i++)
            {
                var uid = enumerator.Current;
                
                var res = oktaClient.Logs.GetLogs(
                        filter: $"eventType eq \"user.lifecycle.create\" and target.id eq \"{uid}\"",
                        since: DateTime.Now.Add(TimeSpan.FromDays(-180d)).ToString("yyyy-MM-dd"))
                    .Select(ev => ev.Target.First().Id + " " + ev.Actor.AlternateId + " " + ev.Actor.Id)
                    .First().Result;

                Console.WriteLine(res);


                if (i % 40 != 0) continue;
                
                Console.WriteLine("Pausing...");
                Thread.Sleep(80 * 1000);
            }
            enumerator.Dispose();
        }
    }
}