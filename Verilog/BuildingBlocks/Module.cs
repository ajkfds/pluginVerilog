using System;
using System.Collections.Generic;

namespace pluginVerilog.Verilog.BuildingBlocks
{
    public class Module : BuildingBlock, IModuleOrInterface, IPortNameSpace , IBuildingBlockWithModuleInstance
    {
        protected Module() : base(null, null)
        {

        }

        // IModuleOrInterfaceOrProfram

        // Port
        private Dictionary<string, DataObjects.Port> ports = new Dictionary<string, DataObjects.Port>();
        public Dictionary<string, DataObjects.Port> Ports { get { return ports; } }
        private List<DataObjects.Port> portsList = new List<DataObjects.Port>();
        public List<DataObjects.Port> PortsList { get { return portsList; } }

        public WordReference NameReference;
        private List<string> portParameterNameList = new List<string>();
        public List<string> PortParameterNameList { get { return portParameterNameList; } }

        // Module
        private Dictionary<string, ModuleItems.ModuleInstantiation> moduleInstantiations = new Dictionary<string, ModuleItems.ModuleInstantiation>();
        public Dictionary<string, ModuleItems.ModuleInstantiation> ModuleInstantiations { get { return moduleInstantiations; } }


        private WeakReference<Data.IVerilogRelatedFile> fileRef;
        public Data.IVerilogRelatedFile File
        {
            get
            {
                Data.IVerilogRelatedFile ret;
                if (!fileRef.TryGetTarget(out ret)) return null;
                return ret;
            }
            protected set
            {
                fileRef = new WeakReference<Data.IVerilogRelatedFile>(value);
            }
        }

        public string FileId { get; protected set; }
        private bool cellDefine = false;
        public bool CellDefine
        {
            get { return cellDefine; }
        }


        //public static Module ParseCreate(
        //    WordScanner word, 
        //    Dictionary<string, Expressions.Expression> parameterOverrides,
        //    Data.IVerilogRelatedFile file,
        //    bool protoType)
        //{

        //    /* # SystemVerilog

        //        module_declaration ::= 
        //              module_nonansi_header [ timeunits_declaration ] { module_item } "endmodule" [ ":" module_identifier ] 
        //            | module_ansi_header [ timeunits_declaration ] { non_port_module_item } "endmodule" [ ":" module_identifier ] 
        //            | { attribute_instance } module_keyword [ lifetime ] module_identifier "( .* ) ;"  [ timeunits_declaration ] { module_item } endmodule [ : module_identifier ] 
        //            | "extern" module_nonansi_header 
        //            | "extern" module_ansi_header

        //        module_nonansi_header ::= 
        //            { attribute_instance } module_keyword [ lifetime ] module_identifier { package_import_declaration } [ parameter_port_list ] list_of_ports ;
        //        module_ansi_header ::= 
        //            { attribute_instance } module_keyword [ lifetime ] module_identifier { package_import_declaration } [ parameter_port_list ] [ list_of_port_declarations ] ;

        //        module_keyword ::=
        //            "module" | "macromodule"         
        //    */

        //    // module_keyword
        //    if (word.Text != "module" && word.Text != "macromodule") System.Diagnostics.Debugger.Break();
        //    word.Color(CodeDrawStyle.ColorType.Keyword);
        //    Module module = new Module();

        //    module.BuildingBlock = module;
        //    module.File = file;
        //    module.BeginIndex = word.RootIndex;
        //    if (word.CellDefine) module.cellDefine = true;

        //    word.MoveNext();

        //    // [ lifetime ]
        //    if(word.Text == "static")
        //    {
        //        word.Color(CodeDrawStyle.ColorType.Keyword);
        //        word.MoveNext();
        //    } else if(word.Text == "automatic")
        //    {
        //        word.Color(CodeDrawStyle.ColorType.Keyword);
        //        word.MoveNext();
        //    }

        //    // parse definitions
        //    Dictionary<string, Macro> macroKeep = new Dictionary<string, Macro>();
        //    foreach (var kvpair in word.RootParsedDocument.Macros)
        //    {
        //        macroKeep.Add(kvpair.Key, kvpair.Value);
        //    }


