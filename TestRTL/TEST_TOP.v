module TOP;

localparam P_DELAY = 1; // @delay

integer i;

initial begin
	i=0;
	$display("launched");
	forever begin
		$display("%d",i);
		i=i+1;
	end
	#100;
	$finish;
end



endmodule