﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class Module : NameSpace
    {
        protected Module() : base(null,null)
        {

        }

        public Dictionary<string, Variables.Port> Ports = new Dictionary<string, Variables.Port>();
        public Dictionary<string, Function> Functions = new Dictionary<string, Function>();
        public Dictionary<string, Task> Tasks = new Dictionary<string, Task>();
        public Dictionary<string, ModuleItems.ModuleInstantiation> ModuleInstantiations = new Dictionary<string, ModuleItems.ModuleInstantiation>();

        public static Module Create(WordScanner word,Attribute attribute)
        {
            /*
            module_declaration  ::= { attribute_instance } module_keyword module_identifier [ module_parameter_port_list ]
                                        [ list_of_ports ] ; { module_item }
                                        endmodule
                                    | { attribute_instance } module_keyword module_identifier [ module_parameter_port_list ]
                                        [ list_of_port_declarations ] ; { non_port_module_item }
                                        endmodule
            module_keyword      ::= module | macromodule  
            module_identifier   ::= identifier

            module_parameter_port_list  ::= # ( parameter_declaration { , parameter_declaration } ) 
            list_of_ports ::= ( port { , port } )
            */

            Module module = new Module();
            module.Module = module;

            if (word.Text != "module" && word.Text != "macromodule") System.Diagnostics.Debugger.Break();
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext();

            module.Name = word.Text;
            word.Color((byte)Style.Color.Identifier);
            if (!General.IsIdentifier(word.Text)) word.AddError("illegal module name");
            word.MoveNext();

            while (true)
            {
                if (word.Eof || word.Text == "endmodule")
                {
                    break;
                }
                if (word.Text == "#")
                { // module_parameter_port_list
                    word.MoveNext();
                    do
                    {
                        if (word.GetCharAt(0) != '(')
                        {
                            word.AddError("( expected");
                            break;
                        }
                        word.MoveNext();
                        while(!word.Eof)
                        {
                            Verilog.Variables.Parameter.ParseCreateDeclarationForPort(word, module, null);
                            if (word.Text != ",") break;
                            word.MoveNext();
                        }

                        if (word.GetCharAt(0) != ')')
                        {
                            word.AddError(") expected");
                            break;
                        }
                        word.MoveNext();
                    } while (false);
                }

                if (word.Eof || word.Text == "endmodule") break;
                if (word.Text == "(")
                {
                    parseListOfPorts_ListOfPortsDeclarations(word, module);
                } // list_of_ports or list_of_posrt_declarations

                if (word.Eof || word.Text == "endmodule") break;

                if(word.GetCharAt(0) == ';')
                {
                    word.MoveNext();
                }
                else
                {
                    word.AddError("; expected");
                }

                parseModuleItems(word, module);
                break;
            }

            if (word.Text == "endmodule")
            {
                word.Color((byte)Style.Color.Keyword);
                word.MoveNext();
                return module;
            }
            else
            {
                word.AddError("endmodule expected");
            }
            return module;
        }

        private static void parseListOfPorts_ListOfPortsDeclarations(WordScanner word, Module module)
        {
            // list_of_ports::= (port { , port } )
            // list_of_port_declarations::= (port_declaration { , port_declaration } ) | ( )

            // port::= [port_expression] | .port_identifier( [port_expression] )
            // port_expression::= port_reference | { port_reference { , port_reference } }
            // port_reference::= port_identifier | port_identifier[constant_expression] | port_identifier[range_expression]

            // port_declaration::= { attribute_instance} inout_declaration | { attribute_instance} input_declaration | { attribute_instance} output_declaration  

            // inout_declaration::= inout[net_type][signed][range] list_of_port_identifiers
            // input_declaration ::= input[net_type][signed][range] list_of_port_identifiers
            // output_declaration ::= output[net_type][signed][range]      
            // list_of_port_identifiers | output[reg][signed][range]     
            // list_of_port_identifiers | output reg[signed][range]      
            // list_of_variable_port_identifiers | output[output_variable_type]      
            // list_of_port_identifiers | output output_variable_type list_of_variable_port_identifiers 
            // list_of_port_identifiers::= (From Annex A -A.2.3) port_identifier { , port_identifier }

            // list_of_variable_port_identifiers ::= port_identifier [ = constant_expression ]                               { , port_identifier [ = constant_expression ] }  

            if (word.Text != "(") return;
            word.MoveNext();
            if (word.Text == ")")
            {
                word.MoveNext();
                return;
            }

            if (word.Text == "input" || word.Text == "output" || word.Text == "inout")
            {
                Verilog.Variables.Port.ParsePortDeclaration(word, module, null);

                while (!word.Eof && word.Text != ")")
                {
                    if (word.Text == ",")
                    {
                        word.MoveNext();
                    }
                    else
                    {
                        word.AddError(", expected");
                    }
                    if (word.Text == "input" || word.Text == "output" || word.Text == "inout")
                    {
                        Verilog.Variables.Port.ParsePortDeclaration(word, module, null);
                    }
                    else
                    {
                        break;
                    }
                }
                if (word.Text == ")")
                {
                    word.MoveNext();
                }
                else
                {
                    word.AddError(") expected");
                }
            }
            else
            {
                Verilog.Variables.Port port = Verilog.Variables.Port.Create(word, null);
                if (port == null)
                {
                    word.AddError("illegal port identifier");
                    return;
                }
                module.Ports.Add(port.Name, port);

                while (!word.Eof && word.Text != ")")
                {
                    if (word.Text == ",")
                    {
                        word.MoveNext();
                    }
                    else
                    {
                        word.AddError(", expected");
                    }
                    if (word.Eof)
                    {
                        word.AddError("port identifier expected");
                        return;
                    }
                    if (word.Text == ")")
                    {
                        word.AddError("illegal ,");
                        break;
                    }

                    port = Verilog.Variables.Port.Create(word, null);
                    if (port == null)
                    {
                        word.AddError("illegal port identifier");
                        return;
                    }
                    else
                    {
                        module.Ports.Add(port.Name, port);
                    }
                }
                if (word.Text == ")")
                {
                    word.MoveNext();
                }
                else
                {
                    word.AddError(") expected");
                }
            }
        }

        private static void parseModuleItems(WordScanner word, Module module)
        {
            while (!word.Eof)
            {
                switch (word.Text)
                {
                    case "endmodule":
                        return;
                    case "input":
                    case "output":
                    case "inout":
                        Verilog.Variables.Port.ParsePortDeclaration(word, module, null);
                        if (word.GetCharAt(0) != ';')
                        {
                            word.AddError("; expected");
                        }
                        else
                        {
                            word.MoveNext();
                        }
                        break;
                    case "reg":
                        Verilog.Variables.Reg.ParseCreateDeclaration(word, module);
                        break;
                    case "supply0":
                    case "supply1":
                    case "tri":
                    case "triand":
                    case "trior":
                    case "tri0":
                    case "tri1":
                    case "wire":
                    case "wand":
                    case "wor":
                        Verilog.Variables.Net net = Verilog.Variables.Net.ParseCreateDeclaration(word, module);
                        break;
                    case "always":
                        ModuleItems.AlwaysConstruct always = ModuleItems.AlwaysConstruct.ParseCreate(word, module);
                        break;
                    case "parameter":
                    case "localparam":
                        Verilog.Variables.Parameter.ParseCreateDeclaration(word, module, null);
                        break;
                    case "assign":
                        ModuleItems.ContinuousAssign continuousAssign = ModuleItems.ContinuousAssign.ParseCreate(word, module);
                        break;
                    case "function":
                        Function.Parse(word, module);
                        break;
                    default:
                        ModuleItems.ModuleInstantiation.Parse(word, module);
                        break;
                }
            }
        }

        /*
        module_item ::= 
                        port_declaration ;
                        | module_or_generate_item
                        | { attribute_instance } generated_instantiation
                        | { attribute_instance } local_parameter_declaration
                        | { attribute_instance } parameter_declaration
                        | { attribute_instance } specify_block
                        | { attribute_instance } specparam_declaration

        non_port_module_item ::=
                                    { attribute_instance } module_or_generate_item
                                    | { attribute_instance } generated_instantiation
                                    | { attribute_instance } local_parameter_declaration
                                    | { attribute_instance } parameter_declaration
                                    | { attribute_instance } specify_block
                                    | { attribute_instance } specparam_declaration

        module_or_generate_item ::= 
                                    { attribute_instance } module_or_generate_item_declaration
                                    | { attribute_instance } parameter_override
                                    | { attribute_instance } continuous_assign
                                    | { attribute_instance } gate_instantiation
                                    | { attribute_instance } udp_instantiation
                                    | { attribute_instance } module_instantiation
                                    | { attribute_instance } initial_construct
                                    | { attribute_instance } always_construct  

        module_or_generate_item_declaration ::= net_declaration          
                                                | reg_declaration          
                                                | integer_declaration          
                                                | real_declaration          
                                                | time_declaration          
                                                | realtime_declaration          
                                                | event_declaration         
                                                | genvar_declaration          
                                                | task_declaration          
                                                | function_declaration  

        parameter_override ::= defparam list_of_param_assignments ;  
        */
    }
}
