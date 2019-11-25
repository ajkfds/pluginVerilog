
module MOD_TEST(IN1_I,IN2_I,OUT_O);
input	IN1_I;
input	IN2_I;
output	OUT_O;


endmodule


module TEST_MOD_TEST;

wire	abcdef = 1'b0;
wire	b = 1'b1;
wire	c;

MOD_TEST TT (abcdef,b,c);


endmoule