        //    if (!word.CellDefine && !protoType)
        //    {
        //        // protptype parse
        //        WordScanner prototypeWord = word.Clone();
        //        prototypeWord.Prototype = true;
        //        parseModuleItems(prototypeWord, parameterOverrides, null, module);
        //        prototypeWord.Dispose();

        //        // parse
        //        word.RootParsedDocument.Macros = macroKeep;
        //        parseModuleItems(word, parameterOverrides, null, module);
        //    }
        //    else
        //    {
        //        // parse prototype only
        //        word.Prototype = true;

        //        parseModuleItems(word, parameterOverrides, null, module);
        //        word.Prototype = false;
        //    }

        //    // endmodule keyword
        //    if (word.Text == "endmodule")
        //    {
        //        word.Color(CodeDrawStyle.ColorType.Keyword);
        //        module.LastIndex = word.RootIndex;

        //        word.AppendBlock(module.BeginIndex, module.LastIndex);
        //        word.MoveNext();
        //        return module;
        //    }

        //    {
        //        word.AddError("endmodule expected");
        //    }

        //    return module;

        //}

        public static Module Create(WordScanner word, Attribute attribute, Data.IVerilogRelatedFile file, bool protoType)
        {
            return Create(word, null, attribute, file, protoType);
        }
        public static Module Create(
            WordScanner word,
            Dictionary<string, Expressions.Expression> parameterOverrides,
            Attribute attribute,
            Data.IVerilogRelatedFile file,
            bool protoType
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
            module.Parent = word.RootParsedDocument.Root;

            module.BuildingBlock = module;
            module.File = file;
            module.BeginIndexReference = word.CreateIndexReference();
            if (word.CellDefine) module.cellDefine = true;
            word.MoveNext();


            // parse definitions
            Dictionary<string, Macro> macroKeep = new Dictionary<string, Macro>();
            foreach (var kvpair in word.RootParsedDocument.Macros)
            {
                macroKeep.Add(kvpair.Key, kvpair.Value);
            }


            if (!word.CellDefine && !protoType)
            {
                // protptype parse
                WordScanner prototypeWord = word.Clone();
                prototypeWord.Prototype = true;
                parseModuleItems(prototypeWord, parameterOverrides, null, module);
                prototypeWord.Dispose();

                // parse
                word.RootParsedDocument.Macros = macroKeep;
                parseModuleItems(word, parameterOverrides, null, module);
            }
            else
            {
                // parse prototype only
                word.Prototype = true;
                parseModuleItems(word, parameterOverrides, null, module);
                word.Prototype = false;
            }

            // endmodule keyword
            if (word.Text == "endmodule")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                module.LastIndexReference = word.CreateIndexReference();

                word.AppendBlock(module.BeginIndexReference, module.LastIndexReference);
                word.MoveNext();
                return module;
            }

            {
                word.AddError("endmodule expected");
            }

            return module;
        }

