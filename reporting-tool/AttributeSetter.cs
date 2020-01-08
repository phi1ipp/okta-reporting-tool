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
        private readonly bool _writeEmpty;

        private static readonly Regex Regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        private static readonly Regex ListRegex = new Regex(",(?=(?:[^']*'[^']*')*(?![^']*'))");

        /// <inheritdoc />
        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Config instance</param>
        /// <param name="fileInfo">FileInfo to use as a source of records</param>
        /// <param name="attrName">Attribute name to be set</param>
        /// <param name="attrValue">Attribute value to be set for all users</param>
        /// <param name="writeEmpty">Skip empty values while updating user profile</param>
        public AttributeSetter(
            OktaConfig config, FileInfo fileInfo, string attrName, string attrValue, bool writeEmpty = false)
            : base(config)
        {
            _fileInfo = fileInfo;
            _writeEmpty = writeEmpty;

            if (string.IsNullOrWhiteSpace(attrName))
                throw new InvalidOperationException("Required parameter --attrName is missing");

            _attrNames = attrName.Contains(',') ? attrName.Split(',') : new[] {attrName};

            if (attrValue == null) return;

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

            var semaphore = new SemaphoreSlim(16);
            var tasks =
                uidToValue.Select(
                    async pair =>
                    {
                        var (userId, values) = pair;

                        var lstValues = values.ToList();
                        if (lstValues.Count() != _attrNames.Count())
                        {
                            Console.WriteLine("Attribute values count does not match attribute names count");
                            return;
                        }

                        await semaphore.WaitAsync();
                        try
                        {
                            IUser oktaUser;

                            try
                            {
                                oktaUser = userId.Contains('/')
                                    ? await OktaClient.Users.ListUsers(search: $"profile.login eq \"{userId}\"").First()
                                    : await OktaClient.Users.GetUserAsync(userId);
                            }
                            catch (OktaApiException e)
                            {
                                Console.WriteLine(e.Message.Contains("Not found")
                                    ? $"{userId} !!! user not found"
                                    : $"{userId} !!! exception fetching the user: {e.Message}");
                                return;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"{userId} !!! exception fetching the user: {e}");
                                return;
                            }

                            _attrNames.Zip(lstValues).AsParallel().ForAll(
                                (nameValPair) =>
                                {
                                    var (attrName, attrVal) = nameValPair;

                                    if (!_writeEmpty && string.IsNullOrEmpty(attrVal)) return;

                                    // check if value is a list
                                    if (Regex.IsMatch(attrVal,"^\"\\([^)]*\\)\"$"))
                                    {
                                        var arrValues =
                                            ListRegex.Split(attrVal.Substring(2, attrVal.Length - 4))
                                                .Select(val => val.Replace("'", ""))
                                                .ToList();

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
                                    $"Updating user {userId}: set attributes {string.Join(",", _attrNames)} to {string.Join(",", lstValues)} - success");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(
                                    $"Updating user {userId}: set attribute {string.Join(",", _attrNames)} to {string.Join(",", lstValues)} - update failed " +
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