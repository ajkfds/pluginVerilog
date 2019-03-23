// String variable declaration

reg [8*12:1] stringvar;
 initial begin
 stringvar = "Hello world!"; 
 end

// String manipulation

module string_test;
 reg [8*14:1] stringvar;
 initial begin 
 stringvar = "Hello world"; 
 $display("%s is stored as %h", stringvar,stringvar); 
 stringvar = {stringvar,"!!!"}; 
 $display("%s is stored as %h", stringvar,stringvar);
 end 
 endmodule 