        /*
        module_declaration  ::= { attribute_instance } module_keyword module_identifier [ module_parameter_port_list ]Foverride
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
        protected static void parseModuleItems(
            WordScanner word,
            //            string parameterOverrideModueName,
            Dictionary<string, Expressions.Expression> parameterOverrides,
            Attribute attribute, Module module)
        {

            // module_identifier
            module.Name = word.Text;
            word.Color(CodeDrawStyle.ColorType.Identifier);
            if (!General.IsIdentifier(word.Text))
            {
                word.AddError("illegal module name");
            }
            else
            {
                module.NameReference = word.GetReference();
            }
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
                            if (word.Text == "parameter") Verilog.DataObjects.Constants.Parameter.ParseCreateDeclarationForPort(word, module, null);
                            if (word.Text != ",")
                            {
                                if (word.Text == ")") break;
                                if (word.Text == ",") continue;

                                if (word.Prototype) word.AddPrototypeError("illegal separator");
                                // illegal
                                word.SkipToKeyword(",");
                                if (word.Text == "parameter") continue;
                                break;
                            }
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

                if (parameterOverrides != null)
                {
                    foreach (var vkp in parameterOverrides)
                    {
                        if (module.Parameters.ContainsKey(vkp.Key))
                        {
                            if (module.Parameters[vkp.Key].DefinitionRefrecnce != null)
                            {
                                //                                module.Parameters[vkp.Key].DefinitionRefrecnce.AddNotice("override " + vkp.Value.Value.ToString());
                                module.Parameters[vkp.Key].DefinitionRefrecnce.AddHint("override " + vkp.Value.Value.ToString());
                            }

                            module.Parameters.Remove(vkp.Key);
                            DataObjects.Constants.Parameter param = new DataObjects.Constants.Parameter();
                            param.Name = vkp.Key;
                            param.Expression = vkp.Value;
                            module.Parameters.Add(param.Name, param);
                        }
                        else
                        {
                            //System.Diagnostics.Debug.Print("undefed params "+module.File.Name +":" + vkp.Key );
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

                while (!word.Eof)
                {
                    if(!Items.ModuleItem.Parse(word, module))
                    {
                        if (word.Text == "endmodule") break;
                        word.AddError("illegal module item");
                        word.MoveNext();
                    }
                }
                //parseModuleItems(word, module);
                break;
            }

            if (!word.Prototype)
            {
                checkVariablesUseAndDriven(word, module);
            }

            //foreach (var variable in module.Variables.Values)
            //{
            //    if (variable.DefinedReference == null) continue;
            //    variable.UsedReferences.Clear();
            //}
            return;
        }

        private codeEditor.CodeEditor.AutocompleteItem newItem(string text, CodeDrawStyle.ColorType colorType)
        {
            return new codeEditor.CodeEditor.AutocompleteItem(text, CodeDrawStyle.ColorIndex(colorType), Global.CodeDrawStyle.Color(colorType));
        }
        public override void AppendAutoCompleteItem(List<codeEditor.CodeEditor.AutocompleteItem> items)
        {
            base.AppendAutoCompleteItem(items);

            foreach (ModuleItems.ModuleInstantiation mi in ModuleInstantiations.Values)
            {
                if (mi.Name == null) System.Diagnostics.Debugger.Break();
                items.Add(newItem(mi.Name, CodeDrawStyle.ColorType.Identifier));
            }
        }

        protected static void checkVariablesUseAndDriven(WordScanner word, NameSpace nameSpace)
        {
            foreach (var variable in nameSpace.Variables.Values)
            {
                if (variable.DefinedReference == null) continue;

                DataObjects.Variables.ValueVariable valueVar = variable as DataObjects.Variables.ValueVariable;
                if (valueVar == null) continue;

                if (valueVar.AssignedReferences.Count == 0)
                {
                    if (valueVar.UsedReferences.Count == 0)
                    {
                        word.AddNotice(variable.DefinedReference, "undriven & unused");
                    }
                    else
                    {
                        word.AddNotice(variable.DefinedReference, "undriven");
                    }
                }
                else
                {
                    if (valueVar.UsedReferences.Count == 0)
                    {
                        word.AddNotice(variable.DefinedReference, "unused");
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

            Verilog.DataObjects.Port.ParsePortDeclarations(word, module);

            if (word.Text == ")")
            {
                word.MoveNext();
            }
            else
            {
                word.AddError(") expected");
            }
        }

        /*
        ##Verilog 2001
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

        /*
         ## SystemVerilog 2012
        
        module_common_item::=
                      module_or_generate_item_declaration 
                    | interface_instantiation 
                    | program_instantiation 
                    | assertion_item 
                    | bind_directive 
                    | continuous_assign 
                    | net_alias 
                    | initial_construct 
                    | final_construct 
                    | always_construct 
                    | loop_generate_construct 
                    | conditional_generate_construct
        
        module_item ::= 
                      port_declaration ;
                    | non_port_module_item

        module_or_generate_item ::= 
                      { attribute_instance } parameter_override 
                    | { attribute_instance } gate_instantiation 
                    | { attribute_instance } udp_instantiation
                    | { attribute_instance } module_instantiation
                    | { attribute_instance } module_common_item

        module_or_generate_item_declaration ::= 
                      package_or_generate_item_declaration
                    | genvar_declaration
                    | clocking_declaration
                    | default clocking clocking_identifier ;

        package_or_generate_item_declaration ::= 
                      net_declaration 
                    | data_declaration 
                    | task_declaration 
                    | function_declaration 
                    | checker_declaration 
                    | dpi_import_export 
                    | extern_constraint_declaration 
                    | class_declaration 
                    | class_constructor_declaration 
                    | local_parameter_declaration ;
                    | parameter_declaration ;
                    | covergroup_declaration 
                    | overload_declaration 
                    | assertion_item_declaration 
                    | ;
        */

