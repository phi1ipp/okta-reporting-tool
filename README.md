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
 appUser
 appUserLifecycle
 listGroups
 userReport
 userSearchReport
 userLifecycle
 manageMembership
```
### Configuration
All the configuration is held in `appsettings.json` file, which includes the following section:
```
    ...,
    "Okta": {
        "Domain": "https://your_okta_domain_url",
        "RateLimiPreservationPct": 50,
        "ApiKey": "your okta api key"
    }
```
`RateLimitPreservationPct` represents a percentage of an Okta's endpoint [rate limit](https://developer.okta.com/docs/reference/rate-limits/) 
which has to be preserved while running the tool. 

### General comments
In general the tool can accept its input from a standart input, so that you can use piping from external utils as its input. 
Otherwise you can use file as an input source.

All commands accept parameter `-OFS` as an output fields separator (see `awk` utility).

### Attributes
The tool uses Okta user profile attribute names where applicable: 
* prefixed by `profile.` in conditional expressions, like `--filter "profile.LOA eq \"3\""`
* not prefixed in output attributes list, like `--attrs login,firstName,lastName,...`

Some non-profile attributes supported as well:
* id
* status
* created
* lastLogin

Also group name attribute can be included in the output for a user with `grp.Name` in case you want to get information about 
user group membership, like `--search "not profile.SourceType pr" --attrs login,grp.Name`

### Conditions
The tool supports conditional expressions based on a built-in grammatic. 
```
grammar BoolExpr;

expr : '(' expr ')'     #parenthesisExp 
    | expr 'and' expr   #andExp
    | expr 'or' expr    #orExp
    | 'not' expr        #notExp
    | attr_comp         #attrCompExp
    | attr_pr           #attrPrExp
    ;
    
attr_comp : attr 'eq' STR       #eqCompare
            | attr 'co' STR     #coCompare
            | attr 'sw' STR     #swCompare
            ;
            
attr_pr   : attr 'pr' ;
attr      : 'profile.' ATTR #profileAttr
            | ATTR          #nonProfileAttr
            ;    

ATTR: [a-zA-Z] [a-zA-Z_0-9]* ;
STR: '"' ~["]+ '"' ;
WS: [ \n] -> skip ;
```

### findCreator
In this mode the tool expects user GUID as its input and will find a user, who created a user with a given GUID, 
based on the information available in the system log. Please keep in mind, that Okta has a limited history, so if 
a user was created long time ago, you won't be able to find a creator. 

It accepts the following switches:
```
--input <file_name> 
--attrs <csv_of_creator_attributes>
```

### userReport
This report can be used when you have a list of user UUIDs/logins for whom you want to pull information out of Okta 
(one id per line). It can be provided either with `--input <file_name>` or through standard input.

It accepts the following switches:
```
--input  <file_name> 
--attrs  <csv_of_user_attributes>
```

### userSearchReport
This report can be used when you have a condition according to which users should be pull out of Okta. Usually switch `--search` is preferable 
over `--filter`, because the former one makes search happening on Okta's side (by Okta engine), while the latter one performs filtering 
on a client side. Due to Okta API limitation, you can't use `--search` if your result exceeds 50,000 entries. You will be forced to use 
`--filter` in this case.

It accepts the following switches:
```
--input  <file_name> 
--filter <filter_expression>
--search <filter_expression>
--attrs  <csv_of_user_attributes>
```

### setAttribute
This action is to set value(s) for a particular attribute(s) for given user(s). Each user can be assigned its own value, or all of the users 
can be assigned the same value. Input may come from a standard input or a file, which structure should match the following (you only provide attribute values in the input, field names will come from a command line parameters:
```
username1,attr1_value1[,"attr2_val2",...]
username2,attr1_value2[,attr2_val3,...]
```

Attribute can be wiped by providing "" for attribute value either in an input file or on a command line with a corresponding switch on a command line `--writeEmpty true`.

Examples:
* `./reporting-tool setAttribute --attrName LOA --attrValue "3" --input /tmp/loa` will set all users' LOA, whose usernames are listed in `/tmp/loa`, to "3"
* `./reporting-tool setAttribute --attrName LOA --input /tmp/loa` will set all users' LOA to value listed in the second column in `/tmp/loa`
* `./reporting-tool setAttribute --attrName firstName,lastName,LOA --input /tmp/values` will update several attributes for each user
from the input file `/tmp/values`, which format matches the one from the command description. Attribute and value order is obviously significant. If a value does not include space or comma, you can skip double quotes around it.
* `cut -f1 -d, /tmp/loa | ./reporting-tool setAttribute --attrName LOA --attrValue "3"` similar to the first example, just to demonstrate that the input 
may be a standard input

```
--input     <file_name>
--attrName  <user_profile_attribute_name>
--attrValue <value_to_assign_to_all_given_users> (optional)
--writeEmpty true (optional)
```

### manageMembership
This action is to manage/report user's group membership in Okta. To add/remove group membership the input has to be formatted the 
following way (group names can be substituted with group UUIDs, if `--idUsed true` switch is applied)
```
username1,"group 1","group 2"
username2,"group 2","group n"
```
so that a user is assigned/removed to/from the groups specified in the same line.

The action accepts the following switches, where `--input` can be skipped if standard input is used.
```
--input  <file_name>
--action [add | remove | display]
--grpName <group name(s) or UUIDs>
```
The last switch given on the command line allows to assign the list of users to the same group(s).

### listGroups
It's a basic report to extract all groups from Okta with their UUID and name

### listApps
It's a basic report to extract all applications from Okta with their UUID, name and display name

### appUser
This report allows to find if users are assigned to an application specified by `--appLabel` and display their application username. This report is obsolete and replaced fully by `appUserLifecycle`

```
--input     <file_name with user_ids>
--appLabel  <application label>
```
### appUserLifecycle
This action allows getting application user profile information, when `--action display` or removing/unassigning of application user profile with `--action delete`. It also allows to do a basic user to app assignment with `--action assign` w/o specifying profile attributes though

```
--appLabel  <application label>
--action    <display|delete|assign>
--attrs     <list of application user profile attributes to display>
--input     <filename for list of user IDs for which to do display/delete>
```
### userLifecycle
This action is to manage user lifecycle (activate, deactivate, delete) for all users given in the input file or standard input

```
--input  <file_name_with_usernames>
--action [activate,reactivate,activate_email,reactivate_email,deactivate,suspend,unsuspend,delete]
```
