// 19.compiler directive

module test0;

	`define wordsize 8
	reg [1:`wordsize] data;

endmodule

module test1;

localparam dly = 1;
//define a nand with variable delay
`define var_nand(dly) nand #dly 
//`var_nand(2) g121 (q21, n10, n11);
//`var_nand(5) g122 (q22, n10, n11);

`define first_half "start of string
//$display("first_half end of string");

`define max(a,b)((a) > (b) ? (a) : (b))
//n = max(p+q, r+s) ;

endmodule

// 19.4 `ifdef, `else, `elsif, `endif, `ifndef 


// example 1
module and_op(a, b, c);
output a;
input b, c;
`ifdef behavioral
	wire a = b & c; 
`else
	and a1 (a,b,c); 
`endif
endmodule

// example 2
module test(out);
output out;
`define wow
`define nest_one
`define second_nest
`define nest_two

`ifdef wow
		`ifdef nest_two
			initial $display("nest_two is defined");
		`else
			initial $display("nest_two is not defined");
		`endif
`else
		`ifdef nest_two
			initial $display("nest_two is defined");
		`else
			initial $display("nest_two is not defined");
		`endif
`endif

`ifdef wow
	initial $display("wow is defined");
	`ifdef nest_one
		initial $display("nest_one is defined");
		`ifdef nest_two
			initial $display("nest_two is defined");
		`else
			initial $display("nest_two is not defined");
		`endif
	`else
		initial $display("nest_one is not defined");
	`endif
`else
	initial $display("wow is not defined");
	`ifdef second_nest
		initial $display("nest_two is defined");
	`else
		initial $display("nest_two is not defined");
	`endif
`endif
endmodule

// example 3

module test2;
`ifdef first_block
	`ifndef second_nest
		initial $display("first_block is defined");
	`else
		initial $display("first_block and second_nest defined");
	`endif
`elsif second_block
	initial $display("second_block defined, first_block is not");
`else
	`ifndef last_result
		initial $display("first_block, second_block, last_result not defined.");
	`elsif real_last
		initial $display("first_block, second_block not defined, last_result and real_last defined.");
	`else
		initial $display("Only last_result defined!");
	`endif
`endif

endmodule



