using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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
        private readonly IEnumerable<string> _attrNames;
        private readonly IEnumerable<string> _attrValues;

        private static readonly Regex Regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

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

            _attrNames = attrName.Contains(',') ? attrName.Split(',') : new[] {attrName};

            if (string.IsNullOrWhiteSpace(attrValue)) return;

            _attrValues = Regex.Split(attrValue);
            if (_attrValues.Count() < _attrNames.Count())
            {
                throw new Exception("List of values provided less than the number of fields to populate");
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Action's main entry
        /// </summary>
        public override async Task Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            // produce map of uid -> attrValue
            // if _attrVal is set -> override what comes from a source input
            var uidToValue = _attrValues == null
                ? lines
                    .Select(line => new List<string>(line.Trim().Split(new[] {' ', ','}, 2)))
                    .ToDictionary(lst => lst.First(), lst => Regex.Split(lst.Last()) as IEnumerable<string>)
                : lines
                    .Select(line => new List<string>(line.Trim().Split(new[] {' ', ','}, 2)))
                    .ToDictionary(lst => lst.First(), lst => _attrValues);

            var semaphore = new SemaphoreSlim(8);
            var tasks =
                uidToValue.Select(
                    async pair =>
                    {
                        var (uuid, values) = pair;

                        await semaphore.WaitAsync();
                        try
                        {
                            IUser oktaUser;

                            try
                            {
                                oktaUser = await OktaClient.Users.GetUserAsync(uuid);
                            }
                            catch (OktaApiException e)
                            {
                                Console.WriteLine(e.Message.Contains("Not found")
                                    ? $"{uuid} !!! user not found"
                                    : $"{uuid} !!! exception fetching the user: {e.Message}");
                                return;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"{uuid} !!! exception fetching the user: {e}");
                                return;
                            }

                            _attrNames.Zip(_attrValues).AsParallel().ForAll(
                                (nameValPair) =>
                                {
                                    var (attrName, attrVal) = nameValPair;
                                    // check if value is a list
                                    if (Regex.IsMatch(attrVal, "^\\([^)]*\\)$"))
                                    {
                                        var arrValues =
                                            Regex.Split(attrVal.Substring(1, attrVal.Length - 2))
                                                .Select(val => val.Replace("\"", ""));

                                        oktaUser.Profile[attrName] = arrValues;
                                    }
                                    else
                                    {
                                        oktaUser.Profile[attrName] = attrVal.Replace("\"", "");
                                    }
                                });

                            try
                            {
                                await oktaUser.UpdateAsync();

                                Console.WriteLine(
                                    $"Updating user {uuid}: set attribute {_attrNames} to {values} - success");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(
                                    $"Updating user {uuid}: set attribute {_attrNames} to {values} - update failed " +
                                    $"({e})");
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });
            await Task.WhenAll(tasks);
        }
    }
}