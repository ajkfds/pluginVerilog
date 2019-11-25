
module a;

integer i;
b a_b1();

endmodule

module b;
	integer i;
	c b_c1(),b_c2();
	initial // downward path references two copies of i:
		#10 b_c1.i = 2;// a.a_b1.b_c1.i, d.d_b1.b_c1.i
endmodule

module c;
integer i;
initial begin // local name references four copies of i:
i = 1;		// a.a_b1.b_c1.i, a.a_b1.b_c2.i, 
			// d.d_b1.b_c1.i, d.d_b1.b_c2.i 
b.i = 1;	// upward path references two copies of i: 
			// a.a_b1.i, d.d_b1.i 
end 
endmodule


module d; 
integer i; 
b d_b1(); 
initial begin // full path name references each copy of i 
a.i = 1;			d.i = 5; 
a.a_b1.i = 2;		d.d_b1.i = 6; 
a.a_b1.b_c1.i = 3;	d.d_b1.b_c1.i = 7;
a.a_b1.b_c2.i = 4;	d.d_b1.b_c2.i = 8;

end 

task t;
reg r, s;
begin : b
	// redundant assignments to reg r 
	t.s = 0;	// fully defined downward reference 
	t.b.r = 0;	// poorly defined but found by upward search 
end 
endtask



endmodule


 endmodule