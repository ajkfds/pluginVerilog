`timescale 1ns/1ps

module TEST2
(
input	CLK_I,	// @clock
input	RST_X	// @reset
);

localparam	P_DELAY = 1; // @delay

always @(posedge CLK_I or negedge RST_X)
begin
	if(~RST_X) begin
		
	end else begin
		
	end
end


always begin
	
end

localparam	P_TEST = 1;

generate
	if(P_TEST == 1) begin
		function aaa;
		input aa;
		begin
		end
		endfunction
	end else begin
		function aaa;
		begin
		end
		endfunction
	end
endgenerate


endmodule




