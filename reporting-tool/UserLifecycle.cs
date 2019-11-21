using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Okta.Sdk;

namespace reporting_tool
{
    /// <summary>
    /// Class to run activate users operation
    /// </summary>
    public class UserLifecycle : OktaAction
    {
        private readonly FileInfo _fileInfo;
        private readonly string _action;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="config">Okta Configuration object</param>
        /// <param name="fileInfo">File with input data</param>
        /// <param name="action">Action to be taken: [activate, deactivate, delete]</param>
        public UserLifecycle(OktaConfig config, FileInfo fileInfo, string action) : base(config)
        {
            _fileInfo = fileInfo;
            _action = action;
        }

        /// <summary>
        /// Main executable method to perform activation
        /// </summary>
        public override Task Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            var tasks = lines
                .Select(async line =>
                {
                    var userName = line.Trim().Split(' ', ',').First();
                    try
                    {
                        var user = await OktaClient.Users.GetUserAsync(userName);

                        switch (_action)
                        {
                            case "activate":
                                await user.ActivateAsync();
                                Console.WriteLine($"{userName} activated");
                                break;
                            case "deactivate":
                                await user.DeactivateAsync();
                                Console.WriteLine($"{userName} deactivated");
                                break;
                            case "delete":
                                if (user.Status != UserStatus.Suspended)
                                    await user.DeactivateAsync();
                                
                                await user.DeactivateOrDeleteAsync();
                                Console.WriteLine($"{userName} deleted");
                                break;
                            default:
                                throw new InvalidOperationException($"{_action} is not supported");
                        }
                    }
                    catch (OktaApiException e)
                    {
                        if (e.Message.StartsWith("Not found"))
                        {
                            Console.WriteLine(userName + " !!!!! user not found");
                        }
                        else
                        {
                            Console.WriteLine(userName + " exception processing the user: " + e);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(userName + " exception processing the user: " + e);
                    }
                });

            return Task.WhenAll(tasks);
        }
    }
}