using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    public class UserSearchReport
    {
        private OktaConfig _oktaConfig;
        private string _search;
        private ICollection<string> _attrs;

        public UserSearchReport(OktaConfig config, string search, string attrs)
        {
            _oktaConfig = config;
            _search = search;

            _attrs = attrs?.Split(",").ToHashSet();
        }

        public void Run()
        {
            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey
            });

            Console.WriteLine("id " + _attrs?.Aggregate((acc, attr) => acc + " " + attr));
            
            oktaClient.Users.ListUsers(search: _search).ForEach(user =>
            {
                Console.Write(user.Id + " ");
                foreach (var attr in _attrs)
                {
                    Console.Write($"{user.Profile[attr]} ");
                }
                Console.WriteLine();
            });
        }
    }
}