# ToluPL Documentation
```
fn int HELLO(string word){
  out(word;);
}
string Hystring = "Hello! 😃";
HELLO(Hystring;);
```
\> `Hello! 😃`
##
ToluPL is an experimental programming language, whose focus is to allow fast implementations with simple grammar.
These docs will cover the basics of Tolu's syntax and methods.

### Operations
Tolu's operators allow the user to perform all the basic binary operations.
Current implemented operators:
| Operator | Operation |
| :---         |     :---:      |
|+ - * / **|Add, subtract, multiply, divide, raise to power|
|%|Modulo operand|
|{ [ (|Open Parenthesis|
|) ] }|Close Parenthesis|
|=|Assign|
|==|Equals|
|!=|Not equals|
|<, >|Less than, more than|
|<=, >=|Less or equal to, more or equal to|
|!|Negates|

## Variable declaration
Tolu supports a type-based variable assignment.
Current supported types: int, float, long, double, string, bool, list.

Declaration syntax: 
```
TYPE NAME = VALUE;
//OR
TYPE NAME;
```

If no `VALUE` is given, the variable is given a default value of -999.
To change a variable's value after instantiation, use the following syntax.
```
NAME = VALUE;
```

### Variable declaration: Lists
To instantiate a list, the syntax follows the standard `TYPE NAME = VALUE;`
Unlike other programming languages, the end of the array is followed by a ";".
The syntax to change a pre-instantiated list value at a certain index is as follows.
```
list NEWLIST = [1,2,3;];
NEWLIST[0;] = 123;
out(NEWLIST;);
```
\> `[123,2,3]`

To instantiate a multi-dimensional array and to change its value:
```
list MULTILIST = [[1,2,3;],2,3;];
MULTILIST[0;0;] = 3;
out(MULTILIST;);
```
\> `[[3,2,3],2,3]`
