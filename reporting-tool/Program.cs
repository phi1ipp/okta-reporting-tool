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
            var optionAll = new Option("--all", "all records", new Argument<bool>());

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
                async (attrName, since) => { await new EmptyAttributeReport(oktaConfig, attrName, since).Run(); }));

            cCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            cCommand.AddOption(new Option("--since", "select users created since specified date (YYYY-MM-DD format)",
                new Argument<string>()));
            cCommand.AddOption(optionOfs);
            root.AddCommand(cCommand);

            var groupMembershipWithFilter = new Command("groupMembership",
                handler: CommandHandler.Create<string, string, string, FileInfo, string>(
                    (grpName, filter, attrs, input, ofs) =>
                        new GroupMembersReportWithUserFilter(oktaConfig, grpName, filter, attrs, input, ofs)
                            .Run()));
            groupMembershipWithFilter.AddOption(optionGroupName);
            groupMembershipWithFilter.AddOption(optionFilter);
            groupMembershipWithFilter.AddOption(optionAttrs);
            groupMembershipWithFilter.AddOption(optionOfs);
            groupMembershipWithFilter.AddOption(optionInputFile);
            root.AddCommand(groupMembershipWithFilter);

            var eCommand = new Command("listApps",
                handler: CommandHandler.Create<string>(async ofs =>
                {
                    await new ApplicationList(oktaConfig, ofs).Run();
                }));
            eCommand.AddOption(optionOfs);
            root.AddCommand(eCommand);

            var appAssignmentCmd = new Command("appUser",
                handler: CommandHandler.Create<string, string, FileInfo, bool, string>(async (appLabel, attrs, input, all, ofs) =>
                {
                    await new AppUserReport(oktaConfig, appLabel, attrs, input, all, ofs).Run();
                }));
            appAssignmentCmd.AddOption(optionOfs);
            appAssignmentCmd.AddOption(new Option("--appLabel", "application label", new Argument<string>()));
            appAssignmentCmd.AddOption(optionAttrs);
            appAssignmentCmd.AddOption(optionInputFile);
            appAssignmentCmd.AddOption(optionAll);
            root.AddCommand(appAssignmentCmd);

            var appLifecycleCmd = new Command("appUserLifecycle",
                handler: CommandHandler.Create<string, FileInfo, string, string, string>(async (appLabel, input,  action, attrs, ofs) =>
                {
                    await new AppUserLifecycle(oktaConfig, appLabel, input, action, attrs, ofs).Run();
                }));
            appLifecycleCmd.AddOption(optionOfs);
            appLifecycleCmd.AddOption(new Option("--appLabel", "application label", new Argument<string>()));
            appLifecycleCmd.AddOption(new Option("--action", "application label", new Argument<string>()));
            appLifecycleCmd.AddOption(optionAttrs);
            appLifecycleCmd.AddOption(optionInputFile);
            root.AddCommand(appLifecycleCmd);

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

            var userLifeCycle = new Command("userLifecycle",
                handler: CommandHandler.Create<FileInfo, string>(async (input, action) =>
                    await new UserLifecycle(oktaConfig, input, action).Run()));
            userLifeCycle.AddOption(optionOfs);
            userLifeCycle.AddOption(optionInputFile);
            userLifeCycle.AddOption(new Option("--action", "activate, deactivate, suspend, unsuspend or delete", new Argument<string>()));
            root.AddCommand(userLifeCycle);

            var manageGroups = new Command("manageMembership",
                handler: CommandHandler.Create<FileInfo, string, string, bool>(async (input, action, grpName, idUsed) =>
                {
                    await new ManageMembership(oktaConfig, input, action, grpName, idUsed).Run();
                }));
            manageGroups.AddOption(optionInputFile);
            manageGroups.AddOption(optionOfs);
            manageGroups.AddOption(optionGroupName);
            manageGroups.AddOption(new Option("--action", "[add | remove | display]", new Argument<string>()));
            manageGroups.AddOption(new Option("--idUsed", "true if group id used instead of name", new Argument<bool>()));
            root.AddCommand(manageGroups);

            var groupRename = new Command("groupRename",
                handler: CommandHandler.Create<FileInfo, bool>(async (input, idUsed) =>
                {
                    await new GroupRename(oktaConfig, input, idUsed).Run();
                }));
            groupRename.AddOption(optionInputFile);
            groupRename.AddOption(new Option("--idUsed", "true if group id used instead of name", new Argument<bool>()));
            groupRename.AddOption(optionOfs);
            root.AddCommand(groupRename);
            
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