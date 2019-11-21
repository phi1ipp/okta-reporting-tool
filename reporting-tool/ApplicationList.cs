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
    /// <summary>
    /// Report to produce a list of applications
    /// </summary>
    public class ApplicationList : OktaAction
    {
        private readonly string _ofs;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta config instance</param>
        /// <param name="ofs">Output field separator</param>
        public ApplicationList(OktaConfig config, string ofs = " ") : base(config)
        {
            _ofs = ofs;
        }

        /// <summary>
        /// Main report entry
        /// </summary>
        public override async Task Run()
        {
            Console.WriteLine($"uuid,name,label");
            await OktaClient.Applications.ListApplications().ForEachAsync(app => {
                Console.WriteLine($"{app.Id}{_ofs}{app.Name}{_ofs}{app.Label}");
            });
        }
    }
}