using pluginVerilog.Verilog.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class Function : NameSpace, IPortNameSpace
    {
        protected Function(NameSpace parent) : base(parent.BuildingBlock, parent)
        {
        }

        private Dictionary<string, Variables.Port> ports = new Dictionary<string, Variables.Port>();
        public Dictionary<string, Variables.Port> Ports { get { return ports; } }
        private List<Variables.Port> portsList = new List<Variables.Port>();
        public List<Variables.Port> PortsList { get { return portsList; } }

        public Statements.IStatement Statement;

        private enum valType
        {
            reg,
            integer,
            real,
            realtime,
            time
        }

        public static void Parse(WordScanner word, NameSpace nameSpace)
        {
            if(word.Text != "function")
            {
                System.Diagnostics.Debugger.Break();
            }
            Function function = new Function(nameSpace);
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

            // function_item_declaration::= block_item_declaration | tf_input_declaration;
            // function_port_list::= { attribute_instance }
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

            if (!word.Active)
            {
                // skip
            }
            else if (word.Prototype)
            {
                if (!nameSpace.BuildingBlock.Functions.ContainsKey(function.Name) && !nameSpace.BuildingBlock.NameSpaces.ContainsKey(function.Name))
                {
                    nameSpace.BuildingBlock.Functions.Add(function.Name, function);
                    nameSpace.BuildingBlock.NameSpaces.Add(function.Name, function);
                }
                else
                {
                    word.AddError("duplicated name");
                }
            }
            else
            {
                if (nameSpace.BuildingBlock.Functions.ContainsKey(function.Name))
                {
                    function = nameSpace.BuildingBlock.Functions[function.Name];
                }
            }
            word.Color(CodeDrawStyle.ColorType.Identifier);
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

            if (word.Prototype)
            {
                function.Variables.Add(retVal.Name, retVal);
            }

            /*            
            // function_item_declaration::= block_item_declaration | tf_input_declaration;

            A.2.8 Block item declarations
            
            block_item_declaration ::=
                { attribute_instance } block_reg_declaration |
                { attribute_instance } event_declaration |
                { attribute_instance } integer_declaration |
                { attribute_instance } local_parameter_declaration |
                { attribute_instance } parameter_declaration |
                { attribute_instance } real_declaration |
                { attribute_instance } realtime_declaration |
                { attribute_instance } time_declaration
            
            block_reg_declaration ::= reg[signed][range] list_of_block_variable_identifiers;
            */

            if (word.Text == ";")
            {
                word.MoveNext();

                // TODO imlement { attriute_instance }
                while(!word.Eof)
                {
                    switch (word.Text)
                    {
                        case "input": // tf_input_declaration
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
                        case "reg": // block_reg_declaration
                            Verilog.Variables.Reg.ParseCreateFromDeclaration(word, function);
                            continue;
                        // event_declaration
                        case "integer": // integer_declaration
                            Verilog.Variables.Integer.ParseCreateFromDeclaration(word, function);
                            continue;
                        case "localparameter": // local_parameter_declaration
                        case "paraeter":  // parameter_declaration
                            Verilog.Variables.Parameter.ParseCreateDeclaration(word, function,null);
                            continue;
                        case "real": // real_declaration
                            Verilog.Variables.Real.ParseCreateFromDeclaration(word, function);
                            continue;
                        case "realtime": // realtime_declaration
                            Verilog.Variables.RealTime.ParseCreateFromDeclaration(word, function);
                            continue;
                        case "time": // time_declaration
                            Verilog.Variables.Time.ParseCreateFromDeclaration(word, function);
                            continue;
                        case "wire": // illegal format for Verilog 2001
                            word.AddError("not supported(Veriog2001)");
                            Verilog.Variables.Net.ParseCreateFromDeclaration(word, function);
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

            if(word.Text == "endfunction")
            {
                word.AddError("statement required");
            }
            else
            {
                if(word.Prototype)
                {
                    while (!word.Eof)
                    {
                        if (word.Text == "endfunction") break;
                        switch (word.Text)
                        {
                            case "endmodule":
                            case "endtask":
                            case "always":
                            case "initial":
                                word.AddError("missed endfunction");
                                return;
                            default:
                                break;
                        }
                        word.MoveNext();
                    }
                }
                else
                {
                    Statements.IStatement statement = Statements.Statements.ParseCreateFunctionStatement(word, function);
                }
            }

            if(word.Text != "endfunction")
            {
                word.AddError("endfunction expected");
                return;
            }


            word.Color(CodeDrawStyle.ColorType.Keyword);
            function.LastIndex = word.RootIndex;
            word.AppendBlock(function.BeginIndex, function.LastIndex);
            word.MoveNext();

            return;
        }

    }
}
