using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Statements
{
    public class SequentialBlock : IStatement
    {
        protected SequentialBlock() { }
        public List<IStatement> Statements = new List<IStatement>();

        /*
        A.6.3 Parallel and sequential blocks
        function_seq_block      ::= begin[ : block_identifier { block_item_declaration } ] { function_statement }
        end variable_assignment ::= variable_lvalue = expression
        par_block               ::= fork [ : block_identifier { block_item_declaration } ] { statement }
        join seq_block          ::= begin[ : block_identifier { block_item_declaration } ] { statement } end  


        block_item_declaration ::=  { attribute_instance } block_reg_declaration          
                                    | { attribute_instance } event_declaration          
                                    | { attribute_instance } integer_declaration          
                                    | { attribute_instance } local_parameter_declaration          
                                    | { attribute_instance } parameter_declaration          
                                    | { attribute_instance } real_declaration          
                                    | { attribute_instance } realtime_declaration          
                                    | { attribute_instance } time_declaration 
        block_reg_declaration ::= reg [ signed ] [ range ] list_of_block_variable_identifiers ;
        list_of_block_variable_identifiers ::=  block_variable_type { , block_variable_type } 
        block_variable_type ::=  variable_identifier        | variable_identifier dimension { dimension }  
        */
        public static IStatement ParseCreate(WordScanner word,NameSpace nameSpace)
        {
            if(word.Text != "begin")
            {
                System.Diagnostics.Debugger.Break();
            }
            word.Color(CodeDrawStyle.ColorType.Keyword);
            int beginIndex = word.RootIndex;
            word.MoveNext(); // begin

            if (word.GetCharAt(0) == ':')
            {
                NamedSequentialBlock namedBlock = new NamedSequentialBlock(nameSpace.Module, nameSpace);
                namedBlock.BeginIndex = beginIndex;

                word.MoveNext(); // :
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal ifdentifier name");
                }
                else
                {
                    if (!word.Active)
                    { // prototype
                        if (nameSpace.NameSpaces.ContainsKey(word.Text))
                        {
                            word.AddError("duplicated name");
                            namedBlock.Name = word.Text;
                        }
                        else
                        {
                            namedBlock.Name = word.Text;
                            nameSpace.NameSpaces.Add(namedBlock.Name, namedBlock);
                        }
                        word.MoveNext();
                    }
                    else
                    { // implementation
                        if (nameSpace.NameSpaces.ContainsKey(word.Text) && nameSpace.NameSpaces[word.Text] is NamedSequentialBlock)
                        {
                            word.Color(CodeDrawStyle.ColorType.Identifier);
                            namedBlock = nameSpace.NameSpaces[word.Text] as NamedSequentialBlock;
                        }
                        word.MoveNext();
                    }
                }

                while (!word.Eof && word.Text != "end")
                {
                    bool endFlag = false;
                    switch (word.Text)
                    {
                        case "reg":
                            Variables.Reg.ParseCreateFromDeclaration(word, namedBlock);
                            break;
                        case "integer":
                            Variables.Integer.ParseCreateFromDeclaration(word, namedBlock);
                            break;
                        case "real":
                            Verilog.Variables.Real.ParseCreateFromDeclaration(word, namedBlock);
                            break;
                        case "realtime":
                            Verilog.Variables.RealTime.ParseCreateFromDeclaration(word, namedBlock);
                            break;
                        case "time":
                            Verilog.Variables.Time.ParseCreateFromDeclaration(word, namedBlock);
                            break;
                        case "event":
                            Verilog.Variables.Event.ParseCreateFromDeclaration(word, namedBlock);
                            break;
                        case "parameter":
                        case "localparam":
                            Verilog.Variables.Parameter.ParseCreateDeclaration(word, namedBlock, null);
                            break;
                        default:
                            endFlag = true;
                            break;
                    }
                    if (endFlag) break;
                }


                while (!word.Eof && word.Text != "end")
                {
                    IStatement statement = Verilog.Statements.Statements.ParseCreateStatement(word, namedBlock);
                    if (statement == null) break;
                    namedBlock.Statements.Add(statement);
                }
                if (word.Text != "end")
                {
                    word.AddError("illegal sequential block");
                    return null;
                }
                word.Color(CodeDrawStyle.ColorType.Keyword);
                namedBlock.LastIndex = word.RootIndex;
                word.MoveNext(); // end

                if (namedBlock.Name != null && !nameSpace.NameSpaces.ContainsKey(namedBlock.Name))
                {
                    nameSpace.NameSpaces.Add(namedBlock.Name,namedBlock);
                }

                return namedBlock;
            }
            else
            {
                SequentialBlock sequentialBlock = new SequentialBlock();
                while (!word.Eof && word.Text != "end")
                {
                    IStatement statement = Verilog.Statements.Statements.ParseCreateStatement(word, nameSpace);
                    if (statement == null) break;
                    sequentialBlock.Statements.Add(statement);
                }
                if (word.Text != "end")
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

    public class NamedSequentialBlock : Verilog.NameSpace,IStatement
    {
        public NamedSequentialBlock(Module module, NameSpace parent) : base(module,parent)
        {
        }

        public List<IStatement> Statements = new List<IStatement>();
    }

}
