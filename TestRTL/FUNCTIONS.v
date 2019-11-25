`timescale 1n/1p

module FUNCTIONS;
/*
localparam	P_DELAY = 1;
localparam [7:0]	P_TEMP = 8'h00;

function [7:0] get_value;
input [7:0]	data;
reg [7:0] temp;
begin
	temp = P_TEMP;
	
end
endfunction
*/

reg [7:0] aaa = 1'b1 ? 8'h02 : 8'h01;

function conditional_execute;
input [3:0] condition;
input [3:0] flags;
begin
conditional_execute  
               = ( condition == AL                                        ) ||
                 ( condition == EQ  &&  flags[2]                          ) ||
                 ( condition == NE  && !flags[2]                          ) ||
                 ( condition == CS  &&  flags[1]                          ) ||
                 ( condition == CC  && !flags[1]                          ) ||
                 ( condition == MI  &&  flags[3]                          ) ||
                 ( condition == PL  && !flags[3]                          ) ||
                 ( condition == VS  &&  flags[0]                          ) ||
                 ( condition == VC  && !flags[0]                          ) ||
            
                 ( condition == HI  &&    flags[1] && !flags[2]           ) ||
                 ( condition == LS  &&  (!flags[1] ||  flags[2])          ) ||
            
                 ( condition == GE  &&  flags[3] == flags[0]              ) ||
                 ( condition == LT  &&  flags[3] != flags[0]              ) ||

                 ( condition == GT  &&  !flags[2] && flags[3] == flags[0] ) ||
                 ( condition == LE  &&  (flags[2] || flags[3] != flags[0])) ;
            
end
endfunction





endmodule
