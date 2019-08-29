using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class to run a process of setting Okta's attribute for users
    /// </summary>
    public class AttributeSetter : OktaAction
    {
        private readonly FileInfo _fileInfo;
        private readonly string _attrName;

        /// <inheritdoc />
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="fileInfo">FileInfo to use as a source of records</param>
        /// <param name="attrName">Attribute name to be set</param>
        public AttributeSetter(OktaConfig config, FileInfo fileInfo, string attrName) : base(config)
        {
            _fileInfo = fileInfo;

            if (string.IsNullOrWhiteSpace(attrName))
                throw new InvalidOperationException("Required parameter --attrName is missing");

            _attrName = attrName;
        }

        /// <inheritdoc />
        /// <summary>
        /// Action's main entry
        /// </summary>
        public override void Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            // produce map of uid -> attrValue
            var uidToValue = lines
                .Select(line => new List<string>(line.Trim().Split(" ", 2)))
                .ToDictionary(lst => lst.First(), lst => lst.Last().Replace("\"", ""));

            uidToValue
                .AsParallel()
                .ForAll(pair =>
                {
                    var (key, value) = pair;
                    IUser oktaUser;

                    try
                    {
                        oktaUser = OktaClient.Users.GetUserAsync(key).Result;
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException is OktaApiException oktaApiException &&
                            oktaApiException.Message.Contains("Not found"))
                            Console.WriteLine($"{key} !!! user not found");
                        else
                        {
                            Console.WriteLine($"{key} !!! exception fetching the user");
                        }

                        return;
                    }

                    oktaUser.Profile[_attrName] = value;

                    try
                    {
                        oktaUser.UpdateAsync().Wait();

                        Console.WriteLine(
                            $"Updating user {key}: set attribute {_attrName} to {value} - success");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(
                            $"Updating user {key}: set attribute {_attrName} to {value} - update failed " +
                            $"({e.Message})");
                    }
                });
        }
    }
}