        /*
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
                        Verilog.DataObjects.Port.ParsePortDeclaration(word, module);
                        if (word.GetCharAt(0) != ';')
                        {
                            word.AddError("; expected");
                        }
                        else
                        {
                            word.MoveNext();
                        }
                        break;
                    // module_or_generate_item_declaration(V) / package_or_generate_item_declaration(SV)
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
                    case "real":
                    case "realtime":
                    case "time":
                    case "logic":
                    case "bit":
                    case "enum":
                        Verilog.Variables.Variable.ParseCreateFromDataDeclaration(word, module);
                        break;
                    case "event":
                        Verilog.Variables.Event.ParseCreateFromDeclaration(word, module);
                        break;
                    case "genvar":
                        Verilog.Variables.Genvar.ParseCreateFromDeclaration(word, module);
                        break;
                    // always_construct
                    case "always":
                    case "always_comb":
                    case "always_latch":
                    case "always_ff":
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
                        Verilog.DataObjects.Parameter.ParseCreateDeclaration(word, module, null);
                        break;
                    // continuous_assign
                    case "assign":
                        ModuleItems.ContinuousAssign continuousAssign = ModuleItems.ContinuousAssign.ParseCreate(word, module);
                        break;
                    // specify_block
                    case "specify":
                        // specify_block::= specify { specify_item } endspecify
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        while (!word.Eof && word.Text != "endspecify")
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
                    case "if":
                    case "case":
                    case "for":
                    case "begin":
                        // generate region can oimi on system verilog
                        if (!word.SystemVerilog)
                        {
                            word.AddError("generate requred");
                        }
                        ParseGenerateItems(word, module);
                        break;
                    case "generate":
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
                        ParseGenerateItems(word, module);
                        if (word.Text != "endgenerate")
                        {
                            word.AddError("endgenerate expected");
                        }
                        else
                        {
                            word.Color(CodeDrawStyle.ColorType.Keyword);
                            word.MoveNext();
                        }
                        break;
                    case "typedef":
                        Verilog.Variables.Typedef.ParseCreateFromDeclaration(word, module);
                        break;
                    case "": // blank at include return
                        word.MoveNext();
                        break;
                    case "class":
                        if (!word.SystemVerilog) word.AddError("SystemVerilog description");
                        //                        Verilog.Class.Parse(word, module);
                        break;
                    case ";":
                        word.AddError("illegal ;");
                        word.MoveNext();
                        break;
                    default:
                        if (module.Typedefs.ContainsKey(word.Text))
                        {
                            Variables.Variable type = module.Typedefs[word.Text].VariableType;
                            word.Color(CodeDrawStyle.ColorType.Identifier);
                            word.MoveNext();
                            if (type == null)
                            {
                                word.SkipToKeyword(";");
                                break;
                            }
                            Verilog.Variables.Variable.ParseCreateFromDataDeclaration(word, module, type);
                        }
                        else
                        {
                            ModuleItems.ModuleInstantiation.Parse(word, module);
                        }
                        break;
                }
            }
        }
        */

        /*
        generated_instantiation ::= (From Annex A -A.4.2) generate { generate_item } endgenerate
        generate_item_or_null ::= generate_item | ;  
        generate_item ::=   generate_conditional_statement | generate_case_statement | generate_loop_statement | generate_block | module_or_generate_item  

         */
        //public static void ParseGenerateItems(WordScanner word, NameSpace nameSpace)
        //{
        //    while (!word.Eof)
        //    {
        //        if (!ParseGenerateItem(word, nameSpace)) break;
        //    }
        //}

        //public static bool ParseGenerateItem(WordScanner word, NameSpace nameSpace)
        //{
        //    switch (word.Text)
        //    {
        //        case "(*":
        //            Attribute attribute = Attribute.ParseCreate(word);
        //            break;
        //        case "endgenerate":
        //            return false;
        //        case "end":
        //            return false;
        //        case "endmodule":
        //            return false;

