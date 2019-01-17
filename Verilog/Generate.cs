using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class Generate
    {
        private static void parseGenerateLoopStatement(WordScanner word, NameSpace nameSpace)
        {
            // generate_loop_statement::=  for (genvar_assignment; constant_expression; genvar_assignment) begin : generate_block_identifier { generate_item } end
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            if (word.Text != "(")
            {
                word.AddError("( expected");
                return;
            }
            word.MoveNext();

            if(!parseGenvarAssignment(word, nameSpace))
            {
                return;
            }

            if (word.Text != ";")
            {
                word.AddError("; expected");
                return;
            }
            word.MoveNext();

            Expressions.Expression constant = Expressions.Expression.ParseCreate(word, nameSpace);
            if (constant == null) return;
            if (!constant.Constant)
            {
                word.AddError("should be constant");
            }

            if (word.Text != ";")
            {
                word.AddError("; expected");
                return;
            }
            word.MoveNext();


        }

        private static bool parseGenvarAssignment(WordScanner word, NameSpace nameSpace)
        {
            //    genvar_assignment::= genvar_identifier = constant_expression
            Expressions.VariableReference genvar = Expressions.VariableReference.ParseCreate(word, nameSpace);
            if (genvar == null) return false;
            if(!(genvar.Variable is Variables.Genvar))
            {
                word.AddError("should be genvar");
            }
            if (word.Text != "=")
            {
                word.AddError("( expected");
                return false;
            }
            word.MoveNext();
            Expressions.Expression constant = Expressions.Expression.ParseCreate(word, nameSpace);
            if (constant == null) return false;
            if (!constant.Constant)
            {
                word.AddError("should be constant");
            }
            return true;
        }


        // generate_loop_statement ::=  for ( genvar_assignment ; constant_expression ; genvar_assignment ) begin : generate_block_identifier { generate_item } end 
        //        generate_conditional_statement::= if (constant_expression ) generate_item_or_null[ else generate_item_or_null]
        //        generate_case_statement::=  case (constant_expression )
        //       genvar_case_item
        //        { genvar_case_item }
        //        endcase
        //genvar_case_item ::=  constant_expression  { , constant_expression } :      
        //        generate_item_or_null  | default [ : ]
        //        generate_item_or_null
        //        generate_block ::= begin[ : generate_block_identifier]  { generate_item }
        //        end
    }
}
