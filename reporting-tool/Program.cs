using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace reporting_tool
{
    class Program
    {
        static void Main(string[] args)
        {
            var oktaConfig = OktaConfig.BuildConfig();

            var root = new RootCommand();

            var optionAttrs = new Option("--attrs", "attributes to output", new Argument<string>());
            var optionInputFile = new Option("--input", "input file name", new Argument<FileInfo>());
            var optionGroupName = new Option("--grpName", "group name to run report for", new Argument<string>());
            var optionSearch = new Option("--search", "search expression", new Argument<string>());
            var optionFilter = new Option("--filter", "filter expression", new Argument<string>());

            var command = new Command("usedAttributeValues",
                handler: CommandHandler.Create<string>((attrName) =>
                {
                    new UsedAttributeValues(oktaConfig, attrName).Run();
                }));
            command.AddOption(new Option("--attrName", "profile attribute name to check", new Argument<string>()));
            root.AddCommand(command);

            var aCommand = new Command("findCreator",
                handler: CommandHandler.Create<FileInfo>(input => { new CreatorReport(oktaConfig, input).Run(); }));

            aCommand.AddOption(optionInputFile);
            root.AddCommand(aCommand);

            var bCommand = new Command("setAttribute", handler: CommandHandler.Create<string, FileInfo>(
                (attrName, input) => { new AttributeSetter(oktaConfig, input, attrName).Run(); }));

            bCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            bCommand.AddOption(optionInputFile);
            root.AddCommand(bCommand);


            var cCommand = new Command("emptyAttribute", handler: CommandHandler.Create<string, string>(
                (attrName, since) => { new EmptyAttributeReport(oktaConfig, attrName, since).Run(); }));

            cCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            cCommand.AddOption(new Option("--since", "select users created since specified date (YYYY-MM-DD format)",
                new Argument<string>()));
            root.AddCommand(cCommand);

            var dCommand = new Command("groupMembership", handler: CommandHandler.Create<string>(
                (grpName) => { new GroupMembersReport(oktaConfig, grpName).Run(); }));

            dCommand.AddOption(optionGroupName);
            root.AddCommand(dCommand);

            var groupMembershipWithFilter = new Command("groupMembershipWithFilter",
                handler: CommandHandler.Create<string, string, string>(
                    (grpName, filter, attrs) =>
                        new GroupMembersReportWithUserFilter(oktaConfig, grpName, filter, attrs)
                            .Run()));
            groupMembershipWithFilter.AddOption(optionGroupName);
            groupMembershipWithFilter.AddOption(optionFilter);
            groupMembershipWithFilter.AddOption(optionAttrs);
            root.AddCommand(groupMembershipWithFilter);

            var eCommand = new Command("listApps",
                handler: CommandHandler.Create(() => { new ApplicationList(oktaConfig).Run(); }));
            root.AddCommand(eCommand);

            root.AddCommand(new Command("listGroups",
                handler: CommandHandler.Create(() => { new GroupList(oktaConfig).Run(); })));

            var fCommand = new Command("userReport",
                handler: CommandHandler.Create<FileInfo, string>((input, attrs) =>
                {
                    new UserReport(oktaConfig, input, attrs).Run();
                }));
            fCommand.AddOption(optionInputFile);
            fCommand.AddOption(optionAttrs);
            root.AddCommand(fCommand);

            var gCommand = new Command("userSearchReport",
                handler: CommandHandler.Create<string, string, string>((search, filter, attrs) =>
                {
                    new UserSearchReport(oktaConfig, search, filter, attrs).Run();
                }));
            gCommand.AddOption(optionSearch);
            gCommand.AddOption(optionFilter);
            gCommand.AddOption(optionAttrs);
            root.AddCommand(gCommand);

            var activateUsers = new Command("activateUsers",
                handler: CommandHandler.Create<FileInfo>((input) => new ActivateUsers(oktaConfig, input).Run()));
            activateUsers.AddOption(optionInputFile);
            root.AddCommand(activateUsers);

            root.InvokeAsync(args).Wait();
        }
        public static IEnumerable<string> ReadConsoleLines()
        {
            string s;
            while ((s = Console.ReadLine()) != null)
                yield return s;
        }
    }
}