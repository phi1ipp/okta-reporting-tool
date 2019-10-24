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
    public class ApplicationList : OktaAction
    {
        private readonly string _ofs;
        
        public ApplicationList(OktaConfig config, string ofs = " ") : base(config)
        {
            _ofs = ofs;
        }

        public override void Run()
        {
            Console.WriteLine($"uuid,name,label");
            OktaClient.Applications.ListApplications().ForEach(app =>
            {
                Console.WriteLine($"{app.Id}{_ofs}{app.Name}{_ofs}{app.Label}");
            });
        }
    }
}