        //        case "if":
        //            return Generate.ParseGenerateConditionalStatement(word, nameSpace);
        //        case "case":
        //            return Generate.ParseGenerateCaseStatement(word, nameSpace);
        //        case "for":
        //            return Generate.ParseGenerateLoopStatement(word, nameSpace);
        //        case "begin":
        //            return Generate.ParseGenerateBlockStatement(word, nameSpace);

        //        case "input":
        //        case "output":
        //        case "inout":
        //            if (nameSpace is Module)
        //            {
        //                Verilog.DataObjects.Port.ParsePortDeclaration(word, nameSpace as Module);
        //                if (word.GetCharAt(0) != ';')
        //                {
        //                    word.AddError("; expected");
        //                }
        //                else
        //                {
        //                    word.MoveNext();
        //                }
        //            }
        //            else
        //            {
        //                word.AddError("port in named block is not supported");
        //                word.MoveNext();
        //            }
        //            break;
        //        case "reg":
        //            Verilog.Variables.Reg.ParseCreateFromDeclaration(word, nameSpace as NameSpace);
        //            break;
        //        case "trireg":
        //            Verilog.Variables.Trireg.ParseCreateFromDeclaration(word, nameSpace as NameSpace);
        //            break;
        //        case "supply0":
        //        case "supply1":
        //        case "tri":
        //        case "triand":
        //        case "trior":
        //        case "tri0":
        //        case "tri1":
        //        case "wire":
        //        case "wand":
        //        case "wor":
        //            Verilog.Variables.Net.ParseCreateFromDeclaration(word, nameSpace as NameSpace);
        //            break;
        //        case "integer":
        //            Verilog.Variables.Integer.ParseCreateFromDeclaration(word, nameSpace as NameSpace);
        //            break;
        //        case "real":
        //            Verilog.Variables.Real.ParseCreateFromDeclaration(word, nameSpace as NameSpace);
        //            break;
        //        case "realtime":
        //            Verilog.Variables.RealTime.ParseCreateFromDeclaration(word, nameSpace as NameSpace);
        //            break;
        //        case "time":
        //            Verilog.Variables.Time.ParseCreateFromDeclaration(word, nameSpace as NameSpace);
        //            break;
        //        case "event":
        //            Verilog.Variables.Event.ParseCreateFromDeclaration(word, nameSpace as NameSpace);
        //            break;
        //        case "genvar":
        //            Verilog.Variables.Genvar.ParseCreateFromDeclaration(word, nameSpace as NameSpace);
        //            break;
        //        case "initial":
        //            ModuleItems.InitialConstruct initial = ModuleItems.InitialConstruct.ParseCreate(word, nameSpace);
        //            break;
        //        case "always":
        //            ModuleItems.AlwaysConstruct always = ModuleItems.AlwaysConstruct.ParseCreate(word, nameSpace);
        //            break;
        //        case "parameter":
        //        case "localparam":
        //            Verilog.DataObjects.Parameter.ParseCreateDeclaration(word, nameSpace as NameSpace, null);
        //            break;
        //        case "assign":
        //            ModuleItems.ContinuousAssign continuousAssign = ModuleItems.ContinuousAssign.ParseCreate(word, nameSpace);
        //            break;
        //        case "function":
        //            Function.Parse(word, nameSpace);
        //            break;
        //        case "task":
        //            Task.Parse(word, nameSpace);
        //            break;
        //        // gate_instantiation
        //        case "cmos":
        //        case "rcmos":
        //        case "bufif0":
        //        case "bufif1":
        //        case "notif0":
        //        case "notif1":
        //        case "nmos":
        //        case "pmos":
        //        case "rnmos":
        //        case "rpmos":
        //        case "and":
        //        case "nand":
        //        case "or":
        //        case "nor":
        //        case "xor":
        //        case "xnor":
        //        case "buf":
        //        case "not":
        //        case "tranif0":
        //        case "tranif1":
        //        case "rtranif0":
        //        case "rtranif1":
        //        case "tran":
        //        case "rtran":
        //        case "pullup":
        //        case "pulldown":
        //            ModuleItems.GateInstantiation gate = ModuleItems.GateInstantiation.ParseCreate(word, nameSpace);
        //            break;
        //        case "defparam":
        //            ModuleItems.ParameterOverride.Parse(word, nameSpace);
        //            break;
        //        default:
        //            ModuleItems.ModuleInstantiation.Parse(word, nameSpace);
        //            break;
        //    }

