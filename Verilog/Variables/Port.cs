using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Port : Item
    {
        public DirectionEnum Direction = DirectionEnum.Undefined;
        public bool Signed = false;
        public Range Range = null;

        public enum DirectionEnum
        {
            Undefined,
            Input,
            Output,
            Inout
        }

        public static Port Create(WordScanner word, Attribute attribute)
        {
            if (!General.IsIdentifier(word.Text)) return null;
            Port port = new Port();
            port.Name = word.Text;
            word.Color(CodeDrawStyle.ColorType.Net);
            word.MoveNext();
            return port;
        }

        public static void ParsePortDeclaration(WordScanner word, Module module, Attribute attribute)
        {
            // port_declaration::= { attribute_instance} inout_declaration | { attribute_instance} input_declaration | { attribute_instance} output_declaration  

            // inout_declaration::= inout[net_type][signed][range] list_of_port_identifiers
            // input_declaration ::= input[net_type][signed][range] list_of_port_identifiers
            // output_declaration ::=   output[net_type][signed][range] list_of_port_identifiers
            //                          | output[reg][signed][range] list_of_port_identifiers
            //                          | output reg[signed][range] list_of_variable_port_identifiers
            //                          | output[output_variable_type] list_of_port_identifiers
            //                          | output output_variable_type list_of_variable_port_identifiers 
            // list_of_port_identifiers::= (From Annex A -A.2.3) port_identifier { , port_identifier }
            // range ::= [ msb_constant_expression : lsb_constant_expression ]  

            List<Variable> variables;
            switch (word.Text)
            {
                case "input":
                    parseInputDeclaration(word, module, attribute, out variables);
                    if (variables.Count == 0) return;
                    foreach (Variable variable in variables)
                    {
                        Port port = new Port();
                        port.Name = variable.Name;
                        port.Direction = DirectionEnum.Input;
                        if (word.Active)
                        {
                            if (!module.Ports.ContainsKey(variable.Name))
                            {
                                module.Ports.Add(port.Name, port);
                            }
                            if (module.Variables.ContainsKey(variable.Name))
                            {
                                word.AddError("illegal port name");
                            }
                            else
                            {
                                module.Variables.Add(variable.Name, variable);
                            }
                        }
                    }
                    break;
                case "output":
                    parseOutputDeclaration(word, module, attribute, out variables);
                    if (variables.Count == 0) return;
                    foreach (Variable variable in variables)
                    {
                        Port port = new Port();
                        port.Name = variable.Name;
                        port.Direction = DirectionEnum.Output;
                        if (word.Active)
                        {
                            if (!module.Ports.ContainsKey(variable.Name))
                            {
                                module.Ports.Add(port.Name, port);
                            }
                            if (module.Variables.ContainsKey(variable.Name))
                            {
                                word.AddError("illegal port name");
                            }
                            else
                            {
                                module.Variables.Add(variable.Name, variable);
                            }
                        }
                    }
                    break;
                case "inout":
                    parseInoutDeclaration(word, module, attribute, out variables);
                    if (variables.Count == 0) return;
                    foreach (Variable variable in variables)
                    {
                        Port port = new Port();
                        port.Name = variable.Name;
                        port.Direction = DirectionEnum.Inout;
                        if (word.Active)
                        {
                            if (!module.Ports.ContainsKey(variable.Name))
                            {
                                module.Ports.Add(port.Name, port);
                            }
                            if (module.Variables.ContainsKey(variable.Name))
                            {
                                word.AddError("illegal port name");
                            }
                            else
                            {
                                module.Variables.Add(variable.Name, variable);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        public static void ParseFunctionPortDeclaration(WordScanner word, Function function, Attribute attribute)
        {
            // port_declaration::= { attribute_instance} inout_declaration | { attribute_instance} input_declaration | { attribute_instance} output_declaration  

            // inout_declaration::= inout[net_type][signed][range] list_of_port_identifiers
            // input_declaration ::= input[net_type][signed][range] list_of_port_identifiers
            // output_declaration ::=   output[net_type][signed][range] list_of_port_identifiers
            //                          | output[reg][signed][range] list_of_port_identifiers
            //                          | output reg[signed][range] list_of_variable_port_identifiers
            //                          | output[output_variable_type] list_of_port_identifiers
            //                          | output output_variable_type list_of_variable_port_identifiers 
            // list_of_port_identifiers::= (From Annex A -A.2.3) port_identifier { , port_identifier }
            // range ::= [ msb_constant_expression : lsb_constant_expression ]  

            List<Variable> variables;
            switch (word.Text)
            {
                case "input":
                    ParseTfInputDeclaration(word,function.Module,attribute, out variables);
                    if (variables.Count == 0) return;
                    foreach (Variable variable in variables)
                    {
                        Port port = new Port();
                        port.Name = variable.Name;
                        port.Direction = DirectionEnum.Input;
                        if (!function.Ports.ContainsKey(variable.Name))
                        {
                            function.Ports.Add(port.Name, port);
                        }
                        if (function.Variables.ContainsKey(variable.Name))
                        {
                            word.AddError("illegal port name");
                        }
                        else
                        {
                            function.Variables.Add(variable.Name, variable);
                        }
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        public static void ParseTaskPortDeclaration(WordScanner word, Task task, Attribute attribute)
        {

            List<Variable> variables;
            switch (word.Text)
            {
                case "input":
                    ParseTfInputDeclaration(word, task.Module, attribute, out variables);
                    if (variables.Count == 0) return;
                    foreach (Variable variable in variables)
                    {
                        Port port = new Port();
                        port.Name = variable.Name;
                        port.Direction = DirectionEnum.Input;
                        if (!task.Ports.ContainsKey(variable.Name))
                        {
                            task.Ports.Add(port.Name, port);
                        }
                        if (task.Variables.ContainsKey(variable.Name))
                        {
                            word.AddError("illegal port name");
                        }
                        else
                        {
                            task.Variables.Add(variable.Name, variable);
                        }
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        private static void parseInputDeclaration(WordScanner word, Module module, Attribute attribute, out List<Variable> variables)
        {
            // input_declaration ::= input[net_type][signed][range] list_of_port_identifiers
            variables = new List<Variable>();
            if (word.Text != "input") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            bool signed = false;

            Net.NetTypeEnum? netType = parseNetType(word);

            if (word.Text == "signed")
            {
                signed = true;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }

            Range range = null;
            if(word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, module);
            }

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) return;

                Net net = new Net();
                net.Name = word.Text;
                net.Range = range;
                net.Signed = signed;
                if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
                variables.Add(net);
                word.Color(CodeDrawStyle.ColorType.Net);
                word.MoveNext();

                if (word.GetCharAt(0) == ',')
                {
                    string next = word.NextText;
                    if (next == "input" || next == "output" || next == "inout")
                    {
                        break;
                    }
                    else
                    {
                        word.MoveNext(); // ,
                    }
                }
                else
                {
                    break;
                }
            }

        }

        private static void parseInoutDeclaration(WordScanner word, Module module, Attribute attribute, out List<Variable> variables)
        {
            // inout_declaration::= inout[net_type][signed][range] list_of_port_identifiers
            variables = new List<Variable>();
            if (word.Text != "inout") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            bool signed = false;

            Net.NetTypeEnum? netType = parseNetType(word);

            if (word.Text == "signed")
            {
                signed = true;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }

            Range range = null;
            if (word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, module);
            }


            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) return;

                Net net = new Net();
                net.Name = word.Text;
                net.Range = range;
                net.Signed = signed;
                if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
                variables.Add(net);
                word.Color(CodeDrawStyle.ColorType.Net);
                word.MoveNext();

                if (word.GetCharAt(0) == ',')
                {
                    string next = word.NextText;
                    if (next == "input" || next == "output" || next == "inout")
                    {
                        break;
                    }
                    else
                    {
                        word.MoveNext(); // ,
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private enum portVariableType
        {
            wire,
            time,
            real,
            realtime,
            integer,
            reg
        }
        private static void parseOutputDeclaration(WordScanner word, Module module, Attribute attribute, out List<Variable> variables)
        {
            // output_declaration ::=     output       [net_type] [signed][range] list_of_port_identifiers
            //                          | output [reg]            [signed][range] list_of_port_identifiers
            //                          | output  reg             [signed][range] list_of_variable_port_identifiers
            //                          | output [output_variable_type]           list_of_port_identifiers
            //                          | output output_variable_type             list_of_variable_port_identifiers 
            // output_variable_type ::= integer | time  

            // list_of_port_identifiers::= port_identifier { , port_identifier }
            // list_of_variable_port_identifiers::= port_identifier[ = constant_expression] { , port_identifier[ = constant_expression] }

            variables = new List<Variable>();
            if (word.Text != "output") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            bool signed = false;
            portVariableType varType = portVariableType.wire;

            Net.NetTypeEnum? netType = parseNetType(word);
            if (netType == null)
            {
                switch (word.Text)
                {
                    case "reg":
                        varType = portVariableType.reg;
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
                        break;
                    case "integer":
                        varType = portVariableType.integer;
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
                        break;
                    case "time":
                        varType = portVariableType.time;
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
                        break;
                    default:
                        netType = Net.NetTypeEnum.Wire;
                        break;
                }
            }

            if (word.Text == "signed")
            {
                signed = true;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }

            Range range = null;
            if (word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, module);
            }

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) return;

                switch (varType)
                {
                    case portVariableType.reg:
                        Reg reg = new Reg(word.Text, range, signed);
                        variables.Add(reg);
                        word.Color(CodeDrawStyle.ColorType.Net);
                        word.MoveNext();
                        break;
                    case portVariableType.integer:
                        Integer integer = new Integer(word.Text);
                        variables.Add(integer);
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        word.MoveNext();
                        break;
                    case portVariableType.time:
                        Time time = new Time(word.Text);
                        variables.Add(time);
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        word.MoveNext();
                        break;
                    case portVariableType.wire:
                        Net net = new Net();
                        net.Name = word.Text;
                        net.Range = range;
                        net.Signed = signed;
                        if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
                        variables.Add(net);
                        word.Color(CodeDrawStyle.ColorType.Net);
                        word.MoveNext();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if(varType != portVariableType.wire && word.Text == "=")
                {
                    word.MoveNext();
                    Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module);

                }

                if (word.GetCharAt(0) == ',')
                {
                    string next = word.NextText;
                    if (next == "input" || next == "output" || next == "inout")
                    {
                        break;
                    }
                    else
                    {
                        word.MoveNext(); // ,
                    }
                }
                else
                {
                    break;
                }
            }

        }

        private static Net.NetTypeEnum? parseNetType(WordScanner word)
        {
            // supply0 | supply1 | tri | triand  | trior | tri0 | tri1 | wire  | wand   | wor
            switch (word.Text)
            {
                case "supply0":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Supply0;
                case "supply1":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Supply1;
                case "tri":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Tri;
                case "triand":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Triand;
                case "trior":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Trior;
                case "tri0":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Tri0;
                case "tri1":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Tri1;
                case "wire":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Wire;
                case "wand":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Wand;
                case "wor":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Net.NetTypeEnum.Wor;
                default:
                    return  null;
            }
        }

        /*
        tf_output_declaration ::=   output [ reg ] [ signed ] [ range ] list_of_port_identifiers
                                    |  output [ task_port_type ] list_of_port_identifiers
        tf_inout_declaration  ::=   inout [ reg ] [ signed ] [ range ] list_of_port_identifiers
                                    |  inout [ task_port_type ] list_of_port_identifiers
        task_port_type ::=           time | real | realtime | integer
         */

        public static void ParseTfInputDeclaration(WordScanner word, Module module, Attribute attribute, out List<Variable> variables)
        {
            //            tf_input_declaration::= input[reg][signed][range] list_of_port_identifiers
            //                          | input[task_port_type] list_of_port_identifiers
            //            task_port_type::= time | real | realtime | integer

            portVariableType varType = portVariableType.wire;

            variables = new List<Variable>();
            if (word.Text != "input") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            if(word.Text == "time")
            {
                varType = portVariableType.time;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }
            else if (word.Text == "real")
            {
                varType = portVariableType.real;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }
            else if (word.Text == "realtime")
            {
                varType = portVariableType.realtime;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }
            else if (word.Text == "integer")
            {
                varType = portVariableType.integer;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }
            else if (word.Text == "reg")
            {
                varType = portVariableType.reg;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }

            bool signed = false;
            if (word.Text == "signed")
            {
                signed = true;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }

            Range range = null;
            if (word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, module);
            }

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) return;

                switch (varType)
                {
                    case portVariableType.wire:
                        Net val = new Net();
                        val.Name = word.Text;
                        val.Range = range;
                        val.Signed = signed;
                        variables.Add(val);
                        word.Color(CodeDrawStyle.ColorType.Net);
                        word.MoveNext();
                        break;
                    case portVariableType.reg:
                        Reg reg = new Reg(word.Text, range, signed);
                        variables.Add(reg);
                        word.Color(CodeDrawStyle.ColorType.Register);
                        word.MoveNext();
                        break;
                    case portVariableType.integer:
                        Integer integer = new Integer(word.Text);
                        variables.Add(integer);
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        word.MoveNext();
                        break;
                    case portVariableType.real:
                        Real real = new Real(word.Text);
                        variables.Add(real);
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        word.MoveNext();
                        break;
                    case portVariableType.realtime:
                        RealTime realtime = new RealTime(word.Text);
                        variables.Add(realtime);
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        word.MoveNext();
                        break;
                    case portVariableType.time:
                        Time time = new Time(word.Text);
                        variables.Add(time);
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        word.MoveNext();
                        break;

                }

                if (word.GetCharAt(0) == ',')
                {
                    string next = word.NextText;
                    if (next == "input" || next == "output" || next == "inout")
                    {
                        break;
                    }
                    else
                    {
                        word.MoveNext(); // ,
                    }
                }
                else
                {
                    break;
                }
            }

        }

    }
}