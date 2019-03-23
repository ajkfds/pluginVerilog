

module test;
parameter aaa = 1;


reg aa;

genvar i;

generate
	if(aaa == 0) begin : res
		reg aaaa;
		
	end else begin : res2
		reg aaaa;
	end
endgenerate

endmodule


//Example 1 A parameterized gray-code to binary-code converter module using a loop to generate continuous assignments 
module gray2bin1 (bin, gray);
parameter SIZE = 8;      // this module is parameterizable 
output [SIZE-1:0] bin;     
input  [SIZE-1:0] gray;  
	 
genvar i; 
	
generate for (i=0; i < SIZE ; i=i+1) begin:bit 
	assign bin[i] = ^gray[SIZE-1:i];  
end endgenerate  
endmodule


// Example 2 The same gray-code to binary-code converter module in example 1 is built using a loop to generate always blocks  
module gray2bin2 (bin, gray);
     parameter SIZE = 8;      // this module is parameterizable    
	 output [SIZE-1:0] bin; 
	 input  [SIZE-1:0] gray; 
	 reg    [SIZE-1:0] bin;
	 genvar i;     
	 generate for (i=0; i<SIZE; i=i+1) begin:bit  
	 always @(gray[SIZE-1:i]) // fixed part select  
	 bin[i] = ^gray[SIZE-1:i];
     end
endgenerate   
endmodule 


// Example 3 Generated ripple adder with two-dimensional net declaration outside of the generate loop 
module addergen1 (co, sum, a, b, ci);  
parameter SIZE = 4;  
output [SIZE-1:0] sum;
output            co; 
input  [SIZE-1:0] a, b; 
input             ci; 
wire   [SIZE  :0] c;
wire   [SIZE-1:0] t [1:3];   

genvar		i;

assign c[0] = ci;    
// Generated instance names are:  
// xor gates: bit[0].g1 bit[1].g1 bit[2].g1 bit[3].g1
//            bit[0].g2 bit[1].g2 bit[2].g2 bit[3].g2
// and gates: bit[0].g3 bit[1].g3 bit[2].g3 bit[3].g3  
//            bit[0].g4 bit[1].g4 bit[2].g4 bit[3].g4  
// or  gates: bit[0].g5 bit[1].g5 bit[2].g5 bit[3].g5
// Generated instances are connected with
// multi-dimensional nets t[1][3:0] t[2][3:0] t[3][3:0] 
// (12 multi-dimensional nets total)
generate    
	for(i=0; i<SIZE; i=i+1) begin:bit  
		xor	g1	( t[1][i], a[i],    b[i]   );
		xor	g2	( sum[i],  t[1][i], c[i]   );
		and	g3	( t[2][i], a[i],    b[i]   );
		and	g4	( t[3][i], t[1][i], c[i]   ); 
		or	g5	( c[i+1],  t[2][i], t[3][i]);  
	end 
endgenerate   
assign co = c[SIZE]; 

endmodule 



//  Example 4 Generated ripple adder with net declaration inside of the generate loop 

module generated_instantiation_addergen1 (co, sum, a, b, ci);   
parameter SIZE = 4;  
output [SIZE-1:0] sum;   
output            co;
input  [SIZE-1:0] a, b;
input             ci;
wire   [SIZE  :0] c;
genvar            i;   
assign c[0] = ci;
// Generated instance names are:  
// xor gates: bit[0].g1 bit[1].g1 bit[2].g1 bit[3].g1
//            bit[0].g2 bit[1].g2 bit[2].g2 bit[3].g2  
// and gates: bit[0].g3 bit[1].g3 bit[2].g3 bit[3].g3
//            bit[0].g4 bit[1].g4 bit[2].g4 bit[3].g4 
// or  gates: bit[0].g5 bit[1].g5 bit[2].g5 bit[3].g5 
// Generated instances are connected with 
// generated nets: bit[0].t1 bit[1].t1 bit[2].t1 bit[3].t1 
//                 bit[0].t2 bit[1].t2 bit[2].t2 bit[3].t2
//                 bit[0].t3 bit[1].t3 bit[2].t3 bit[3].t3   
generate
	for(i=0; i<SIZE; i=i+1) begin:bit    
		wire   t1, t2, t3; // generated net declaration    
		xor g1 (     t1, a[i], b[i]);
		xor g2 ( sum[i],   t1, c[i]);
		and g3 (     t2, a[i], b[i]);
		and g4 (     t3,   t1, c[i]);
		or  g5 ( c[i+1],   t2,   t3);
	end
endgenerate

assign co = c[SIZE]; 
endmodule 


// Example 5 A multi-level generate loop

