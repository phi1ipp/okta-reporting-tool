using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace reporting_tool
{
    /// <summary>
    /// Class to run a process of setting Okta's attribute for users
    /// </summary>
    public class AttributeSetter : OktaAction
    {
        private FileInfo _fileInfo;
        private string _attrName;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="fileInfo">FileInfo to use as a source of records</param>
        /// <param name="attrName">Attribute name to be set</param>
        public AttributeSetter(OktaConfig config, FileInfo fileInfo, string attrName) : base(config)
        {
            _fileInfo = fileInfo;
            _attrName = attrName;
        }

        /// <summary>
        /// Action's main entry
        /// </summary>
        public override void Run()
        {
            // produce map of uid -> attrValue
            var uidToValue = File.ReadLines(_fileInfo.FullName)
                .Select(line => new List<string>(line.Trim().Split(" ", 2)))
                .ToDictionary(lst => lst.First(), lst => lst.Last().Replace("\"", ""));

            uidToValue.AsParallel().ForAll(pair =>
            {
                var oktaUser = OktaClient.Users.GetUserAsync(pair.Key).Result;
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