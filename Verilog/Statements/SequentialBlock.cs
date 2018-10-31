using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Statements
{
    public class SequentialBlock : Statement
    {
        protected SequentialBlock() { }
        public List<Statement> Statements = new List<Statement>();

        /*
        A.6.3 Parallel and sequential blocks
        function_seq_block      ::= begin[ : block_identifier { block_item_declaration } ] { function_statement }
        end variable_assignment ::= variable_lvalue = expression
        par_block               ::= fork [ : block_identifier { block_item_declaration } ] { statement }
        join seq_block          ::= begin[ : block_identifier { block_item_declaration } ] { statement } end  
        */
        public static SequentialBlock ParseCreate(WordScanner word,NameSpace nameSpace)
        {
            SequentialBlock sequentialBlock = new SequentialBlock();
            if(word.Text != "begin")
            {
                System.Diagnostics.Debugger.Break();
            }
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext(); // begin

            if(word.GetCharAt(0) == ':')
            {
                word.MoveNext(); // :
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal ifdentifier name");
                }
                else
                {
//                    sequentialBlock.Name = word.Text;
                    word.MoveNext();
                }
            }
            while (!word.Eof && word.Text != "end")
            {
                Statement statement = Statement.ParseCreateStatement(word, nameSpace);
                if (statement == null) break;
                sequentialBlock.Statements.Add(statement);
            }
            if(word.Text != "end")
            {
                word.AddError("illegal sequential block");
                return null;
            }
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext(); // end

            return sequentialBlock;
        }

    }
}
