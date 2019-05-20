using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.DragonFruit;
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

            root.AddCommand(new Command("usedSourceType",
                handler: CommandHandler.Create(() => { new UsedSourceTypeReport(oktaConfig).Run(); })));


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


            var cCommand = new Command("emptyAttribute", handler: CommandHandler.Create<string>(
                (attrName) => { new EmptyAttributeReport(oktaConfig, attrName).Run(); }));

            cCommand.AddOption(new Option("--attrName", "profile attribute name to populate", new Argument<string>()));
            root.AddCommand(cCommand);

            var dCommand = new Command("groupMembership", handler: CommandHandler.Create<string>(
                (grpName) => { new GroupMembersReport(oktaConfig, grpName).Run(); }));

            dCommand.AddOption(new Option("--grpName", "group name to run report for", new Argument<string>()));
            root.AddCommand(dCommand);
            
            root.InvokeAsync(args).Wait();
        }
    }
}