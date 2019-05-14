# harv-code-compiler
Compiling HarvCode, an original and simplistic programming language (somewhere between assembly and python), to run in Python and C#

As of V0.1, all variables and methods are global...

## Syntax
1. All commands are as follows: "command a b ... c"
2. Strictly one command per line
3. 'Blocked' methods such as 'if' and 'def' terminate with "end"
4. method bodies should be indented for readability, you monster.

## Commands
### set
- set val = 10
- set val_1 = val_2

### print
- print 10
- print val
- print "Hello World"

## if
- if a > b <br> print "a greater than b" <br> end
- if a != b <br>	print "a not equal to b" <br> end

## comment
- // You can type anything here.

## calc
- calc val = 2 * 5
- calc c = a + b

## def
- def increment <br> calc val = val + 1 <br> end

## call
- call increment

## run
- run "./file_name.hc"

## check_exists
- check_exists check_val variable
- if check_val == 1 <br> print "variable exists!" <br> end

## exit
- exit
