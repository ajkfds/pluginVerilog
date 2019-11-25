
module MOD #(
	parameter	P_SELECT = 0
)(
output reg [7:0]	DATA_O,
input	CLK_I,
inout aaa,
input	RST_X
);

localparam P_DELAY = 1;

/*
generate
	if(P_SELECT == 0) begin
		reg	reg0;
	end else if(P_SELECT == 1) begin
		reg	reg1;
	end else if(P_SELECT == 2) begin
		reg	reg2;
	end else if(P_SELECT == 3) begin
		reg	reg3;
	end else if(P_SELECT == 4) begin
		reg	reg4;
	end else if(P_SELECT == 5) begin
		reg	reg5;
	end else if(P_SELECT == 6) begin
		reg	reg6;
	end
	
endgenerate
*/

	always @(posedge CLK_I or negedge RST_X)
	begin
		if(~RST_X) begin
			DATA_O	<= #P_DELAY 8'h00;
		end else begin
			DATA_O	<= #P_DELAY 8'h11;
			
			
			
		end
	end

MOD2 MOD2_0 (
	.a	(  ),
	.b	(  ),
	.c	(  )
);

MOD3 MOD3_0 (
	.CLK_I	(  ),
	.RST_X	(  )
);

endmodule



endmodule