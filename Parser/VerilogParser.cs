using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;

namespace pluginVerilog.Parser
{
    public class VerilogParser : codeEditor.CodeEditor.DocumentParser
    {
        public VerilogParser(
            codeEditor.CodeEditor.CodeDocument document, 
            string id, codeEditor.Data.Project project,
            codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode
            ) : base(document, id, project,parseMode)
        {
            parsedDocument = new Verilog.ParsedDocument(project, id, document.EditID);
            word = new Verilog.WordScanner(this.document, parsedDocument,false);
        }

        public VerilogParser(
            codeEditor.CodeEditor.CodeDocument document,
            string moduleName,
            Dictionary<string, Verilog.Expressions.Expression> parameterOverrides,
            string id, codeEditor.Data.Project project,
            codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode
            ) : base(document, id, project, parseMode)
        {
            this.moduleName = moduleName;
            this.parameterOverrides = parameterOverrides;
            parsedDocument = new Verilog.ParsedDocument(project, id, document.EditID);
            word = new Verilog.WordScanner(this.document, parsedDocument, false);
        }

        public Verilog.WordScanner word;
        private Verilog.ParsedDocument parsedDocument = null;

        public override ParsedDocument ParsedDocument { get { return parsedDocument as codeEditor.CodeEditor.ParsedDocument; } }

        private string moduleName;
        private Dictionary<string, Verilog.Expressions.Expression> parameterOverrides;

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
//        public static System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public override void Parse()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();

            word.GetFirst();
            while (!word.Eof)
            {
                if (word.Text == "module")
                {
                    Verilog.Module module;
                    if (ParseMode == ParseModeEnum.LoadParse)
                    {
                        if(moduleName == null)
                        {
                            module = Verilog.Module.Create(word, null, parsedDocument.ItemID, true);
                        }
                        else
                        {
                            module = Verilog.Module.Create(word, moduleName,parameterOverrides, null, parsedDocument.ItemID, true);
                        }
                    }
                    else
                    {
                        if (moduleName == null)
                        {
                            module = Verilog.Module.Create(word, null, parsedDocument.ItemID, false);
                        }
                        else
                        {
                            module = Verilog.Module.Create(word, moduleName, parameterOverrides, null, parsedDocument.ItemID, false);
                        }
                    }
                    if (!parsedDocument.Modules.ContainsKey(module.Name))
                    {
                        parsedDocument.Modules.Add(module.Name, module);
                    }
                    else
                    {
                        word.AddError("duplicated module name");
                    }
                }
                else
                {
                    word.MoveNext();
                }
            }
            word.Dispose();
            word = null;

            if (sw.ElapsedMilliseconds > 100)
            {
                System.Diagnostics.Debug.Print("parse0 " + sw.ElapsedMilliseconds.ToString() + "ms : "+ parsedDocument.ItemID );
            }
            sw.Stop();
        }
    }
}
