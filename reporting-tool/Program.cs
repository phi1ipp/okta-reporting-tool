using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                handler: CommandHandler.Create<FileInfo, string>((input, attrs) =>
                {
                    new CreatorReport(oktaConfig, input, attrs).Run();
                }));

            aCommand.AddOption(optionInputFile);
            aCommand.AddOption(optionAttrs);
            root.AddCommand(aCommand);

            var bCommand = new Command("setAttribute", handler: CommandHandler.Create<string, FileInfo, string>(
                (attrName, input, attrValue) =>
                {
                    new AttributeSetter(oktaConfig, input, attrName, attrValue).Run();
                }));

            bCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            bCommand.AddOption(new Option("--attrValue", "profile attribute value to be set", new Argument<string>()));
            bCommand.AddOption(optionInputFile);
            root.AddCommand(bCommand);

            var cCommand = new Command("emptyAttribute", handler: CommandHandler.Create<string, string>(
                (attrName, since) => { new EmptyAttributeReport(oktaConfig, attrName, since).Run(); }));

            cCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            cCommand.AddOption(new Option("--since", "select users created since specified date (YYYY-MM-DD format)",
                new Argument<string>()));
            root.AddCommand(cCommand);

            var groupMembershipWithFilter = new Command("groupMembership",
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
                handler: CommandHandler.Create<FileInfo, string, string>((input, attrName, attrs) =>
                {
                    new UserReport(oktaConfig, input, attrName, attrs).Run();
                }));
            fCommand.AddOption(optionInputFile);
            fCommand.AddOption(optionAttrs);
            fCommand.AddOption(new Option("--attrName", "attribute name to check", new Argument<string>()));
            root.AddCommand(fCommand);

            var gCommand = new Command("userSearchReport",
                handler: CommandHandler.Create<string, string, string, string>((search, filter, attrs, ofs) =>
                {
                    new UserSearchReport(oktaConfig, search, filter, attrs, ofs).Run();
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

            var manageGroups = new Command("manageGroups",
                handler: CommandHandler.Create<FileInfo, string>((input, action) =>
                {
                    new ManageGroups(oktaConfig, input, action).Run();
                }));
            manageGroups.AddOption(optionInputFile);
            manageGroups.AddOption(new Option("--action", "[add | remove | display]", new Argument<string>()));
            root.AddCommand(manageGroups);

            var injectedArgs = args.All(s => s != "-OFS")
                ? args.Union(new[] {"-OFS", ","}).ToArray()
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