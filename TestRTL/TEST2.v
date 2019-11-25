`timescale 1ns/1ps

module TEST2
(
input	CLK_I,	// @clock
input	RST_X	// @reset
);


reg [7:0] aaa;
reg [7:0] bbb;
reg [8:0] ccc;

assign ccc = aaa + {7'h00,1'b0};

endmodule