module generated_instantiation_example5;
parameter	SIZE = 2; 
genvar		i, j, k, m; 
generate for (i=0; i<SIZE+1; i=i+1) begin:B1 // scope B1[i]    
	M1 N1(); // instantiates B1[i].N1[i]   
	for (j=0; j<SIZE; j=j+1) begin:B2 // scope B1[i].B2[j]     
		M2 N2(); // instantiates B1[i].B2[j].N2  
		for (k=0; k<SIZE; k=k+1) begin:B3 // scope B1[i].B2[j].B3[k]     
			M3 N3(); // instantiates B1[i].B2[j].B3[k].N3   
		end
	end
	if (i>0) for (m=0; m<SIZE; m=m+1) begin:B4 // scope B1[i].B4[m]       
		M4 N4(); // instantiates B1[i].B4[m].N4    
	end 
end
endgenerate 
// some of the generated instance names are:
// B1[0].N1  B1[1].N1 
// B1[0].B2[0].N2  B1[0].B2[1].N2 
// B1[0].B2[0].B3[0].N3  B1[0].B2[0].B3[1].N3 
// B1[0].B2[1].B3[0].N3
// B1[1].B4[0].N4  B1[1].B4[1].N4

endmodule

// Example 6 An implementation of a parameterized multiplier module 

module multiplier(a,b,product); 
parameter a_width = 8, b_width = 8; 
localparam product_width = a_width+b_width; // can not be modified 
// directly with the defparam statement 
// or the module instance statement # 
input    [a_width-1:0]    a;
input    [b_width-1:0]    b;
output   [product_width-1:0]    product; 

generate  
	if((a_width < 8) || (b_width < 8))
		CLA_multiplier #(a_width,b_width) u1(a, b, product); 
	// instance a CLA  multiplier   
	else   
		WALLACE_multiplier #(a_width,b_width) u1(a, b, product);  
	// instance a Wallace-tree  multiplier 
endgenerate // The generated instance name is u1

endmodule



//Example 7 Generate with a case to handle widths less that 3 

module example7;
localparam WIDTH = 8;

generate  
	case (WIDTH)
	1:  adder_1bit x1(co, sum, a, b, ci); 
	// 1-bit adder implementation     
	2:  adder_2bit x1(co, sum, a, b, ci); 
	// 2-bit adder implementation     
	default: adder_cla #(WIDTH) x1(co, sum, a, b, ci);
	// others - carry look-ahead adder  
	endcase // The generated instance name is x1 
endgenerate
endmodule


// Example 8 A module of memory dimm

module dimm;   parameter [31:0] MEM_SIZE  = 8, // in mbytes
MEM_WIDTH = 16;  
input [11:0] adr;
input [1:0]  ba;
input        rasx, casx, csx, wex;
input [7:0]  dqm;   
input        cke;   
input [7:0]  ds;   
inout [63:0] data;  
input [3:0]  clk;  
wire         rasb, casb, csb, web;
wire [7:0]   bex;   
genvar i;   
generate
case ({MEM_SIZE, MEM_WIDTH})   
	{32'd8, 32'd16}: // 8Meg 16 bits wide. 
		begin
			for (i=0;i<4;i = i + 1) begin:word        
				sms_16b216t0 p
					(.clk(clk), .csb(csx), .cke(cke), .ba(ba[0]),          
					.addr(adr[10:0]),.rasb(rasx), .casb(casx),    
					.web(wex),.udqm(dqm[2*i+1]), .ldqm(dqm[2*i]),
					.dqi(data[15+16*i:16*i]), .dev_id(dev_id3[4:0])            
				);
		end
		
		task read_mem;     
		input [31:0] address;
		output [63:0] data;
		begin
			word[3].p.read_mem(address, data[63:48]);
			word[2].p.read_mem(address, data[47:32]);
			word[1].p.read_mem(address, data[31:16]);
			word[0].p.read_mem(address, data[15:0]);
		end
		endtask
	end

// The generated instance names are word[3].p, word[2].p, 
// word[1].p, word[0].p, and the task read_mem     
	{32'd16, 32'd8}: // 16Meg 8 bits wide.        
		begin    
			for (i=0;i<4;i = i + 1) begin:byte      
        sms_16b208t0 p
       (.clk(clk), .csb(csx), .cke(cke), .ba(ba[0]),      
          .addr(adr[10:0]),      
           ...rasb(rasx), .casb(casx), .web(wex), .dqm(dqm[i]),     
           .dqi(data[8+8*i:8*i]),...dev_id(dev_id7[4:0])      
          );       
			end

task read_mem;        
input [31:0] address;
output [63:0] data;
begin
       byte[7].p.read_mem(address, data[63:56]);      
        byte[6].p.read_mem(address, data[55:48]);     
         byte[5].p.read_mem(address, data[47:40]);       
       byte[4].p.read_mem(address, data[39:32]);      
        byte[3].p.read_mem(address, data[31:24]);      
        byte[2].p.read_mem(address, data[23:16]);     
         byte[1].p.read_mem(address, data[15:8]);     
         byte[0].p.read_mem(address, data[7:0]);      
        end    
        endtask     
///    .....     
   endcase     
endgenerate   
// The generated instance names are byte[7].p, byte[6].p,  
// byte[5].p, byte[4].p, byte[3].p, byte[2].p, byte[1].p, 
// byte[0].p and the task read_mem 
endmodule 


