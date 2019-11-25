using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;

namespace pluginVerilog.Parser
{
    //public class SystemVerilogParser : codeEditor.CodeEditor.DocumentParser
    //{
    //    public SystemVerilogParser(
    //        Data.SystemVerilogFile systemVerilogFile,
    //        codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode
    //    ) : base(systemVerilogFile, parseMode)
    //    {
    //        parsedDocument = new Verilog.ParsedDocument(project, id, document.EditID);
    //        word = new Verilog.WordScanner(this.document, parsedDocument,true);
    //    }

    //    public Verilog.WordScanner word;
    //    private Verilog.ParsedDocument parsedDocument = null;

    //    public override ParsedDocument ParsedDocument { get { return parsedDocument as codeEditor.CodeEditor.ParsedDocument; } }


    //    /*
    //    source_text ::= { description }
    //    description ::= module_declaration
    //                    | udp_declaration
    //    module_declaration ::=  { attribute_instance } module_keyword module_identifier [ module_parameter_port_list ]
    //                                [ list_of_ports ] ; { module_item }
    //                                endmodule
    //                            | { attribute_instance } module_keyword module_identifier [ module_parameter_port_list ]
    //                                [ list_of_port_declarations ] ; { non_port_module_item }
    //                                endmodule
    //    module_keyword ::= module | macromodule  
    //    */

    //    public override void Parse()
    //    {
    //        word.GetFirst();
    //        while (!word.Eof)
    //        {
    //            if (word.Text == "module")
    //            {
    //                Verilog.Module module = Verilog.Module.Create(word, null, parsedDocument.ItemID,false);
    //                if (!parsedDocument.Modules.ContainsKey(module.Name))
    //                {
    //                    parsedDocument.Modules.Add(module.Name, module);
    //                }
    //                else
    //                {
    //                    word.AddError("duplicated module name");
    //                }
    //            }else if(word.Text == "program")
    //            {
    //                SystemVerilog.ProgramItem module = SystemVerilog.ProgramItem.Create(word, null, parsedDocument.ItemID);
    //                if (!parsedDocument.Modules.ContainsKey(module.Name))
    //                {
    //                    parsedDocument.Modules.Add(module.Name, module);
    //                }
    //                else
    //                {
    //                    word.AddError("duplicated module name");
    //                }

    //            }
    //            else
    //            {
    //                word.MoveNext();
    //            }
    //        }
    //        word.Dispose();
    //        word = null;
    //    }

    //    /*
    //    // A.1.3 Module and primitive source text
    //    source_text ::= [ timeunits_declaration ] { description } 
    //    description ::=
    //        module_declaration 
    //        | udp_declaration 
    //        | interface_declaration  
    //        | program_declaration 
    //        | package_declaration 
    //        | { attribute_instance } package_item 
    //        | { attribute_instance } bind_directive

    //    module_nonansi_header ::=
    //        { attribute_instance } module_keyword [ lifetime ] module_identifier [ parameter_port_list ] list_of_ports ; 
    //    module_ansi_header ::=
    //        { attribute_instance } module_keyword [ lifetime ] module_identifier [ parameter_port_list ] [ list_of_port_declarations ] ;
    //    module_declaration ::=
    //        module_nonansi_header [ timeunits_declaration ] { module_item } endmodule [ : module_identifier ] 
    //        | module_ansi_header [ timeunits_declaration ] { non_port_module_item } endmodule [ : module_identifier ] 
    //        | { attribute_instance } module_keyword [ lifetime ] module_identifier ( .* ) ; [ timeunits_declaration ] { module_item } endmodule [ : module_identifier ] 
    //        | extern module_nonansi_header | extern module_ansi_header
    //    module_keyword ::= module | macromodule 

    //    interface_nonansi_header ::= { attribute_instance } interface [ lifetime ] interface_identifier [ parameter_port_list ] list_of_ports ; 
    //    interface_ansi_header ::= {attribute_instance } interface [ lifetime ] interface_identifier [ parameter_port_list ] [ list_of_port_declarations ] ;
    //    interface_declaration ::= 
    //        interface_nonansi_header [ timeunits_declaration ] { interface_item } endinterface [ : interface_identifier ] 
    //        | interface_ansi_header [ timeunits_declaration ] { non_port_interface_item } endinterface [ : interface_identifier ] 
    //        | { attribute_instance } interface interface_identifier ( .* ) ; [ timeunits_declaration ] { interface_item } endinterface [ : interface_identifier ] 
    //        | extern interface_nonansi_header | extern interface_ansi_header
        
    //    program_nonansi_header ::= 
    //        { attribute_instance } program [ lifetime ] program_identifier [ parameter_port_list ] list_of_ports ;
    //    program_ansi_header ::= 
    //        {attribute_instance } program [ lifetime ] program_identifier [ parameter_port_list ] [ list_of_port_declarations ] ; 
    //    */
    //}
}
