# okta-reporting-tool
It's a command line tool to help with Okta data manipulation activities. If your organization doesn't have/want to use LDAP interface for these purposes, you can harness this tool. 

**Currently the repository doesn't have all the components/steps required to build the tool (modified Okta Sdk to throttle request frequency)**
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

Also group name attribute can be included in the output for a user with `grp.Name` in case you want to get information about 
user group membership, like `--search "not profile.SourceType pr" --attrs login,grp.Name`

### Conditions
The tool supports conditional expressions based on a built-in grammatic. 
```
grammar BoolExpr;

expr : '(' expr')'      #parenthesisExp 
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
-- input <file_name> 
-- attrs <csv_of_creator_attributes>
```

### userSearchReport
This report can be used when you have a condition according to which users should be pull out of Okta. Usually switch `--search` is preferable 
over `--filter`, because the former one makes search happening on Okta's side (by Okta engine), while the latter one performs filtering 
on a client side. Due to Okta API limitation, you can't use `--search` if your result exceeds 50,000 entries. You will be forced to use 
`--filter` in this case.

It accepts the following switches:
```
-- input <file_name> 
-- filter <filter_expression>
-- search <filter_expression>
-- attrs <csv_of_user_attributes>
```

