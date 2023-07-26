using System;
using System.Collections.Generic;
using System.CommandLine;
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
            var optionOfs = new Option<string>("-OFS", "output field separator");
            root.AddOption(optionOfs);

            var optionAttrs = new Option<string>("--attrs", "attributes to output");
            var optionFilter = new Option<string>("--filter", "filter expression");
            var optionGroupName = new Option<string>("--grpName", "group name to run report for");
            var optionInputFile = new Option<FileInfo>("--input", "input file name");
            var optionIdUsed = new Option<bool>("--idUsed", "true if group id used instead of name");
            var optionSearch = new Option<string>("--search", "search expression");
            var optionAll = new Option<bool>("--all", "all records");

            var aCommand = new Command("findCreator");
            aCommand.SetHandler(
                    async (input, attrs, ofs) => { 
                        await new CreatorReport(oktaConfig, input, attrs, ofs).Run(); 
                    }, optionInputFile, optionAttrs, optionOfs);

            aCommand.AddOption(optionInputFile);
            aCommand.AddOption(optionAttrs);
            aCommand.AddOption(optionOfs);
            root.AddCommand(aCommand);

            // setAttribute command
            var optionAttrName = new Option<string>("--attrName", "profile attribute name to populate");
            var optionAttrValue = new Option<string>("--attrValue", "profile attribute value to be set");
            var optionWriteEmpty = new Option<bool>("--writeEmpty", "skip empty values");

            var bCommand = new Command("setAttribute");

            bCommand.SetHandler(
                async ( input, attrName, attrValue, writeEmpty) =>
                {
                    await new AttributeSetter(oktaConfig, input, attrName, attrValue, writeEmpty).Run();
                }, optionInputFile, optionAttrName, optionAttrValue, optionWriteEmpty);

            bCommand.AddOption(optionAttrName);
            bCommand.AddOption(optionAttrValue);
            bCommand.AddOption(optionInputFile);
            bCommand.AddOption(optionOfs);
            bCommand.AddOption(optionWriteEmpty);
            root.AddCommand(bCommand);

            // emptyAttribute
            var optionSince = new Option<string>("--since", "select users created since specified date (YYYY-MM-DD format)");
            var cCommand = new Command("emptyAttribute");
            cCommand.SetHandler(
                async (attrName, since) => { 
                    await new EmptyAttributeReport(oktaConfig, attrName, since).Run(); 
                }, optionAttrName, optionSince);

            cCommand.AddOption(optionAttrName);
            cCommand.AddOption(optionSince);
            cCommand.AddOption(optionOfs);
            root.AddCommand(cCommand);

            // groupMembership
            var groupMembershipWithFilter = new Command("groupMembership");
            groupMembershipWithFilter.SetHandler(
                    (grpName, filter, attrs, input, ofs) =>
                        new GroupMembersReportWithUserFilter(oktaConfig, grpName, filter, attrs, input, ofs).Run(), 
                        optionGroupName, optionFilter, optionAttrs, optionInputFile, optionOfs);
            groupMembershipWithFilter.AddOption(optionGroupName);
            groupMembershipWithFilter.AddOption(optionFilter);
            groupMembershipWithFilter.AddOption(optionAttrs);
            groupMembershipWithFilter.AddOption(optionOfs);
            groupMembershipWithFilter.AddOption(optionInputFile);
            root.AddCommand(groupMembershipWithFilter);

            // listApps
            var eCommand = new Command("listApps");
            eCommand.SetHandler(
                async (ofs) =>
                {
                    await new ApplicationList(oktaConfig, ofs).Run();
                }, optionOfs);
            eCommand.AddOption(optionOfs);
            root.AddCommand(eCommand);

            // appUser
            var optionAppLabel = new Option<string>("--appLabel", "application label");
            var appAssignmentCmd = new Command("appUser");
            appAssignmentCmd.SetHandler(
                    async (appLabel, attrs, input, all, ofs) =>
                    {
                        await new AppUserReport(oktaConfig, appLabel, attrs, input, all, ofs).Run();
                    }, optionAppLabel, optionAttrs, optionInputFile, optionAll, optionOfs);
            appAssignmentCmd.AddOption(optionOfs);
            appAssignmentCmd.AddOption(optionAppLabel);
            appAssignmentCmd.AddOption(optionAttrs);
            appAssignmentCmd.AddOption(optionInputFile);
            appAssignmentCmd.AddOption(optionAll);
            root.AddCommand(appAssignmentCmd);

            // appUserLifecycle
            var optionAction = new Option<string>("--action", "action");
            var appLifecycleCmd = new Command("appUserLifecycle");
            appLifecycleCmd.SetHandler(
                    async (appLabel, input, action, attrs, all, ofs) =>
                    {
                        await new AppUserLifecycle(oktaConfig, appLabel, input, action, attrs, all, ofs).Run();
                    }, optionAppLabel, optionInputFile, optionAction, optionAttrs, optionAll, optionOfs);
            appLifecycleCmd.AddOption(optionOfs);
            appLifecycleCmd.AddOption(optionAppLabel);
            appLifecycleCmd.AddOption(optionAction);
            appLifecycleCmd.AddOption(optionAttrs);
            appLifecycleCmd.AddOption(optionInputFile);
            appLifecycleCmd.AddOption(optionAll);
            root.AddCommand(appLifecycleCmd);

            // listGroups
            var listGroups = new Command("listGroups");
            listGroups.SetHandler(
                async (ofs) => { 
                    await new GroupList(oktaConfig, ofs).Run(); 
                }, optionOfs);
            listGroups.AddOption(optionOfs);
            root.AddCommand(listGroups);

            // createGroups
            var createGroups = new Command("createGroups");
            createGroups.SetHandler(
                async (input, ofs) => { 
                    await new GroupCreate(oktaConfig, input, ofs).Run(); 
                }, optionInputFile, optionOfs);
            createGroups.AddOption(optionInputFile);
            createGroups.AddOption(optionOfs);
            root.AddCommand(createGroups);

            // deleteGroups
            var deleteGroups = new Command("deleteGroups");
            deleteGroups.SetHandler(
                async (input, useIds) => { 
                    await new GroupDelete(oktaConfig, input, useIds).Run(); 
                }, optionInputFile, optionIdUsed);
            deleteGroups.AddOption(optionInputFile);
            deleteGroups.AddOption(optionIdUsed);
            deleteGroups.AddOption(optionOfs);
            root.AddCommand(deleteGroups);

            // userReport
            var optionAttrNameToCheck = new Option<string>("--attrName", "attribute name to check");
            var fCommand = new Command("userReport");
            fCommand.SetHandler(
                async (input, attrName, attrs) =>
                {
                    await new UserReport(oktaConfig, input, attrName, attrs).Run();
                }, optionInputFile, optionAttrNameToCheck, optionAttrs);
            fCommand.AddOption(optionInputFile);
            fCommand.AddOption(optionAttrs);
            fCommand.AddOption(optionAttrNameToCheck);
            fCommand.AddOption(optionOfs);
            root.AddCommand(fCommand);

            // userSearchReport
            var gCommand = new Command("userSearchReport");
            gCommand.SetHandler(
                async (search, filter, attrs, ofs) =>
                {
                    await new UserSearchReport(oktaConfig, search, filter, attrs, ofs).Run();
                }, optionSearch, optionFilter, optionAttrs, optionOfs);
            gCommand.AddOption(optionSearch);
            gCommand.AddOption(optionFilter);
            gCommand.AddOption(optionAttrs);
            gCommand.AddOption(optionOfs);
            root.AddCommand(gCommand);

            // userLifecycle
            var optionUserLifecycleAction = new Option<string>("--action", "activate, deactivate, suspend, unsuspend or delete");
            var userLifeCycle = new Command("userLifecycle");
            userLifeCycle.SetHandler(
                async (input, action) =>
                    await new UserLifecycle(oktaConfig, input, action).Run(),
                optionInputFile, optionUserLifecycleAction);
            userLifeCycle.AddOption(optionOfs);
            userLifeCycle.AddOption(optionInputFile);
            userLifeCycle.AddOption(optionUserLifecycleAction);
            root.AddCommand(userLifeCycle);

            // manageMembership
            var optionManageMembershipAction = new Option<string>("--action", "[add | remove | display]");
            var manageGroups = new Command("manageMembership");
            manageGroups.SetHandler(
                async (input, action, grpName, idUsed) =>
                {
                    await new ManageMembership(oktaConfig, input, action, grpName, idUsed).Run();
                }, optionInputFile, optionManageMembershipAction, optionGroupName, optionIdUsed);
            manageGroups.AddOption(optionInputFile);
            manageGroups.AddOption(optionOfs);
            manageGroups.AddOption(optionGroupName);
            manageGroups.AddOption(optionManageMembershipAction);
            manageGroups.AddOption(optionIdUsed);
            root.AddCommand(manageGroups);

            // groupRename
            var groupRename = new Command("groupRename");
            groupRename.SetHandler(
                async (input, idUsed) =>
                {
                    await new GroupRename(oktaConfig, input, idUsed).Run();
                }, optionInputFile, optionIdUsed);
            groupRename.AddOption(optionInputFile);
            groupRename.AddOption(optionIdUsed);
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