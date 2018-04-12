The language syntax for declaring variables, sets and rules is very human friendly

Lists are denoted by square brackets
List items are separated by a semi-colon (;).
Value parts are separated by a comma (,).

for variable declaration:
var <identifier> => min: <min_value>, max:<max_value>, labels: [<label1>; <label2>; ...; <labeln>]

for fuzzyset declaration:
set <identifier> => [<val1>; <val2>; <val3>; <val4>; edge: <right|left>; max: <max>; min: <min>] 
<val4> is optional
<valn> can be a floating point number or one of 'left' and 'right'

for rules
rule <identifier> => rule

The rule syntax is similar to the following
IF (Var1 IS label11 OR Var1 IS label12) AND Var2 is label 21 ... THEN Var3 IS label31

It can be as simple as 
IF RightDistance IS Near AND LeftDistance IS Medium THEN Angle IS LittleNegative

The declarations are stored in a file given by the name of the current context