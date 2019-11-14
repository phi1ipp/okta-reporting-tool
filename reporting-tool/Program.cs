using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

namespace reporting_tool
{
    class Program
    {
        static void Main(string[] args)
        {
            var oktaConfig = OktaConfig.BuildConfig();

            var root = new RootCommand();
            var optionOfs = new Option("-OFS", "output field separator", new Argument<string>());
            root.AddOption(optionOfs);

            var optionAttrs = new Option("--attrs", "attributes to output", new Argument<string>());
            var optionInputFile = new Option("--input", "input file name", new Argument<FileInfo>());
            var optionGroupName = new Option("--grpName", "group name to run report for", new Argument<string>());
            var optionSearch = new Option("--search", "search expression", new Argument<string>());
            var optionFilter = new Option("--filter", "filter expression", new Argument<string>());

            var aCommand = new Command("findCreator",
                handler: CommandHandler.Create<FileInfo, string, string>(
                    async (input, attrs, ofs) => { await new CreatorReport(oktaConfig, input, attrs, ofs).Run(); }));

            aCommand.AddOption(optionInputFile);
            aCommand.AddOption(optionAttrs);
            aCommand.AddOption(optionOfs);
            root.AddCommand(aCommand);

            var bCommand = new Command("setAttribute", handler: CommandHandler.Create<string, FileInfo, string, bool>(
                async (attrName, input, attrValue, writeEmpty) =>
                {
                    await new AttributeSetter(oktaConfig, input, attrName, attrValue, writeEmpty).Run();
                }));

            bCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            bCommand.AddOption(new Option("--attrValue", "profile attribute value to be set", new Argument<string>()));
            bCommand.AddOption(optionInputFile);
            bCommand.AddOption(optionOfs);
            bCommand.AddOption(new Option("--writeEmpty", "skip empty values", new Argument<bool>()));
            root.AddCommand(bCommand);

            var cCommand = new Command("emptyAttribute", handler: CommandHandler.Create<string, string>(
                (attrName, since) => { new EmptyAttributeReport(oktaConfig, attrName, since).Run().Wait(); }));

            cCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            cCommand.AddOption(new Option("--since", "select users created since specified date (YYYY-MM-DD format)",
                new Argument<string>()));
            cCommand.AddOption(optionOfs);
            root.AddCommand(cCommand);

            var groupMembershipWithFilter = new Command("groupMembership",
                handler: CommandHandler.Create<string, string, string, string>(
                    (grpName, filter, attrs, ofs) =>
                        new GroupMembersReportWithUserFilter(oktaConfig, grpName, filter, attrs, ofs)
                            .Run()));
            groupMembershipWithFilter.AddOption(optionGroupName);
            groupMembershipWithFilter.AddOption(optionFilter);
            groupMembershipWithFilter.AddOption(optionAttrs);
            groupMembershipWithFilter.AddOption(optionOfs);
            root.AddCommand(groupMembershipWithFilter);

            var eCommand = new Command("listApps",
                handler: CommandHandler.Create<string>(async ofs =>
                {
                    await new ApplicationList(oktaConfig, ofs).Run();
                }));
            eCommand.AddOption(optionOfs);
            root.AddCommand(eCommand);

            var appAssignmentCmd = new Command("appUser",
                handler: CommandHandler.Create<string, FileInfo, string>(async (appLabel, input, ofs) =>
                {
                    await new AppUserReport(oktaConfig, appLabel, input, ofs).Run();
                }));
            appAssignmentCmd.AddOption(optionOfs);
            appAssignmentCmd.AddOption(new Option("--appLabel", "application label", new Argument<string>()));
            appAssignmentCmd.AddOption(optionInputFile);
            root.AddCommand(appAssignmentCmd);

            var listGroups = new Command("listGroups",
                handler: CommandHandler.Create<string>((ofs) => { new GroupList(oktaConfig, ofs).Run(); }));
            listGroups.AddOption(optionOfs);
            root.AddCommand(listGroups);

            var fCommand = new Command("userReport",
                handler: CommandHandler.Create<FileInfo, string, string>(async (input, attrName, attrs) =>
                {
                    await new UserReport(oktaConfig, input, attrName, attrs).Run();
                }));
            fCommand.AddOption(optionInputFile);
            fCommand.AddOption(optionAttrs);
            fCommand.AddOption(new Option("--attrName", "attribute name to check", new Argument<string>()));
            fCommand.AddOption(optionOfs);
            root.AddCommand(fCommand);

            var gCommand = new Command("userSearchReport",
                handler: CommandHandler.Create<string, string, string, string>(async (search, filter, attrs, ofs) =>
                {
                    await new UserSearchReport(oktaConfig, search, filter, attrs, ofs).Run();
                }));
            gCommand.AddOption(optionSearch);
            gCommand.AddOption(optionFilter);
            gCommand.AddOption(optionAttrs);
            gCommand.AddOption(root.FirstOrDefault(sym => sym.Name == "OFS") as Option);
            root.AddCommand(gCommand);

            var activateUsers = new Command("activateUsers",
                handler: CommandHandler.Create<FileInfo>(input => new ActivateUsers(oktaConfig, input).Run()));
            activateUsers.AddOption(optionInputFile);
            root.AddCommand(activateUsers);

            var userLifeCycle = new Command("userLifecycle",
                handler: CommandHandler.Create<FileInfo, string>(async (input, action) =>
                    await new UserLifecycle(oktaConfig, input, action).Run()));
            userLifeCycle.AddOption(optionOfs);
            userLifeCycle.AddOption(optionInputFile);
            userLifeCycle.AddOption(new Option("--action", "activate, deactivate or delete", new Argument<string>()));
            root.AddCommand(userLifeCycle);

            var manageGroups = new Command("manageGroups",
                handler: CommandHandler.Create<FileInfo, string>(async (input, action) =>
                {
                    await new ManageGroups(oktaConfig, input, action).Run();
                }));
            manageGroups.AddOption(optionInputFile);
            manageGroups.AddOption(optionOfs);
            manageGroups.AddOption(new Option("--action", "[add | remove | display]", new Argument<string>()));
            root.AddCommand(manageGroups);

            var injectedArgs = args.All(s => s != "-OFS")
                ? args.Concat(new[] {"-OFS", ","}).ToArray()
                : args;

            root.InvokeAsync(injectedArgs).Wait();
        }

        public static IEnumerable<string> ReadConsoleLines()
        {
            string s;
            while ((s = Console.ReadLine()) != null)
                yield return s;
        }
    }
}