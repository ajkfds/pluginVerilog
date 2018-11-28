module TOP(
input [7:0]			DATA_I,		// @sync CLK_I
output reg [7:0]	DATA_O,		// @sync CLK_I
input	CLK_I,					// @clock
input	RST_X					// @reset
);

`include "TEST.vh"


`define MAC wire aaa;
`MAC

localparam P_DELAY = 1; // @delay


wire [7:0] data_next;
assign data_next = get_data_next(DATA_I);

always @(posedge CLK_I or negedge RST_X)
begin
	if(~RST_X) begin
		DATA_O <= #P_DELAY 8'h00;
	end else begin
		DATA_O <= #P_DELAY data_next;
	end
end


function [7:0] get_data_next;
input [7:0] data;
reg			overflow;
begin
	{overflow,get_data_next} = data + 8'h01;
end
endfunction

wire [7:0] dat;

MOD MOD0(
	.DATA_O(dat),
	.CLK_I(CLK_I),
	.RST_X(RST_X)
);


endmodule