`timescale 1ns/1ps

module TOP#(
	parameter	WIDTH = 10
)(
// @section data
input [7:0]			DATA_I,		// @sync CLK_I
output reg [7:0]	DATA_O,		// @sync CLK_I
// @section	clock & reset
input	CLK_I,					// @sync clock
input	RST_X					// @sync reset for CLK_I
);

// @section aa


`include "TEST.vh"


MOD #(
	.P_SELECT(1)
) MOD1 (
	.DATA_O	(  ),
	.CLK_I	(  ),
	.aaa	(  ),
	.RST_X	(  )
);

localparam P_DELAY = 1; // @delay

reg testReg = 1;

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


function testes;
input aaa;
begin
	testes = 1'b1;
end
endfunction

endmodule



