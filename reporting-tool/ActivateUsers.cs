using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;
using Okta.Sdk.Configuration;

namespace reporting_tool
{
    /// <summary>
    /// Class to run activate users operation
    /// </summary>
    public class ActivateUsers
    {
        private readonly OktaConfig _oktaConfig;
        private readonly FileInfo _fileInfo;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Configuration object</param>
        /// <param name="fileInfo"></param>
        public ActivateUsers(OktaConfig config, FileInfo fileInfo)
        {
            _oktaConfig = config;
            _fileInfo = fileInfo;
        }

        /// <summary>
        /// Main executable method to perform activation
        /// </summary>
        public void Run()
        {
            var oktaClient = new OktaClient(new OktaClientConfiguration
            {
                OktaDomain = _oktaConfig.Domain,
                Token = _oktaConfig.ApiKey,
                RateLimitPreservationPercent = 10
            });

            var tasks = File.ReadLines(_fileInfo.FullName)
                .Select(async line =>
                {
                    var userName = line.Trim().Split(' ', ',').First();

                    await ActivateUser(userName, oktaClient);
                });

            Task.WhenAll(tasks);
        }

        private static async Task ActivateUser(string userName, IOktaClient oktaClient)
        {
            try
            {
                var user = await oktaClient.Users.GetUserAsync(userName);

                await user.ActivateAsync(false);

                Console.WriteLine($"{userName} activated successfully");
            }
            catch (Exception e)
            {
                if (e.InnerException is OktaApiException oktaException &&
                    oktaException.Message.StartsWith("Not found:"))
                {
                    Console.WriteLine(userName + " !!!!! user not found");
                }
                else
                {
                    Console.WriteLine(userName + " !!!!! exception processing the user");
                    Console.WriteLine(e);
                }
            }
        }
    }
}