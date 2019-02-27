using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Statements
{
    public class ParallelBlock : IStatement
    {
        protected ParallelBlock() { }
        public List<IStatement> Statements = new List<IStatement>();

        /*
        A.6.3 Parallel and sequential blocks
        function_seq_block      ::= begin[ : block_identifier { block_item_declaration } ] { function_statement }
        end variable_assignment ::= variable_lvalue = expression
        par_block               ::= fork [ : block_identifier { block_item_declaration } ] { statement } join
        seq_block          ::= begin[ : block_identifier { block_item_declaration } ] { statement } end  
        */
        public static IStatement ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "fork")
            {
                System.Diagnostics.Debugger.Break();
            }
            word.Color(CodeDrawStyle.ColorType.Keyword);
            int beginIndex = word.RootIndex;
            word.MoveNext(); // begin

            if (word.GetCharAt(0) == ':')
            {
                NamedParallelBlock namedBlock = new NamedParallelBlock(nameSpace.Module, nameSpace);
                namedBlock.BeginIndex = beginIndex;

                word.MoveNext(); // :
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal ifdentifier name");
                }
                else
                {
                    if(word.Active && nameSpace.NameSpaces.ContainsKey(word.Text))
                    {
                        word.AddError("duplicated name");
                    }
                    namedBlock.Name = word.Text;
                    word.MoveNext();
                }
                while (!word.Eof && word.Text != "join")
                {
                    IStatement statement = Verilog.Statements.Statements.ParseCreateStatement(word, namedBlock);
                    if (statement == null) break;
                    namedBlock.Statements.Add(statement);
                }
                if (word.Text != "join")
                {
                    word.AddError("illegal sequential block");
                    return null;
                }
                word.Color(CodeDrawStyle.ColorType.Keyword);
                namedBlock.LastIndex = word.RootIndex;
                word.MoveNext(); // end

                if (word.Active && !nameSpace.NameSpaces.ContainsKey(namedBlock.Name))
                {
                    nameSpace.NameSpaces.Add(namedBlock.Name, namedBlock);
                }

                return namedBlock;
            }
            else
            {
                ParallelBlock sequentialBlock = new ParallelBlock();
                while (!word.Eof && word.Text != "join")
                {
                    IStatement statement = Verilog.Statements.Statements.ParseCreateStatement(word, nameSpace);
                    if (statement == null) break;
                    sequentialBlock.Statements.Add(statement);
                }
                if (word.Text != "join")
                {
                    word.AddError("illegal sequential block");
                    return null;
                }
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext(); // end

                return sequentialBlock;
            }
        }

    }

    public class NamedParallelBlock : Verilog.NameSpace, IStatement
    {
        public NamedParallelBlock(Module module, NameSpace parent) : base(module, parent)
        {
        }

        public List<IStatement> Statements = new List<IStatement>();
    }

}
