using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class Module : NameSpace,IPortNameSpace,IModuleOrGeneratedBlock
    {
        protected Module() : base(null,null)
        {

        }

        private Dictionary<string, Variables.Port> ports = new Dictionary<string, Variables.Port>();
        public Dictionary<string, Variables.Port> Ports { get { return ports; } }
        private List<Variables.Port> portsList = new List<Variables.Port>();
        public List<Variables.Port> PortsList { get { return portsList; } }

        private List<string> portParameterNameList = new List<string>();
        public List<string> PortParameterNameList {  get { return portParameterNameList;  } }

        private Dictionary<string, Function> functions = new Dictionary<string, Function>();
        private Dictionary<string, Task> tasks = new Dictionary<string, Task>();
        private Dictionary<string, Class> classes = new Dictionary<string, Class>();
        private Dictionary<string, ModuleItems.ModuleInstantiation> moduleInstantiations = new Dictionary<string, ModuleItems.ModuleInstantiation>();
        public Dictionary<string, Function> Functions { get { return functions; } }
        public Dictionary<string, Task> Tasks { get { return tasks; } }
        public Dictionary<string, Class> Classes { get { return classes;  } }
        public Dictionary<string, ModuleItems.ModuleInstantiation> ModuleInstantiations { get { return moduleInstantiations; } }
        public string FileId { get; protected set; }
        private bool cellDefine = false;
        public bool CellDefine
        {
            get { return cellDefine; }
        }

        public static Module Create(WordScanner word, Attribute attribute, string fileId, bool protoType)
        {
            return Create(word, null, null, attribute, fileId, protoType);
        }
        public static Module Create(
            WordScanner word,
            string parameterOverrideModueName,
            Dictionary<string, Verilog.Expressions.Expression> parameterOverrides,
            Attribute attribute, string fileId, bool protoType
            )
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

            // module_keyword ( module | macromodule )
            if (word.Text != "module" && word.Text != "macromodule") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            Module module = new Module();

            module.Module = module;
            module.FileId = fileId;
            module.BeginIndex = word.RootIndex;
            if (word.CellDefine) module.cellDefine = true;
            word.MoveNext();


            // parse definitions
            Dictionary<string, Verilog.Macro> macroKeep = new Dictionary<string, Verilog.Macro>();
            foreach(var kvpair in word.RootParsedDocument.Macros)
            {
                macroKeep.Add(kvpair.Key, kvpair.Value);
            }


            if(!word.CellDefine && !protoType)
            {
                // protptype parse
                WordScanner prototypeWord = word.Clone();
                prototypeWord.Prototype = true;
                parseModuleItems(prototypeWord, parameterOverrideModueName, parameterOverrides, null, module);
                prototypeWord.Dispose();
                prototypeWord = null;

                // parse
                word.RootParsedDocument.Macros = macroKeep;
                parseModuleItems(word, parameterOverrideModueName, parameterOverrides, null, module);
            }
            else
            {
                // parse prototype only
                word.Prototype = true;
                parseModuleItems(word, parameterOverrideModueName, parameterOverrides, null, module);
                word.Prototype = false;
            }

            // endmodule keyword
            if (word.Text == "endmodule")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                module.LastIndex = word.RootIndex;

                word.AppendBlock(module.BeginIndex, module.LastIndex);
                word.MoveNext();
                return module;
            }
       
            {
                word.AddError("endmodule expected");
            }

            return module;
        }

        protected static void parseModuleItems(
            WordScanner word,
            string parameterOverrideModueName,
            Dictionary<string, Verilog.Expressions.Expression> parameterOverrides,
            Attribute attribute, Module module)
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

            // module_identifier
            module.Name = word.Text;
            word.Color(CodeDrawStyle.ColorType.Identifier);
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
                        while (!word.Eof)
                        {
                            if (word.Text == "parameter") Verilog.Variables.Parameter.ParseCreateDeclarationForPort(word, module, null);
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

                if(module.Name == parameterOverrideModueName)
                {
                    foreach(var vkp in parameterOverrides)
                    {
                        if(module.Parameters.ContainsKey(vkp.Key))
                        {
                            module.Parameters.Remove(vkp.Key);
                            Verilog.Variables.Parameter param = new Variables.Parameter();
                            param.Name = vkp.Key;
                            param.Expression = vkp.Value;
                            module.Parameters.Add(param.Name, param);
                        }
                        else
                        {
                            System.Diagnostics.Debugger.Break();
                        }
                    }
                }

                if (word.Eof || word.Text == "endmodule") break;
                if (word.Text == "(")
                {
                    parseListOfPorts_ListOfPortsDeclarations(word, module);
                } // list_of_ports or list_of_posrt_declarations

                if (word.Eof || word.Text == "endmodule") break;

                if (word.GetCharAt(0) == ';')
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

            if (!word.Prototype)
            {
                checkVariablesUseAndDriven(word,module);
            }

            return;
        }

        protected static void checkVariablesUseAndDriven(WordScanner word, NameSpace nameSpace)
        {
            foreach (var variable in nameSpace.Variables.Values)
            {
                if (variable.DefinedReference == null) continue;
                if (variable.AssignedReferences.Count == 0)
                {
                    if (variable.UsedReferences.Count == 0)
                    {
                        word.AddHint(variable.DefinedReference, "undriven & unused");
                    }
                    else
                    {
                        word.AddHint(variable.DefinedReference, "undriven");
                    }
                }
                else
                {
                    if (variable.UsedReferences.Count == 0)
                    {
                        word.AddHint(variable.DefinedReference, "unused");
                    }
                }
            }
        }

        protected static void parseListOfPorts_ListOfPortsDeclarations(WordScanner word, Module module)
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
                while (!word.Eof)
                {
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

                while (!word.Eof && word.Text != ")")
                {
                    Verilog.Variables.Port port = Verilog.Variables.Port.Create(word, null);

                    if (port == null)
                    {
                        word.AddError("illegal port identifier");
                        return;
                    }

                    if (!word.Active)
                    {

                    }
                    else if (word.Prototype)
                    {
                        if (module.Ports.ContainsKey(port.Name))
                        {
                            word.AddError("duplicated port name");
                        }
                        else
                        {
                            module.Ports.Add(port.Name, port);
                            module.PortsList.Add(port);
                        }
                    }
                    else
                    {
                        if (module.Ports.ContainsKey(port.Name))
                        {
                            port = module.Ports[port.Name];
                        }
                    }

                    if (word.Text == ",")
                    {
                        word.MoveNext();
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
        }

        /*
        module_item ::= module_or_generate_item
            | port_declaration ;
            | { attribute_instance } generated_instantiation 
            | { attribute_instance } local_parameter_declaration
            | { attribute_instance } parameter_declaration
            | { attribute_instance } specify_block 
            | { attribute_instance } specparam_declaration  
        module_or_generate_item ::=   { attribute_instance } module_or_generate_item_declaration 
            | { attribute_instance } parameter_override 
            | { attribute_instance } continuous_assign
            | { attribute_instance } gate_instantiation
            | { attribute_instance } udp_instantiation 
            | { attribute_instance } module_instantiation 
            | { attribute_instance } initial_construct 
            | { attribute_instance } always_construct  
        module_or_generate_item_declaration ::=   net_declaration 
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
        private static void parseModuleItems(WordScanner word, Module module)
        {
            while (!word.Eof)
            {
                switch (word.Text)
                {
                    case "(*":
                        Attribute attribute = Attribute.ParseCreate(word);
                        break;
                    case "endmodule":
                        return;
                    // port_declaration
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
                    // module_or_generate_item_declaration
                    case "reg":
                        Verilog.Variables.Reg.ParseCreateFromDeclaration(word, module);
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
                        Verilog.Variables.Net.ParseCreateFromDeclaration(word, module);
                        break;
                    case "trireg":
                        Verilog.Variables.Trireg.ParseCreateFromDeclaration(word, module);
                        break;
                    case "integer":
                        Verilog.Variables.Integer.ParseCreateFromDeclaration(word, module);
                        break;
                    case "real":
                        Verilog.Variables.Real.ParseCreateFromDeclaration(word, module);
                        break;
                    case "realtime":
                        Verilog.Variables.RealTime.ParseCreateFromDeclaration(word, module);
                        break;
                    case "time":
                        Verilog.Variables.Time.ParseCreateFromDeclaration(word, module);
                        break;
                    case "event":
                        Verilog.Variables.Event.ParseCreateFromDeclaration(word, module);
                        break;
                    case "genvar":
                        Verilog.Variables.Genvar.ParseCreateFromDeclaration(word, module);
                        break;
                    // always_construct
                    case "always":
                        ModuleItems.AlwaysConstruct always = ModuleItems.AlwaysConstruct.ParseCreate(word, module);
                        break;
                    // initial_construct
                    case "initial":
                        ModuleItems.InitialConstruct initial = ModuleItems.InitialConstruct.ParseCreate(word, module);
                        break;
                    // parameter_declaration
                    case "parameter":
                    // local_parameter_declaration
                    case "localparam":
                        Verilog.Variables.Parameter.ParseCreateDeclaration(word, module, null);
                        break;
                    // continuous_assign
                    case "assign":
                        ModuleItems.ContinuousAssign continuousAssign = ModuleItems.ContinuousAssign.ParseCreate(word, module);
                        break;
                    // specify_block
                    case "specify":
                        // specify_block::= specify { specify_item } endspecify
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        while (!word.Eof && word.Text !="endspecify")
                        {
                            word.MoveNext();
                        }
                        if (word.Text == "endspecify")
                        {
                            word.Color(CodeDrawStyle.ColorType.Keyword);
                            word.MoveNext();
                        }
                        break;
                    // gate_instantiation
                    case "cmos":
                    case "rcmos":
                    case "bufif0":
                    case "bufif1":
                    case "notif0":
                    case "notif1":
                    case "nmos":
                    case "pmos":
                    case "rnmos":
                    case "rpmos":
                    case "and":
                    case "nand":
                    case "or":
                    case "nor":
                    case "xor":
                    case "xnor":
                    case "buf":
                    case "not":
                    case "tranif0":
                    case "tranif1":
                    case "rtranif0":
                    case "rtranif1":
                    case "tran":
                    case "rtran":
                    case "pullup":
                    case "pulldown":
                        ModuleItems.GateInstantiation gate = ModuleItems.GateInstantiation.ParseCreate(word, module);
                        break;
                    case "defparam":
                        ModuleItems.ParameterOverride.Parse(word, module);
                        break;
                    case "function":
                        Function.Parse(word, module);
                        break;
                    case "task":
                        Task.Parse(word, module);
                        break;
                    case "generate":
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
                        ParseGenerateItems(word, module);
                        if(word.Text != "endgenerate")
                        {
                            word.AddError("endgenerate expected");
                        }
                        else
                        {
                            word.Color(CodeDrawStyle.ColorType.Keyword);
                            word.MoveNext();
                        }
                        break;
                    case "": // blank at include return
                        word.MoveNext();
                        break;
                    default:
                        if (word.SystemVerilog)
                        {
                            if(word.Text == "class")
                            {
                                Verilog.Class.Parse(word, module);
                                break;
                            }
                        }

                        ModuleItems.ModuleInstantiation.Parse(word, module);
                        break;
                }
            }
        }

        /*
        generated_instantiation ::= (From Annex A -A.4.2) generate { generate_item } endgenerate
        generate_item_or_null ::= generate_item | ;  
        generate_item ::=   generate_conditional_statement | generate_case_statement | generate_loop_statement | generate_block | module_or_generate_item  

         */
        public static void ParseGenerateItems(WordScanner word, IModuleOrGeneratedBlock module)
        {
            while (!word.Eof)
            {
                if (!ParseGenerateItem(word, module)) break;
            }
        }

        public static bool ParseGenerateItem(WordScanner word, IModuleOrGeneratedBlock module)
        {
            switch (word.Text)
            {
                case "(*":
                    Attribute attribute = Attribute.ParseCreate(word);
                    break;
                case "endgenerate":
                    return false;
                case "end":
                    return false;
                case "endmodule":
                    word.AddError("endgenerate expected");
                    return false;

                case "if":
                    Generate.ParseGenerateConditionalStatement(word, module);
                    break;
                case "case":
                    Generate.ParseGenerateCaseStatement(word, module);
                    break;
                case "for":
                    Generate.ParseGenerateLoopStatement(word, module);
                    break;
                case "begin":
                    Generate.ParseGenerateBlockStatement(word, module);
                    break;

                case "input":
                case "output":
                case "inout":
                    if (module is Module)
                    {
                        Verilog.Variables.Port.ParsePortDeclaration(word, module as Module, null);
                        if (word.GetCharAt(0) != ';')
                        {
                            word.AddError("; expected");
                        }
                        else
                        {
                            word.MoveNext();
                        }
                    }
                    else
                    {
                        word.AddError("port in named block is not supported");
                        word.MoveNext();
                    }
                    break;
                case "reg":
                    Verilog.Variables.Reg.ParseCreateFromDeclaration(word, module as NameSpace);
                    break;
                case "trireg":
                    Verilog.Variables.Trireg.ParseCreateFromDeclaration(word, module as NameSpace);
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
                    Verilog.Variables.Net.ParseCreateFromDeclaration(word, module as NameSpace);
                    break;
                case "integer":
                    Verilog.Variables.Integer.ParseCreateFromDeclaration(word, module as NameSpace);
                    break;
                case "real":
                    Verilog.Variables.Real.ParseCreateFromDeclaration(word, module as NameSpace);
                    break;
                case "realtime":
                    Verilog.Variables.RealTime.ParseCreateFromDeclaration(word, module as NameSpace);
                    break;
                case "time":
                    Verilog.Variables.Time.ParseCreateFromDeclaration(word, module as NameSpace);
                    break;
                case "event":
                    Verilog.Variables.Event.ParseCreateFromDeclaration(word, module as NameSpace);
                    break;
                case "genvar":
                    Verilog.Variables.Genvar.ParseCreateFromDeclaration(word, module as NameSpace);
                    break;
                case "initial":
                    ModuleItems.InitialConstruct initial = ModuleItems.InitialConstruct.ParseCreate(word, module);
                    break;
                case "always":
                    ModuleItems.AlwaysConstruct always = ModuleItems.AlwaysConstruct.ParseCreate(word, module);
                    break;
                case "parameter":
                case "localparam":
                    Verilog.Variables.Parameter.ParseCreateDeclaration(word, module as NameSpace, null);
                    break;
                case "assign":
                    ModuleItems.ContinuousAssign continuousAssign = ModuleItems.ContinuousAssign.ParseCreate(word, module);
                    break;
                case "function":
                    Function.Parse(word, module);
                    break;
                case "task":
                    Task.Parse(word, module);
                    break;
                // gate_instantiation
                case "cmos":
                case "rcmos":
                case "bufif0":
                case "bufif1":
                case "notif0":
                case "notif1":
                case "nmos":
                case "pmos":
                case "rnmos":
                case "rpmos":
                case "and":
                case "nand":
                case "or":
                case "nor":
                case "xor":
                case "xnor":
                case "buf":
                case "not":
                case "tranif0":
                case "tranif1":
                case "rtranif0":
                case "rtranif1":
                case "tran":
                case "rtran":
                case "pullup":
                case "pulldown":
                    ModuleItems.GateInstantiation gate = ModuleItems.GateInstantiation.ParseCreate(word, module);
                    break;
                case "defparam":
                    ModuleItems.ParameterOverride.Parse(word, module);
                    break;
                default:
                    ModuleItems.ModuleInstantiation.Parse(word, module);
                    break;
            }

            return true;
        }

        public static List<string> UniqueKeywords = new List<string> {
            "module","endmodule",
            "function","endfunction",
            "task","endtask",
            "always","initial",
            "assign","specify","endspecify",
            "generate","endgenerate"
        };

    }
}
