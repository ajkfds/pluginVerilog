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
        public List<Variables.Port> PortsList = new List<Variables.Port>();
        public Statements.IStatement Statement;

        private enum valType
        {
            reg,
            integer,
            real,
            realtime,
            time
        }

        public static void Parse(WordScanner word, Module module)
        {
            if(word.Text != "function")
            {
                System.Diagnostics.Debugger.Break();
            }
            Function function = new Function(module, module);
            word.Color(CodeDrawStyle.ColorType.Keyword);
            function.BeginIndex = word.RootIndex;
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

            if (word.Text == "automatic")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }

            bool signed = false;
            if (word.Text == "signed")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                signed = true;
            }

            Verilog.Variables.Range range = null;
            valType valType = valType.reg;

            switch (word.Text)
            {
                case "[":
                    range = Verilog.Variables.Range.ParseCreate(word, function);
                    break;
                case "integer":
                    valType = valType.integer;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                case "real":
                    valType = valType.real;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                case "realtime":
                    valType = valType.realtime;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                case "time":
                    valType = valType.time;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
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
            word.Color(CodeDrawStyle.ColorType.Identifier);
            if (word.Active && module.Functions.ContainsKey(function.Name) || module.NameSpaces.ContainsKey(function.Name))
            {
                word.AddError("duplicated name");
            }
            word.MoveNext();

            Variables.Variable retVal;
            switch (valType)
            {
                case valType.reg:
                    retVal = new Variables.Reg(function.Name, range, signed);
                    break;
                case valType.integer:
                    retVal = new Variables.Integer(function.Name);
                    break;
                case valType.real:
                    retVal = new Variables.Real(function.Name);
                    break;
                case valType.realtime:
                    retVal = new Variables.RealTime(function.Name);
                    break;
                case valType.time:
                    retVal = new Variables.Time(function.Name);
                    break;
                default:
                    throw new Exception();
            }
            function.Variables.Add(retVal.Name, retVal);


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
                            Verilog.Variables.Port.ParseFunctionPortDeclaration(word,function, null);
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
                            Verilog.Variables.Reg.ParseCreateFromDeclaration(word, function);
                            continue;
                        case "integer":
                            Verilog.Variables.Integer.ParseCreateFromDeclaration(word, function);
                            continue;
                        case "real":
                            Verilog.Variables.Real.ParseCreateFromDeclaration(word, function);
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

            Statements.IStatement statement = Statements.Statements.ParseCreateFunctionStatement(word, function);

            if(word.Text != "endfunction")
            {
                word.AddError("endfunction expected");
                return;
            }
            word.Color(CodeDrawStyle.ColorType.Keyword);
            function.LastIndex = word.RootIndex;
            word.MoveNext();

            if (!word.Active) return;
            if (module.Functions.ContainsKey(function.Name)) return;
            if (module.NameSpaces.ContainsKey(function.Name)) return;

            module.Functions.Add(function.Name, function);
            module.NameSpaces.Add(function.Name,function);

            return;
        }

    }
}
