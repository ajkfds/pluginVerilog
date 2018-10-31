using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog
{
    /*
    public class VerilogParser : codeEditor.CodeEditor.DocumentParser
    {
        public VerilogParser(ajkControls.Document document,string relativeFilePath,codeEditor.Data.Project project) : base(document,relativeFilePath,project)
        {
            parsedDocument = new ParsedVerilogDocument(project,relativeFilePath);
            word = new WordScanner(this.document,parsedDocument);
        }

        public WordScanner word;
        private ParsedVerilogDocument parsedDocument = null;
*/

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

/*        Dictionary<string, Module> Modules = new Dictionary<string, Module>();

        public override void Parse()
        {

            while (!word.Eof)
            {
                if (word.Text == "module")
                {
                    Module module = Module.Create((WordScanner)word,null);
                    parsedDocument.Modules.Add(module.Name, module);
                }
                else
                {
                    word.MoveNext();
                }
            }
            word.Dispose();
            ParsedDcument = parsedDocument;
            word = null;
        }
    }
    */
}
