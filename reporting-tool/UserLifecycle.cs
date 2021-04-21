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
        /// <param name="action">Action to be taken: [activate, deactivate, suspend, unsuspend, delete]</param>
        public UserLifecycle(OktaConfig config, FileInfo fileInfo, string action) : base(config)
        {
            _fileInfo = fileInfo;
            _action = action;
        }

        /// <summary>
        /// Main executable method to perform activation
        /// </summary>
        public override async Task Run()
        {
            var lines = _fileInfo == null
                ? Program.ReadConsoleLines()
                : File.ReadLines(_fileInfo.FullName);

            var tasks = lines
                .Select(async line =>
                {
                    var parts = line.Trim().Split(' ', ',');
                    var userName = parts[0];
                    
                    try
                    {
                        var user = await OktaClient.Users.GetUserAsync(userName);

                        switch (_action)
                        {
                            case "set_pwd":
                                user.Credentials.Password = new PasswordCredential {Value = parts[1]};
                                Console.WriteLine($"{userName} set password to {parts[1]}");
                                await user.UpdateAsync();
                                break;
                            case "activate":
                                await user.ActivateAsync(sendEmail: false);
                                Console.WriteLine($"{userName} activated");
                                break;
                            case "activate_email":
                                await user.ActivateAsync(true);
                                Console.WriteLine($"{userName} activated and email sent");
                                break;
                            case "deactivate":
                                await user.DeactivateAsync();
                                Console.WriteLine($"{userName} deactivated");
                                break;
                            case "suspend":
                                await user.SuspendAsync();
                                Console.WriteLine($"{userName} suspended");
                                break;
                            case "unsuspend":
                                await user.UnsuspendAsync();
                                Console.WriteLine($"{userName} unsuspended");
                                break;
                            case "delete":
                                if (user.Status != UserStatus.Deprovisioned)
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
                        else if (e.Message.StartsWith("Activation failed because the user is already active"))
                        {
                            Console.WriteLine($"{userName} is already active");
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

            await Task.WhenAll(tasks);
        }
    }
}