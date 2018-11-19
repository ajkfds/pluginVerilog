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
            switch(word.Text)
            {
                case "input":
                    parseInputDeclaration(word, module , attribute, out variables);
                    if (variables.Count == 0) return;
                    foreach (Variable variable in variables)
                    {
                        Port port = new Port();
                        port.Name = variable.Name;
                        port.Direction = DirectionEnum.Input;
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
                    break;
                case "output":
                    parseOutputDeclaration(word, module, attribute, out variables);
                    if (variables.Count == 0) return;
                    foreach (Variable variable in variables)
                    {
                        Port port = new Port();
                        port.Name = variable.Name;
                        port.Direction = DirectionEnum.Output;
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
                    break;
                case "inout":
                    parseInoutDeclaration(word, module, attribute, out variables);
                    if (variables.Count == 0) return;
                    foreach (Variable variable in variables)
                    {
                        Port port = new Port();
                        port.Name = variable.Name;
                        port.Direction = DirectionEnum.Inout;
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

            if (!General.IsIdentifier(word.Text)) return;
            Net net = new Net();
            net.Name = word.Text;
            net.Range = range;
            word.Color(CodeDrawStyle.ColorType.Net);
            net.Signed = signed;
            if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
            variables.Add(net);
            word.MoveNext();

            while (!word.Eof & word.GetCharAt(0) == ',')
            {
                string next = word.NextText;
                if (next == "input" || next == "output" || next == "inout")
                {
                    break;
                }
                word.MoveNext(); // ,
                if (!General.IsIdentifier(word.Text)) break;
                net = new Net();
                net.Name = word.Text;
                net.Signed = signed;
                if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
                variables.Add(net);
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

            if (!General.IsIdentifier(word.Text)) return;
            Net net = new Net();
            net.Name = word.Text;
            net.Range = range;
            word.Color(CodeDrawStyle.ColorType.Net);
            net.Signed = signed;
            if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
            variables.Add(net);
            word.MoveNext();

            while (!word.Eof & word.GetCharAt(0) == ',')
            {
                string next = word.NextText;
                if(next == "input" || next == "output" || next == "inout")
                {
                    break;
                }

                word.MoveNext(); // ,
                if (!General.IsIdentifier(word.Text)) break;
                net = new Net();
                net.Name = word.Text;
                net.Signed = signed;
                if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
                variables.Add(net);
            }
        }
        private static void parseOutputDeclaration(WordScanner word, Module module, Attribute attribute, out List<Variable> variables)
        {
            // output_declaration ::=     output       [net_type] [signed][range] list_of_port_identifiers
            //                          | output [reg]            [signed][range] list_of_port_identifiers
            //                          | output  reg             [signed][range] list_of_variable_port_identifiers
            //                          | output [output_variable_type]           list_of_port_identifiers
            //                          | output output_variable_type             list_of_variable_port_identifiers 
            // output_variable_type ::= integer | time  
            variables = new List<Variable>();
            if (word.Text != "output") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            bool signed = false;



            Net.NetTypeEnum? netType = parseNetType(word);
            bool reg = false;
            if (netType == null)
            {
                switch (word.Text)
                {
                    case "reg":
                        reg = true;
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
                        break;
                    case "integer":
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
                        break;
                    case "time":
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
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

            if (!General.IsIdentifier(word.Text)) return;
            Net net = new Net();
            net.Name = word.Text;
            net.Range = range;
            net.Signed = signed;
            if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
            variables.Add(net);
            word.Color(CodeDrawStyle.ColorType.Net);
            word.MoveNext();

            if(word.Text == "=")
            {
                word.MoveNext();
                Expressions.Expression consantt = Expressions.Expression.ParseCreate(word, module);
            }

            while (!word.Eof & word.GetCharAt(0) == ',')
            {
                string next = word.NextText;
                if (next == "input" || next == "output" || next == "inout")
                {
                    break;
                }
                word.MoveNext(); // ,
                if (!General.IsIdentifier(word.Text)) break;
                net = new Net();
                net.Name = word.Text;
                net.Signed = signed;
                if (netType != null) net.NetType = (Net.NetTypeEnum)netType;
                variables.Add(net);
                word.Color(CodeDrawStyle.ColorType.Net);
                word.MoveNext();
            }
        }

        private static Net.NetTypeEnum? parseNetType(WordScanner word)
        {
            // supply0 | supply1 | tri | triand  | trior | tri0 | tri1 | wire  | wand   | wor
            switch (word.Text)
            {
                case "supply0":
                    return Net.NetTypeEnum.Supply0;
                case "supply1":
                    return Net.NetTypeEnum.Supply1;
                case "tri":
                    return Net.NetTypeEnum.Tri;
                case "triand":
                   return Net.NetTypeEnum.Supply0;
                case "trior":
                    return Net.NetTypeEnum.Supply0;
                case "tri0":
                    return Net.NetTypeEnum.Supply0;
                case "tri1":
                    return Net.NetTypeEnum.Supply0;
                case "wire":
                    return Net.NetTypeEnum.Supply0;
                case "wand":
                    return Net.NetTypeEnum.Supply0;
                case "wor":
                    return Net.NetTypeEnum.Supply0;
                default:
                    return null;
            }
        }

    }
}