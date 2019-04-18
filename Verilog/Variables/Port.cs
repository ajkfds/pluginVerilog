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
//        public bool Signed = false;
        public Range Range = null;
        public Variable Variable = null;

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

        public ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            switch (Direction)
            {
                case DirectionEnum.Input:
                    label.AppendText("input ",CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case DirectionEnum.Output:
                    label.AppendText("output ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case DirectionEnum.Inout:
                    label.AppendText("inout ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                default:
                    break;
            }

            if (Range != null)
            {
                label.AppendLabel(Range.GetLabel());
            }

            if (Variable != null)
            {
                if(Variable is Net)
                {
                    label.AppendText(Name, CodeDrawStyle.Color(CodeDrawStyle.ColorType.Net));
                }
                else if(Variable is Reg)
                {
                    label.AppendText(Name, CodeDrawStyle.Color(CodeDrawStyle.ColorType.Register));
                }
                else
                {
                    label.AppendText(Name);
                }
            }

            return label;
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
            // list_of_port_identifiers::= port_identifier { , port_identifier }
            // range ::= [ msb_constant_expression : lsb_constant_expression ]  

            switch (word.Text)
            {
                case "input":
                    parseInputDeclaration(word, module, module, attribute);
                    break;
                case "output":
                    parseOutputDeclaration(word, module, module, attribute);
                   break;
                case "inout":
                    parseInoutDeclaration(word, module,module, attribute);
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

            switch (word.Text)
            {
                case "input":
                    ParseTfInputDeclaration(word,function.Module,function,attribute);
                    break;
                default:
                    break;
            }
            return;
        }

        /*        tf_input_declaration::= input[reg][signed][range] list_of_port_identifiers 
                                            | input[task_port_type] list_of_port_identifiers
                tf_output_declaration::= output[reg][signed][range] list_of_port_identifiers 
                                            | output[task_port_type] list_of_port_identifiers
                tf_inout_declaration::= inout[reg][signed][range] list_of_port_identifiers 
                                            | inout[task_port_type] list_of_port_identifiers
                task_port_type::= time 
                                            | real 
                                            | realtime 
                                            | integer
        */
        public static void ParseTaskPortDeclaration(WordScanner word, Task task, Attribute attribute)
        {
            switch (word.Text)
            {
                case "input":
                    ParseTfInputDeclaration(word, task.Module, task, attribute);
                    break;
                case "inout":
                    ParseTfInoutDeclaration(word, task.Module, task, attribute);
                    break;
                case "output":
                    ParseTfOutputDeclaration(word, task.Module, task, attribute);
                    break;
                default:
                    break;
            }
            return;
        }

        public static void ParseClassPortDeclaration(WordScanner word, Class classItem, Attribute attribute)
        {
            switch (word.Text)
            {
                case "input":
                    ParseTfInputDeclaration(word, classItem.Module, classItem, attribute);
                    break;
                case "inout":
                    ParseTfInoutDeclaration(word, classItem.Module, classItem, attribute);
                    break;
                case "output":
                    ParseTfOutputDeclaration(word, classItem.Module, classItem, attribute);
                    break;
                default:
                    break;
            }
            return;
        }


        private static void parseInputDeclaration(WordScanner word, NameSpace nameSpace, IPortNameSpace portNameSpace, Attribute attribute)
        {
            // input_declaration ::= input[net_type][signed][range] list_of_port_identifiers
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
            if (word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, nameSpace);
            }

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal port name");
                    return;
                }

                Net net = new Net();
                net.Name = word.Text;
                net.Range = range;
                net.Signed = signed;
                if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
                word.Color(CodeDrawStyle.ColorType.Net);

                Port port = new Port();
                port.Name = net.Name;
                port.Direction = DirectionEnum.Input;
                port.Variable = net;

                if (!word.Active)
                {
                    // skip
                }
                else if (word.Prototype)
                {
                    if (!portNameSpace.Variables.ContainsKey(port.Name))
                    {
                        portNameSpace.Variables.Add(net.Name, net);
                    }

                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        word.AddError("port name duplicated");
                    }
                    else
                    {
                        portNameSpace.Ports.Add(port.Name, port);
                    }
                }
                else
                {
                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        port = portNameSpace.Ports[port.Name];
                    }
                }

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
        
        private static void parseInoutDeclaration(WordScanner word, Module module, IPortNameSpace portNameSpace, Attribute attribute)
        {
            // inout_declaration::= inout[net_type][signed][range] list_of_port_identifiers
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
                word.Color(CodeDrawStyle.ColorType.Net);

                Port port = new Port();
                port.Name = net.Name;
                port.Direction = DirectionEnum.Inout;
                port.Variable = net;

                if (!word.Active)
                {
                    // skip
                }
                else if (word.Prototype)
                {
                    if (!portNameSpace.Variables.ContainsKey(port.Name))
                    {
                        portNameSpace.Variables.Add(net.Name, net);
                    }

                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        word.AddError("port name duplicated");
                    }
                    else
                    {
                        portNameSpace.Ports.Add(port.Name, port);
                        portNameSpace.PortsList.Add(port);
                    }
                }
                else
                {
                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        port = portNameSpace.Ports[port.Name];
                    }
                }
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

        private static void parseOutputDeclaration(WordScanner word, Module module, IPortNameSpace portNameSpace, Attribute attribute)
        {
            // output_declaration ::=     output       [net_type] [signed][range] list_of_port_identifiers
            //                          | output [reg]            [signed][range] list_of_port_identifiers
            //                          | output  reg             [signed][range] list_of_variable_port_identifiers
            //                          | output [output_variable_type]           list_of_port_identifiers
            //                          | output output_variable_type             list_of_variable_port_identifiers 
            // output_variable_type ::= integer | time  

            // list_of_port_identifiers::= port_identifier { , port_identifier }
            // list_of_variable_port_identifiers::= port_identifier[ = constant_expression] { , port_identifier[ = constant_expression] }

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

            Variable variable;
            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) return;

                switch (varType)
                {
                    case portVariableType.reg:
                        variable = new Reg(word.Text, range, signed);
                        word.Color(CodeDrawStyle.ColorType.Net);
                        break;
                    case portVariableType.integer:
                        variable = new Integer(word.Text);
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.time:
                        variable = new Time(word.Text);
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.wire:
                        Net net = new Net();
                        net.Name = word.Text;
                        net.Range = range;
                        net.Signed = signed;
                        if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
                        variable = net;
                        word.Color(CodeDrawStyle.ColorType.Net);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                Port port = new Port();
                port.Name = variable.Name;
                port.Direction = DirectionEnum.Output;
                port.Variable = variable;

                if (!word.Active)
                {
                    // skip
                }
                else if (word.Prototype)
                {
                    if (!portNameSpace.Variables.ContainsKey(variable.Name))
                    {
                        portNameSpace.Variables.Add(variable.Name, variable);
                    }

                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        word.AddError("port name duplicated");
                    }
                    else
                    {
                        portNameSpace.Ports.Add(port.Name, port);
                        portNameSpace.PortsList.Add(port);
                    }
                }
                else
                {
                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        port = portNameSpace.Ports[port.Name];
                    }
                }
                word.MoveNext();

                if (varType != portVariableType.wire && word.Text == "=")
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

        public static void ParseTfOutputDeclaration(WordScanner word, Module module, IPortNameSpace portNameSpace, Attribute attribute)
        {
            // tf_output_declaration::= output[reg][signed][range] list_of_port_identifiers
            //                        | output[task_port_type] list_of_port_identifiers
            // task_port_type::= time | real | realtime | integer

            portVariableType varType = portVariableType.wire;

            if (word.Text != "output") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            bool signed = false;
            Range range = null;

            if (word.Text == "time")
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

                if (word.Text == "signed")
                {
                    signed = true;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                }

                if (word.GetCharAt(0) == '[')
                {
                    range = Range.ParseCreate(word, module);
                }
            }
            else
            {
                if (word.Text == "signed")
                {
                    signed = true;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                }

                if (word.GetCharAt(0) == '[')
                {
                    range = Range.ParseCreate(word, module);
                }
            }

            Variable variable;
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
                        variable = val;
                        word.Color(CodeDrawStyle.ColorType.Net);
                        break;
                    case portVariableType.reg:
                        Reg reg = new Reg(word.Text, range, signed);
                        variable = reg;
                        word.Color(CodeDrawStyle.ColorType.Register);
                        break;
                    case portVariableType.integer:
                        Integer integer = new Integer(word.Text);
                        variable = integer;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.real:
                        Real real = new Real(word.Text);
                        variable = real;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.realtime:
                        RealTime realtime = new RealTime(word.Text);
                        variable = realtime;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.time:
                        Time time = new Time(word.Text);
                        variable = time;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    default:
                        System.Diagnostics.Debugger.Break();
                        variable = null;
                        break;
                }

                Port port = new Port();
                port.Name = variable.Name;
                port.Direction = DirectionEnum.Output;
                port.Variable = variable;

                if (!word.Active)
                {
                    // skip
                }
                else if (word.Prototype)
                {
                    if (!portNameSpace.Variables.ContainsKey(variable.Name))
                    {
                        portNameSpace.Variables.Add(variable.Name, variable);
                    }

                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        word.AddError("port name duplicated");
                    }
                    else
                    {
                        portNameSpace.Ports.Add(port.Name, port);
                        portNameSpace.PortsList.Add(port);
                    }
                }
                else
                {
                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        port = portNameSpace.Ports[port.Name];
                    }
                }
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

        public static void ParseTfInoutDeclaration(WordScanner word, Module module, IPortNameSpace portNameSpace, Attribute attribute)
        {
            // tf_inout_declaration::= inout[reg][signed][range] list_of_port_identifiers
            //                       | inout[task_port_type] list_of_port_identifiers
            // task_port_type::= time | real | realtime | integer

            portVariableType varType = portVariableType.wire;

            if (word.Text != "inout") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            bool signed = false;
            Range range = null;

            if (word.Text == "time")
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

                if (word.Text == "signed")
                {
                    signed = true;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                }

                if (word.GetCharAt(0) == '[')
                {
                    range = Range.ParseCreate(word, module);
                }
            }
            else
            {
                if (word.Text == "signed")
                {
                    signed = true;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                }

                if (word.GetCharAt(0) == '[')
                {
                    range = Range.ParseCreate(word, module);
                }
            }

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) return;

                Variable variable;
                switch (varType)
                {
                    case portVariableType.wire:
                        Net val = new Net();
                        val.Name = word.Text;
                        val.Range = range;
                        val.Signed = signed;
                        variable = val;
                        word.Color(CodeDrawStyle.ColorType.Net);
                        break;
                    case portVariableType.reg:
                        Reg reg = new Reg(word.Text, range, signed);
                        variable = reg;
                        word.Color(CodeDrawStyle.ColorType.Register);
                        break;
                    case portVariableType.integer:
                        Integer integer = new Integer(word.Text);
                        variable = integer;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.real:
                        Real real = new Real(word.Text);
                        variable = real;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.realtime:
                        RealTime realtime = new RealTime(word.Text);
                        variable = realtime;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.time:
                        Time time = new Time(word.Text);
                        variable = time;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    default:
                        System.Diagnostics.Debugger.Break();
                        variable = null;
                        break;
                }

                Port port = new Port();
                port.Name = variable.Name;
                port.Direction = DirectionEnum.Inout;
                port.Variable = variable;

                if (!word.Active)
                {
                    // skip
                }
                else if (word.Prototype)
                {
                    if (!portNameSpace.Variables.ContainsKey(variable.Name))
                    {
                        portNameSpace.Variables.Add(variable.Name, variable);
                    }

                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        word.AddError("port name duplicated");
                    }
                    else
                    {
                        portNameSpace.Ports.Add(port.Name, port);
                        portNameSpace.PortsList.Add(port);
                    }
                }
                else
                {
                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        port = portNameSpace.Ports[port.Name];
                    }
                }
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

        public static void ParseTfInputDeclaration(WordScanner word, Module module, IPortNameSpace portNameSpace, Attribute attribute)
        {
            //            tf_input_declaration::= input[reg][signed][range] list_of_port_identifiers
            //                          | input[task_port_type] list_of_port_identifiers
            //            task_port_type::= time | real | realtime | integer

            portVariableType varType = portVariableType.wire;

            if (word.Text != "input") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            bool signed = false;
            Range range = null;

            if (word.Text == "time")
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

                if (word.Text == "signed")
                {
                    signed = true;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                }

                if (word.GetCharAt(0) == '[')
                {
                    range = Range.ParseCreate(word, module);
                }
            }
            else
            {
                if (word.Text == "signed")
                {
                    signed = true;
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                }

                if (word.GetCharAt(0) == '[')
                {
                    range = Range.ParseCreate(word, module);
                }
            }

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) return;

                Variable variable;
                switch (varType)
                {
                    case portVariableType.wire:
                        Net val = new Net();
                        val.Name = word.Text;
                        val.Range = range;
                        val.Signed = signed;
                        variable = val;
                        word.Color(CodeDrawStyle.ColorType.Net);
                        break;
                    case portVariableType.reg:
                        Reg reg = new Reg(word.Text, range, signed);
                        variable = reg;
                        word.Color(CodeDrawStyle.ColorType.Register);
                        break;
                    case portVariableType.integer:
                        Integer integer = new Integer(word.Text);
                        variable = integer;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.real:
                        Real real = new Real(word.Text);
                        variable = real;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.realtime:
                        RealTime realtime = new RealTime(word.Text);
                        variable = realtime;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    case portVariableType.time:
                        Time time = new Time(word.Text);
                        variable = time;
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                    default:
                        System.Diagnostics.Debugger.Break();
                        variable = null;
                        break;

                }
                Port port = new Port();
                port.Name = variable.Name;
                port.Direction = DirectionEnum.Input;
                port.Variable = variable;

                if (!word.Active)
                {
                    // skip
                }
                else if (word.Prototype)
                {
                    if (!portNameSpace.Variables.ContainsKey(variable.Name))
                    {
                        portNameSpace.Variables.Add(variable.Name, variable);
                    }

                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        word.AddError("port name duplicated");
                    }
                    else
                    {
                        portNameSpace.Ports.Add(port.Name, port);
                        portNameSpace.PortsList.Add(port);
                    }
                }
                else
                {
                    if (portNameSpace.Ports.ContainsKey(port.Name))
                    {
                        port = portNameSpace.Ports[port.Name];
                    }
                }
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

    }
}