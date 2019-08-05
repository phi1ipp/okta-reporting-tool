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
                .Select(line => new List<string>(line.Trim().Split(',', ' ')))
                .ToDictionary(lst => lst.First(), lst => lst.Last());

            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey
            });

            uidToValue.AsParallel().ForAll(pair =>
            {
                var oktaUser = oktaClient.Users.GetUserAsync(pair.Key).Result;
                oktaUser.Profile[_attrName] = pair.Value;

                try
                {
                    var updatedUser = oktaUser.UpdateAsync().Result;

                    Console.WriteLine($"Updating user {pair.Key}: set attribute {_attrName} to {pair.Value} - success");
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        $"Updating user {pair.Key}: set attribute {_attrName} to {pair.Value} - update failed " +
                        $"({e.Message})");
                }
            });
        }
    }
}