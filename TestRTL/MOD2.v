
`celldefine
module MOD2(a,b,c);
input a;
output b;
input c;


reg [7:0]	rr [0:10];


reg [3:0]	bb = rr[0][3:0];

genvar i;

parameter P_INDEX = 0;

generate

if (P_INDEX == 1) begin : ifgen

	
function [1:0] get_value;
input	a;
reg b;
begin
	if(1'b1==l) get_value = 1;
	
end
endfunction

end else if(P_INDEX == 1) begin


end

endgenerate


and(b,g);
and (a,b);
buf (a,b);


endmodule

`endcelldefine