        //    return true;
        //}

        public static List<string> UniqueKeywords = new List<string> {
            "module","endmodule",
            "function","endfunction",
            "task","endtask",
            "always","initial",
            "assign","specify","endspecify",
            "generate","endgenerate"
        };

        /* #SystemVerilog
         * 
        source_text ::=
            [ timeunits_declaration ] { description } 

        description ::=
              module_declaration 
            | udp_declaration 
            | interface_declaration 
            | program_declaration 
            | package_declaration 
            | { attribute_instance } package_item 
            | { attribute_instance } bind_directive
        
        ## module

        module_declaration ::=
              module_nonansi_header [ timeunits_declaration ] { module_item } "endmodule" [ : module_identifier ] 
            | module_ansi_header [ timeunits_declaration ] { non_port_module_item } "endmodule" [ : module_identifier ] 
            | { attribute_instance } module_keyword [ lifetime ] module_identifier "( .* ) ;"  [ timeunits_declaration ] { module_item } "endmodule" [ : module_identifier ] 
            | extern module_nonansi_header 
            | extern module_ansi_header
        
        module_nonansi_header ::= 
            { attribute_instance } module_keyword [ lifetime ] module_identifier { package_import_declaration } [ parameter_port_list ] list_of_ports ";"
        module_ansi_header ::= 
            { attribute_instance } module_keyword [ lifetime ] module_identifier { package_import_declaration }1 [ parameter_port_list ] [ list_of_port_declarations ] ";"

        module_item ::= 
              port_declaration ";"
            | non_port_module_item

        non_port_module_item ::=
              generate_region 
            | module_or_generate_item 
            | specify_block 
            | { attribute_instance } specparam_declaration 
            | program_declaration 
            | module_declaration 
            | interface_declaration 
            | timeunits_declaration

        module_or_generate_item ::=
              { attribute_instance } parameter_override 
            | { attribute_instance } gate_instantiation 
            | { attribute_instance } udp_instantiation 
            | { attribute_instance } module_instantiation 
            | { attribute_instance } module_common_item

        module_common_item ::= 
              module_or_generate_item_declaration 
            | interface_instantiation 
            | program_instantiation 
            | assertion_item 
            | bind_directive 
            | continuous_assign 
            | net_alias 
            | initial_construct 
            | final_construct 
            | always_construct 
            | loop_generate_construct 
            | conditional_generate_construct
            | elaboration_system_task                 
         
        module_or_generate_item_declaration ::= 
              package_or_generate_item_declaration 
            | genvar_declaration 
            | clocking_declaration 
            | default clocking clocking_identifier ;
        
        ## program
        program_declaration ::= 
              program_nonansi_header [ timeunits_declaration ] { program_item } "endprogram" [ : program_identifier ] 
            | program_ansi_header [ timeunits_declaration ] { non_port_program_item } "endprogram" [ : program_identifier ] 
            | { attribute_instance } program program_identifier "( .* ) ;" [ timeunits_declaration ] { program_item } "endprogram" [ : program_identifier ] 
            | extern program_nonansi_header 
            | extern program_ansi_header

        program_nonansi_header ::= 
            { attribute_instance } "program" [ lifetime ] program_identifier { package_import_declaration } [ parameter_port_list ] list_of_ports ";"
        program_ansi_header ::= 
            {attribute_instance } "program" [ lifetime ] program_identifier { package_import_declaration } [ parameter_port_list ] [ list_of_port_declarations ] ";"

        program_item ::=
              port_declaration ";"
            | non_port_program_item 
        non_port_program_item ::= 
              { attribute_instance } continuous_assign 
            | { attribute_instance } module_or_generate_item_declaration 
            | { attribute_instance } initial_construct 
            | { attribute_instance } final_construct 
            | { attribute_instance } concurrent_assertion_item 
            | timeunits_declaration
            | program_generate_item 
        program_generate_item ::= 
              loop_generate_construct 
            | conditional_generate_construct 
            | generate_region 

        ## package

        package_declaration ::=
            { attribute_instance } "package" [ lifetime ] package_identifier ";" [ timeunits_declaration ] { { attribute_instance } package_item } "endpackage" [ : package_identifier ] 

        package_item ::= 
              package_or_generate_item_declaration 
            | anonymous_program 
            | package_export_declaration 
            | timeunits_declaration

        package_or_generate_item_declaration ::= 
              net_declaration 
            | data_declaration 
            | task_declaration 
            | function_declaration 
            | checker_declaration 
            | dpi_import_export 
            | extern_constraint_declaration 
            | class_declaration 
            | class_constructor_declaration 
            | local_parameter_declaration ;
            | parameter_declaration ;
            | covergroup_declaration 
            | overload_declaration 
            | assertion_item_declaration 
            | ;

        anonymous_program ::= "program" ; { anonymous_program_item } "endprogram"
        anonymous_program_item ::= 
            task_declaration 
            | function_declaration 
            | class_declaration 
            | covergroup_declaration 
            | class_constructor_declaration 
            | ";"

        ## class

        class_declaration ::=
            [ "virtual" ] "class" [ lifetime ] class_identifier [ parameter_port_list ] [ "extends" class_type [ ( list_of_arguments ) ] ] 
            [ "implements" interface_class_type { , interface_class_type } ] ; { class_item } "endclass" [ : class_identifier] 

        interface_class_type ::= ps_class_identifier [ parameter_value_assignment ] 
        
        class_item ::=
              { attribute_instance } class_property 
            | { attribute_instance } class_method 
            | { attribute_instance } class_constraint 
            | { attribute_instance } class_declaration 
            | { attribute_instance } covergroup_declaration 
            | local_parameter_declaration ";"
            | parameter_declaration7 ";"
            | ";"

        class_property ::= 
              { property_qualifier } data_declaration 
            | "const" { class_item_qualifier } data_type const_identifier [ "=" constant_expression ] ";"

        class_method ::= 
              { method_qualifier } task_declaration 
            | { method_qualifier } function_declaration 
            | "pure virtual" { class_item_qualifier } method_prototype ; 
            | "extern" { method_qualifier } method_prototype ;
            | { method_qualifier } class_constructor_declaration 
            | "extern" { method_qualifier } class_constructor_prototype
        
        class_constructor_prototype ::= 
            function new [ ( [ tf_port_list ] ) ] ;

        class_constraint ::= 
              constraint_prototype 
            | constraint_declaration

        property_qualifier ::= 
              random_qualifier 
            | class_item_qualifier 

        random_qualifier ::= 
              "rand"
            | "randc"

        method_qualifier ::= 
              [ "pure" ] "virtual"
            | class_item_qualifier

        class_item_qualifier ::= 
              "static"
            | "protected"
            | "local"

        method_prototype ::= 
              task_prototype 
            | function_prototype 

        ## interface
        interface_declaration ::=
            interface_nonansi_header [ timeunits_declaration ] { interface_item } 
            endinterface [ : interface_identifier ] 
            | interface_ansi_header [ timeunits_declaration ] { non_port_interface_item } 
            endinterface [ : interface_identifier ] 
            | { attribute_instance } interface interface_identifier ( .* ) ;
            [ timeunits_declaration ] { interface_item } 
            endinterface [ : interface_identifier ] 
            | extern interface_nonansi_header 
            | extern interface_ansi_header 

        interface_nonansi_header ::= 
            { attribute_instance } interface [ lifetime ] interface_identifier { package_import_declaration } [ parameter_port_list ] list_of_ports ;

        interface_ansi_header ::= 
            {attribute_instance } interface [ lifetime ] interface_identifier { package_import_declaration }1 [ parameter_port_list ] [ list_of_port_declarations ] ;

        interface_or_generate_item ::= 
              { attribute_instance } module_common_item 
            | { attribute_instance } modport_declaration 
            | { attribute_instance } extern_tf_declaration 

        extern_tf_declaration ::= 
              extern method_prototype ;
            | extern forkjoin task_prototype ;

        interface_item ::= 
              port_declaration ;
            | non_port_interface_item 
        
        non_port_interface_item ::= 
              generate_region 
            | interface_or_generate_item 
            | program_declaration 
            | interface_declaration 
            | timeunits_declaration

        modport_declaration ::= modport modport_item { , modport_item } ; // from A.2.9

        modport_item ::= modport_identifier ( modport_ports_declaration { , modport_ports_declaration } )

        modport_ports_declaration ::=
              { attribute_instance } modport_simple_ports_declaration 
            | { attribute_instance } modport_tf_ports_declaration 
            | { attribute_instance } modport_clocking_declaration 

        modport_clocking_declaration ::= clocking clocking_identifier 

        modport_simple_ports_declaration ::= 
            port_direction modport_simple_port { , modport_simple_port } 

        modport_simple_port ::= 
              port_identifier 
            | . port_identifier ( [ expression ] )

        modport_tf_ports_declaration ::= 
            import_export modport_tf_port { , modport_tf_port } 

        modport_tf_port ::= 
              method_prototype 
            | tf_identifier 

        import_export ::= import | export

        interface_instantiation ::=
            interface_identifier [ parameter_value_assignment ] hierarchical_instance { , hierarchical_instance } ;

        timeunits_declaration ::= 
              timeunit time_literal [ / time_literal ] ;
            | timeprecision time_literal ;
            | timeunit time_literal ; timeprecision time_literal ;
            | timeprecision time_literal ; timeunit time_literal ;

        ## checker
        checker_declaration ::=
            "checker" checker_identifier [ ( [ checker_port_list ] ) ] ; { { attribute_instance } checker_or_generate_item } "endchecker" [ : checker_identifier ] 
        checker_port_list ::= 
            checker_port_item {, checker_port_item}

        checker_port_item ::= 
            { attribute_instance } [ checker_port_direction ] property_formal_type formal_port_identifier {variable_dimension} [ "=" property_actual_arg ]

        checker_port_direction ::= 
            "input" | "output"

        checker_or_generate_item ::= 
              checker_or_generate_item_declaration 
            | initial_construct
            | always_construct 
            | final_construct
            | assertion_item
            | continuous_assign 
            | checker_generate_item

        checker_or_generate_item_declaration ::=
              [ rand ] data_declaration
            | function_declaration 
            | checker_declaration 
            | assertion_item_declaration
            | covergroup_declaration 
            | overload_declaration
            | genvar_declaration
            | clocking_declaration
            | default clocking clocking_identifier ;
            | default disable iff expression_or_dist ;
            | ;

        checker_generate_item6 ::= 
            loop_generate_construct
            | conditional_generate_construct
            | generate_region
            | elaboration_system_task
        checker_identifier ::= 
            identifier

        ## primitive
        // udp

        ## configuration
        config_declaration ::=
            "config" config_identifier ;
            { local_parameter_declaration ; } 
            design_statement 
            { config_rule_statement } 
            "endconfig" [ : config_identifier ] 

        design_statement ::= "design" { [ library_identifier . ] cell_identifier } ;

        config_rule_statement ::= 
            default_clause liblist_clause ;
            | inst_clause liblist_clause ;
            | inst_clause use_clause ;
            | cell_clause liblist_clause ;
            | cell_clause use_clause ;

        default_clause ::= "default"
        inst_clause ::= "instance" inst_name 
        inst_name ::= topmodule_identifier { . instance_identifier } 
        cell_clause ::= "cell" [ library_identifier . ] cell_identifier 
        liblist_clause ::= "liblist" {library_identifier} 

        use_clause ::=
            "use" [ library_identifier . ] cell_identifier [ ": config" ] 
            | "use" named_parameter_assignment { , named_parameter_assignment } [ ": config" ] 
            | "use" [ library_identifier . ] cell_identifier named_parameter_assignment 
            { , named_parameter_assignment } [ ": config" ] 

        ## declaration
        package_or_generate_item_declaration ::= 
              net_declaration 
            | data_declaration 
            | task_declaration 
            | function_declaration 
            | checker_declaration 
            | dpi_import_export 
            | extern_constraint_declaration 
            | class_declaration 
            | class_constructor_declaration 
            | local_parameter_declaration ;
            | parameter_declaration ;
            | covergroup_declaration 
            | overload_declaration 
            | assertion_item_declaration 
            | ;
         */

    }
}
