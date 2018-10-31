using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class Function : NameSpace
    {
        protected Function(Module module, NameSpace parent) : base(module,parent)
        {
        }

        public Dictionary<string, Variables.Port> Ports = new Dictionary<string, Variables.Port>();
        public Statements.Statement Statement;
        public string Name;

        public new static void Parse(WordScanner word, Module module)
        {
            if(word.Text != "function")
            {
                System.Diagnostics.Debugger.Break();
            }
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext();

            // function_declaration::= function[automatic][signed][range_or_type] function_identifier;
            // function_item_declaration { function_item_declaration }
            // function_statement
            // endfunction

            // | function[automatic][signed][range_or_type] function_identifier(function_port_list);
            // block_item_declaration { block_item_declaration }
            // function_statement 
            // endfunction 

            // function_item_declaration::= block_item_declaration | tf_input_declaration; function_port_list::= { attribute_instance }
            // tf_input_declaration { , { attribute_instance } tf_input_declaration }
            // range_or_type::= range | integer | real | realtime | time
            Function function = new Function(module, module);

            if (word.Text == "automatic")
            {
                word.Color((byte)Style.Color.Keyword);
                word.MoveNext();
            }

            bool signed = false;
            if (word.Text == "signed")
            {
                word.Color((byte)Style.Color.Keyword);
                word.MoveNext();
                signed = true;
            }

            Verilog.Variables.Range range = null;

            switch (word.Text)
            {
                case "[":
                    range = Verilog.Variables.Range.ParseCreate(word, function);
                    break;
                case "integer":
                    word.Color((byte)Style.Color.Identifier);
                    word.MoveNext();
                    break;
                case "real":
                    word.Color((byte)Style.Color.Identifier);
                    word.MoveNext();
                    break;
                case "realtime":
                    word.Color((byte)Style.Color.Identifier);
                    word.MoveNext();
                    break;
                case "time":
                    word.Color((byte)Style.Color.Identifier);
                    word.MoveNext();
                    break;
                default:
                    break;
            }

            if (!General.IsIdentifier(word.Text))
            {
                word.AddError("illegal identifier name");
                return;
            }

            function.Name = word.Text;
            word.Color((byte)Style.Color.Identifier);
            if (module.Functions.ContainsKey(function.Name))
            {
                word.AddError("duplicated name");
            }
            word.MoveNext();

            Variables.Reg reg = new Variables.Reg(function.Name, range, signed);
            function.Variables.Add(reg.Name, reg);


            /*            A.2.8 Block item declarations
                        block_item_declaration ::=            { attribute_instance }
                        block_reg_declaration | { attribute_instance }
                        event_declaration | { attribute_instance }
                        integer_declaration | { attribute_instance }
                        local_parameter_declaration | { attribute_instance }
                        parameter_declaration | { attribute_instance }
                        real_declaration | { attribute_instance }
                        realtime_declaration | { attribute_instance }
                        time_declaration block_reg_declaration ::= reg[signed][range]                    list_of_block_variable_identifiers;
            */
            if (word.Text == ";")
            {
                word.MoveNext();
                while(!word.Eof)
                {
                    switch (word.Text)
                    {
                        case "input":
                            Verilog.Variables.Port.ParsePortDeclaration(word,module, null);
                            if(word.Text != ";")
                            {
                                word.AddError("; expected");
                            }
                            else
                            {
                                word.MoveNext();
                            }
                            continue;
                        case "reg":
                            Verilog.Variables.Reg.ParseCreateDeclaration(word, function);
                            continue;
                        default:
                            break;
                    }
                    break;
                }

            }
            else
            {

            }

            Statements.Statement statement = Statements.Statement.ParseCreateFunctionStatement(word, function);

            if(word.Text != "endfunction")
            {
                word.AddError("endfunction expected");
                return;
            }
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext();

            if (!module.Functions.ContainsKey(function.Name))
            {
                module.Functions.Add(function.Name, function);
            }

            return;
        }

    }
}
