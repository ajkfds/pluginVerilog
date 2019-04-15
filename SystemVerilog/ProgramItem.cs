using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.SystemVerilog
{
    public class ProgramItem : Verilog.Module
    {
        //protected ProgramItem() : base(null, null)
        //{

        //}

        public static new ProgramItem Create(Verilog.WordScanner word, Attribute attribute, string fileId)
        {
            /*
            program_nonansi_header ::= 
	            { attribute_instance } program [ lifetime ] program_identifier [ parameter_port_list ] list_of_ports ; 
            program_ansi_header ::= 
	            {attribute_instance } program [ lifetime ] program_identifier [ parameter_port_list ] [ list_of_port_declarations ] ; 
            program_declaration ::= 
	            program_nonansi_header [ timeunits_declaration ] { program_item } endprogram [ : program_identifier ] 
	            | program_ansi_header [ timeunits_declaration ] { non_port_program_item } endprogram [ : program_identifier ] 
	            | { attribute_instance } program program_identifier ( .* ) ; [ timeunits_declaration ] { program_item } endprogram [ : program_identifier ] 
	            | extern program_nonansi_header 
	            | extern program_ansi_header 
            */

            // module_keyword ( module | macromodule )
            if (word.Text != "program") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            ProgramItem program = new ProgramItem();
            program.Module = program;
            program.FileId = fileId;
            program.BeginIndex = word.RootIndex;
            word.MoveNext();


            // parse definitions
            Dictionary<string, string> macroKeep = new Dictionary<string, string>();
            foreach (var kvpair in word.RootParsedDocument.Macros)
            {
                macroKeep.Add(kvpair.Key, kvpair.Value);
            }

            Verilog.WordScanner prototypeWord = word.Clone();
            prototypeWord.Prototype = true;
            parseProgramItems(prototypeWord, null, program);

            // parse
            word.RootParsedDocument.Macros = macroKeep;
            parseProgramItems(word, null, program);

            // endmodule keyword
            if (word.Text == "endprogram")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                program.LastIndex = word.RootIndex;
                word.MoveNext();
                return program;
            }
            else
            {
                word.AddError("endmodule expected");
            }

            return program;
        }

        protected static void parseProgramItems(Verilog.WordScanner word, Attribute attribute, Verilog.Module module)
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

            return;
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
        private static void parseModuleItems(Verilog.WordScanner word, Verilog.Module module)
        {
            while (!word.Eof)
            {
                switch (word.Text)
                {
                    case "endprogram":
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
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.AddError("not allowes in program block");
                        word.MoveNext();
                        break;
                    // initial_construct
                    case "initial":
                        Verilog.ModuleItems.InitialConstruct initial = Verilog.ModuleItems.InitialConstruct.ParseCreate(word, module);
                        break;
                    // parameter_declaration
                    case "parameter":
                    // local_parameter_declaration
                    case "localparam":
                        Verilog.Variables.Parameter.ParseCreateDeclaration(word, module, null);
                        break;
                    // continuous_assign
                    case "assign":
                        Verilog.ModuleItems.ContinuousAssign continuousAssign = Verilog.ModuleItems.ContinuousAssign.ParseCreate(word, module);
                        break;
                    case "function":
                        Verilog.Function.Parse(word, module);
                        break;
                    case "task":
                        Verilog.Task.Parse(word, module);
                        break;
                    case "class":
                        Verilog.Class.Parse(word, module);
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
                    default:
                        Verilog.ModuleItems.ModuleInstantiation.Parse(word, module);
                        break;
                }
            }
        }

        /*
        generated_instantiation ::= (From Annex A -A.4.2) generate { generate_item } endgenerate
        generate_item_or_null ::= generate_item | ;  
        generate_item ::=   generate_conditional_statement | generate_case_statement | generate_loop_statement | generate_block | module_or_generate_item  

         */
        public static new void ParseGenerateItems(Verilog.WordScanner word, Verilog.Module module)
        {
            while (!word.Eof)
            {
                if (!ParseGenerateItem(word, module)) break;
            }
        }

        public static new bool ParseGenerateItem(Verilog.WordScanner word, Verilog.Module module)
        {
            switch (word.Text)
            {
                case "endgenerate":
                    return false;
                case "end":
                    return false;
                case "endmodule":
                    word.AddError("endgenerate expected");
                    return false;

                case "if":
                    Verilog.Generate.ParseGenerateConditionalStatement(word, module);
                    break;
                case "case":
                    Verilog.Generate.ParseGenerateCaseStatement(word, module);
                    break;
                case "for":
                    Verilog.Generate.ParseGenerateLoopStatement(word, module);
                    break;
                case "begin":
                    Verilog.Generate.ParseGenerateBlockStatement(word, module);
                    break;

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
                case "initial":
                    Verilog.ModuleItems.InitialConstruct initial = Verilog.ModuleItems.InitialConstruct.ParseCreate(word, module);
                    break;
                case "always":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.AddError("not allowes in program block");
                    word.MoveNext();
                    break;
                case "parameter":
                case "localparam":
                    Verilog.Variables.Parameter.ParseCreateDeclaration(word, module, null);
                    break;
                case "assign":
                    Verilog.ModuleItems.ContinuousAssign continuousAssign = Verilog.ModuleItems.ContinuousAssign.ParseCreate(word, module);
                    break;
                case "function":
                    Verilog.Function.Parse(word, module);
                    break;
                case "task":
                    Verilog.Task.Parse(word, module);
                    break;
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
                    Verilog.ModuleItems.GateInstantialtion.ParseCreate(word, module);
                    break;
                default:
                    Verilog.ModuleItems.ModuleInstantiation.Parse(word, module);
                    break;
            }

            return true;
        }

    }
}
