# okta-reporting-tool
It's a command line tool to help with Okta data manipulation activities. If your organization doesn't have/want to use LDAP interface for these purposes, you can harness this tool. 

## Usage
```
$ ./reporting-tool.exe
Required command was not provided.
Usage:
 reporting-tool [options] [command]
Options:
 -OFS <OFS>    output field separator
 --version     Display version information
Commands:
 findCreator
 setAttribute
 emptyAttribute
 groupMembership
 listApps
 listGroups
 userReport
 userSearchReport
 activateUsers
 manageGroups
```
### findCreator
In this mode the tool expects user GUID as its input and will find a user, who created a user with a given GUID, based on the information available in the system log. Please keep in mind, that Okta has a limited history, so if a user was created long time ago, you won't be able to find a creator

It accepts the following switches:
* -- input <file_name> - full file path with a list of GUIDs to build the report for
* -- attrs <csv_of_creator_attributes> - attributes of a creator to output
