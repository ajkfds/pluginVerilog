`timescale 1n/1p

module EXPRESSIONS;

task concatination;
reg	[7:0]	e0;
reg	[7:0]	e1;
reg	[8:0]	e2;
begin
	e2 = e0 + e1;
end
endtask


/*
Variable declarations

integer_declaration		::= integer list_of_variable_identifiers ;
reg_declaration			::= reg [ signed ] [ range ] list_of_variable_identifiers ;
time_declaration		::= time list_of_variable_identifiers ;

list_of_variable_identifiers	::= variable_type { , variable_type } 
variable_type			::=  variable_identifier [ = constant_expression ] 
							| variable_identifier dimension { dimension }

real_declaration		::= real list_of_real_identifiers ;
realtime_declaration	::= realtime list_of_real_identifiers ;

list_of_real_identifiers		::= real_type { , real_type }  
real_type				::= real_identifier [ = constant_expression ]
							| real_identifier dimension { dimension }

dimension ::= [ dimension_constant_expression : dimension_constant_expression ]  
range ::= [ msb_constant_expression : lsb_constant_expression ]
*/

integer	int0;
integer	int1,	int2[3:0],	int3 = 1;

reg reg0;
reg signed	reg4;
reg [7:0]	reg5,	reg6[3:0],	reg7 = 8'h00;

time	time0;
time	time1,	time2[3:0],	time3 = 1;

real	real0;
real	real1,	real2 = 0,	real3[4:0];

realtime	realtime0;
realtime	realtime1,	realtime2 = 0,	realtime3[4:0];

// 4.1.6 Arithmetic expressions with regs and integers
task arithmeticExpressionsWithRegsAndIntegers;
integer intA;
reg [15:0] regA;
reg signed [15:0] regS;
begin
	intA = -4'd12;
	regA = intA / 3;	// expression result is -4, 
						// intA is an integer data type, regA is 65532
	
	regA = -4'd12;		// regA is 65524
	intA = regA / 3; 	// expression result is 21841,
						// regA is a reg data type
	intA = -4'd12 / 3;	// expression result is 1431655761. 
						// -4’d12 is effectively a 32-bit reg data type
	regA = -12 / 3;		// expression result is -4, -12 is effectively
						// an integer data type. regA is 65532
	regS = -12 / 3;		// expression result is -4. regS is a signed // reg 
	regS = -4'sd12 / 3;	// expression result is 1. -4’sd12 is actually
						// 4. The rules for integer division yield 4/3==1
end
endtask

task operators;
integer		a;
integer		b;
integer		c;
begin
	a = 1;
	b = 1;
	c = a + b;
	c = a - b;
	c = a * b;
	c = a / b;
	c = a ** b;
end
endtask

wire [7:0]	e0;
wire [7:0]	e1;
wire [7:0]	e2;
assign	e2	= {e0[0],e0[7:1]};

`define DEF1
`define DEF0

`ifdef DEF1
	reg def00;
`else
	`ifdef DEF0
		reg def01;
	`else
		reg def03;
	`endif
	reg def01;
`endif

endmodule