grammar BoolExpr;

expr : '(' expr')'      #parenthesisExp 
    | 'not' expr        #notExp
    | expr 'and' expr   #andExp
    | expr 'or' expr    #orExp
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