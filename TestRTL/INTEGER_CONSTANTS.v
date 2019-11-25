module INTEGER_CONSTANTS;


// Example 1-Unsized constant numbers
integer int0	= 659; // is a decimal number
integer int1	= 'h 837FF; // is a hexadecimal number
integer int2	= 'o7460; // is an octal number
//integer int3	= 4af; // is illegal (hexadecimal format requires ’h)


// Example 2-Sized constant numbers
integer int4	= 4'b1001; // is a 4-bit binary number
integer int5	= 5 'D 3; // is a 5-bit decimal number
integer int6	= 3'b01x; // is a 3-bit number with the least significant bit unknown
integer int7	= 12'hx; // is a 12-bit unknown number
integer int8	= 16'hz; // is a 16-bit high-impedance number

// Example 3-Using sign with constant numbers 
integer int9	= 8'd-6;  // this is illegal syntax
integer int10	= -8 'd 6;  // this defines the two’s complement of 6, held in 8 bits—equivalent to -(8’d 6)
integer int11	= 4 'shf;  // this denotes the 4-bit number ‘1111’, to be interpreted as a 2’s complement number, or ‘-1’. This is equivalent to -4’h 1
integer int12	= -4 'sd15;  // this is equivalent to -(-4’d 1), or ‘0001’.

// Example 4-Automatic left padding  
reg [11:0] a, b, c, d;
initial begin a = 'h x; // yields xxx
b = 'h 3x; // yields 03x 
c = 'h z3; // yields zz3 
d = 'h 0z3; // yields 0z3
end

reg [84:0] e, f, g; 
initial begin
e = 'h5;        // yields {82{1'b0},3'b101} 
f = 'hx;        // yields {85{1'hx}} 
g = 'hz;        // yields {85{1'hz}}
end

// Example 5-Using underscore character in numbers 
initial begin
27_195_000 
16’b0011_0101_0001_1111 
32 ’h 12ab_f001
end

endmodule
