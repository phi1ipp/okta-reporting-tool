using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    public class AttributeSetter
    {
        private OktaConfig _oktaConfig;
        private FileInfo _fileInfo;
        private string _attrName;
        
        public AttributeSetter(OktaConfig config, FileInfo fileInfo, string attrName)
        {
            _oktaConfig = config;
            _fileInfo = fileInfo;
            _attrName = attrName;
        }

        public void Run()
        {
            // produce map of uid -> attrValue
            var uidToValue = File.ReadLines(_fileInfo.FullName)
                .Select(line => new List<string>(line.Split(",")))
                .ToDictionary(lst => lst.First(), lst => lst.Last());
            
            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey
            });
            
            // for each user set its profile[attr] = val
            foreach (var (uid, val) in uidToValue)
            {
                var oktaUser = oktaClient.Users.GetUserAsync(uid).Result;
                oktaUser.Profile[_attrName] = val;
                
                Console.WriteLine($"Updating user ${uid}: set attribute {_attrName} to {val}");
                
                oktaUser.UpdateAsync();
            }
        }
    }
}