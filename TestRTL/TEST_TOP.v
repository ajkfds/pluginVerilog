`timescale 1ns/1ps

`define VAL 8'h01
`define AAAA TOP_0

module TEST_TOP;

localparam P_DELAY = 1; // @delay

wire	aaa = 0;	// comment

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


TOP TOP_1 (
	.DATA_I	(  ),
	.DATA_O	(  ),
	.CLK_I	(  ),
	.RST_X	(  )
);


TOP TOP_2 (
	.DATA_I	(  ),
	.DATA_O	(  ),
	.CLK_I	(  ),
	.RST_X	(  )
);


always begin
	clk_i	<= #P_DELAY 1'b1;
	#100;
	clk_i	<= #P_DELAY 1'b0;
	#100;
end


integer i;

initial begin
	$display("aaa");
	end

















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
	force TOP_0.DATA_O = 8'h00;
	release TOP_0.DATA_O;
	
	assign	TOP_0.DATA_O = 1;
	
	TOP_0.DATA_O <= 1;
	
	$display("aaa");
	
	$finish;
end

endmodule





