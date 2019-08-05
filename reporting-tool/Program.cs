using System;
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
            
            var command = new Command("usedAttributeValues",
                handler: CommandHandler.Create<string>((attrName) =>
                {
                    new UsedAttributeValues(oktaConfig, attrName).Run();
                }));
            command.AddOption(new Option("--attrName", "profile attribute name to check", new Argument<string>()));
            root.AddCommand(command);

            var aCommand = new Command("findCreator",
                handler: CommandHandler.Create<FileInfo>((input) => { new CreatorReport(oktaConfig, input).Run(); }));

            aCommand.AddOption(new Option("--input", "input file name", new Argument<FileInfo>()));
            root.AddCommand(aCommand);

            var bCommand = new Command("setAttribute", handler: CommandHandler.Create<string, FileInfo>(
                (attrName, input) => { new AttributeSetter(oktaConfig, input, attrName).Run(); }));

            bCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            bCommand.AddOption(new Option("--input", "file name with the following structure <guid> <attr value>",
                new Argument<FileInfo>()));
            root.AddCommand(bCommand);


            var cCommand = new Command("emptyAttribute", handler: CommandHandler.Create<string, string>(
                (attrName, since) => { new EmptyAttributeReport(oktaConfig, attrName, since).Run(); }));

            cCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            cCommand.AddOption(new Option("--since", "select users created since specified date (YYYY-MM-DD format)",
                new Argument<string>()));
            root.AddCommand(cCommand);

            var dCommand = new Command("groupMembership", handler: CommandHandler.Create<string>(
                (grpName) => { new GroupMembersReport(oktaConfig, grpName).Run(); }));

            dCommand.AddOption(new Option("--grpName", "group name to run report for", new Argument<string>()));
            root.AddCommand(dCommand);

            var eCommand = new Command("listApps",
                handler: CommandHandler.Create(() => { new ApplicationList(oktaConfig).Run(); }));
            root.AddCommand(eCommand);

            root.AddCommand(new Command("listGroups",
                handler: CommandHandler.Create(() => { new GroupList(oktaConfig).Run(); })));
            
            var fCommand = new Command("userReport", handler: CommandHandler.Create<FileInfo, string>((input, attrs) =>
            {
                new UserReport(oktaConfig, input, attrs).Run();
            }));
            fCommand.AddOption(optionInputFile);
            fCommand.AddOption(optionAttrs);
            root.AddCommand(fCommand);
            
            var gCommand = new Command("userSearchReport", handler: CommandHandler.Create<string, string>((search, attrs) =>
            {
                new UserSearchReport(oktaConfig, search, attrs).Run();
            }));
            gCommand.AddOption(new Option("--search", "search expression", new Argument<string>()));
            gCommand.AddOption(optionAttrs);
            root.AddCommand(gCommand);
            
            root.InvokeAsync(args).Wait();
        }
    }
}