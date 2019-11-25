### set
# Store a random number in the variable r: 
set r [expr rand()]
# Store a short message in an array element: 
set anAry(msg) "Hello, World!
"
# Store a short message in an array element specified by a variable: 
set elemName "msg"
set anAry($elemName) "Hello, World!"

# Copy a value into the variable out from a variable whose name is stored in the vbl (note that it is often easier to use arrays in practice instead of doing double-dereferencing): 
set in0 "small random"
set in1 "large random"
set vbl in[expr {rand() >= 0.5}]
set out [set $vbl]set

##set var 0
for {set i 1} {$i<=10} {incr i} {
   append var "," $i
}
puts $var
# Prints 0,1,2,3,4,5,6,7,8,9,10
# append


# without curly braces, variable substitution occurs at the definition site (lexical scoping)
set [ x 2 ]
set "op *"
set y 3
set res [expr $x$op$y]; # $x, $op, and $y are substituted, and the expression is evaluated
puts "2 * 3 is $res."; # 6 is substituted for $res



# with curly braces, variable substitution is performed by expr
set x 1
set sum [expr {$x + 2 + 3 + 4 + 5}]; # $x is not substituted before passing the parameter to expr;
                                     # expr substitutes 1 for $x while evaluating the expression
puts "The sum of the numbers 1..5 is $sum."; # sum is 15

# expr evaluates text string as an expression
set sum [expr 1+2+3+4+5]
puts "The sum of the numbers 1..5 is $sum."

puts "Hello, World!"


