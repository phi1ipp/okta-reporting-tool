using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Threading.Tasks;
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
        private readonly string _attrVal;

        /// <inheritdoc />
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="fileInfo">FileInfo to use as a source of records</param>
        /// <param name="attrName">Attribute name to be set</param>
        /// <param name="attrValue">Attribute value to be set for all users</param>
        public AttributeSetter(OktaConfig config, FileInfo fileInfo, string attrName, string attrValue) : base(config)
        {
            _fileInfo = fileInfo;

            if (string.IsNullOrWhiteSpace(attrName))
                throw new InvalidOperationException("Required parameter --attrName is missing");

            _attrName = attrName;

            if (!string.IsNullOrWhiteSpace(attrValue))
                _attrVal = attrValue;
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
            // if _attrVal is set -> override what comes from a source input
            var uidToValue = _attrVal == null
                ? lines
                    .Select(line => new List<string>(line.Trim().Split(new[] {' ', ','}, 2)))
                    .ToDictionary(lst => lst.First(), lst => lst.Last())
                : lines
                    .Select(line => new List<string>(line.Trim().Split(new[] {' ', ','}, 2)))
                    .ToDictionary(lst => lst.First(), lst => _attrVal);

            var channel = Channel.CreateUnbounded<Tuple<string, string>>();

            var readers =
                Enumerable.Range(1, 8)
                    .Select(async j => { await StartReader(channel); });

            uidToValue
                .AsParallel()
                .ForAll(pair =>
                {
                    var (key, value) = pair;
                    channel.Writer.TryWrite(new Tuple<string, string>(key, value));
                });

            channel.Writer.Complete();

            Task.WhenAll(readers).Wait();
        }

        private async Task StartReader(Channel<Tuple<string, string>> channel)
        {
            var reader = channel.Reader;

            while (await reader.WaitToReadAsync())
            {
                var (uuid, value) = await reader.ReadAsync();

                IUser oktaUser;

                try
                {
                    oktaUser = await OktaClient.Users.GetUserAsync(uuid);
                }
                catch (Exception e)
                {
                    if (e.InnerException is OktaApiException oktaApiException &&
                        oktaApiException.Message.Contains("Not found"))
                        Console.WriteLine($"{uuid} !!! user not found");
                    else
                    {
                        Console.WriteLine($"{uuid} !!! exception fetching the user");
                    }

                    break;
                }

                // check if value is a list
                if (Regex.IsMatch(value, "^\\([^)]*\\)$"))
                {
                    var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    var arrValues = regex.Split(value.Substring(1, value.Length - 2))
                        .Select(val => val.Replace("\"", ""));

                    oktaUser.Profile[_attrName] = arrValues;
                }
                else
                {
                    oktaUser.Profile[_attrName] = value.Replace("\"", "");
                }

                try
                {
                    await oktaUser.UpdateAsync();

                    Console.WriteLine(
                        $"Updating user {uuid}: set attribute {_attrName} to {value} - success");
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        $"Updating user {uuid}: set attribute {_attrName} to {value} - update failed " +
                        $"({e.Message})");
                }
            }
        }
    }
}