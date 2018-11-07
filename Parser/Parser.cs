﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;

namespace pluginVerilog.Parser
{
    public class Parser : codeEditor.CodeEditor.DocumentParser
    {
        public Parser(codeEditor.CodeEditor.CodeDocument document, string id, codeEditor.Data.Project project) : base(document, id, project)
        {
            parsedDocument = new Verilog.ParsedDocument(project, id, document.EditID);
            word = new Verilog.WordScanner(this.document, parsedDocument);

        }

        public Verilog.WordScanner word;
        private Verilog.ParsedDocument parsedDocument = null;

        public override ParsedDocument ParsedDocument { get { return parsedDocument as codeEditor.CodeEditor.ParsedDocument; } }


        /*
        source_text ::= { description }
        description ::= module_declaration
                        | udp_declaration
        module_declaration ::=  { attribute_instance } module_keyword module_identifier [ module_parameter_port_list ]
                                    [ list_of_ports ] ; { module_item }
                                    endmodule
                                | { attribute_instance } module_keyword module_identifier [ module_parameter_port_list ]
                                    [ list_of_port_declarations ] ; { non_port_module_item }
                                    endmodule
        module_keyword ::= module | macromodule  
        */

        public override void Parse()
        {
            while (!word.Eof)
            {
                if (word.Text == "module")
                {
                    Verilog.Module module = Verilog.Module.Create(word, null);
                    parsedDocument.Modules.Add(module.Name, module);
                }
                else
                {
                    word.MoveNext();
                }
            }
            word.Dispose();
            word = null;
        }
    }
}
