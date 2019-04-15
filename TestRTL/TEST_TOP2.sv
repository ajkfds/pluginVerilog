`timescale 1ns/1ps
`define VAL 8'h01

program TEST_TOP_SV;

localparam P_DELAY = 1; // @delay

reg [7:0]	data_i;
wire [7:0]	data_o;
reg		clk_i;
reg		rst_x;



TOP TOP_0 (
	.DATA_I	( data_i ),
	.DATA_O	( data_o ),
	.CLK_I	( clk_i ),
	.RST_X	( rst_x )
);



integer i;

initial begin
	data_i = 0;
	rst_x = 0;
	@(posedge clk_i);
	data_i	<= #P_DELAY `VAL;
	rst_x	<= #P_DELAY 1'b1;
	@(posedge clk_i);
	@(posedge clk_i);
	@(posedge clk_i);
	$display("out %h",data_o);
	$finish;
end


class test;
end class;

endprogram



