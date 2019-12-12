using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Parser
{
    public class VerilogParser : codeEditor.CodeEditor.DocumentParser
    {
        public VerilogParser(
            Data.IVerilogRelatedFile verilogFile,
            codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode
            )
        {
            this.EditId = verilogFile.CodeDocument.EditID;
            this.document = new CodeEditor.CodeDocument(verilogFile); // use verilog codedocument
            this.document.CopyCharsFrom(verilogFile.CodeDocument);
            this.document.CopyLineIndexFrom(verilogFile.CodeDocument);
            this.ParseMode = parseMode;
            this.TextFile = verilogFile as codeEditor.Data.TextFile;

            File = verilogFile;
            parsedDocument = new Verilog.ParsedDocument(verilogFile);
            word = new Verilog.WordScanner(VerilogDocument, parsedDocument,false);
        }

        public VerilogParser(
            Data.IVerilogRelatedFile verilogFile,
            Dictionary<string, Verilog.Expressions.Expression> parameterOverrides,
            codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode
            ) : base(verilogFile as codeEditor.Data.TextFile, parseMode)
        {
            this.EditId = verilogFile.CodeDocument.EditID;
            this.document = new CodeEditor.CodeDocument(verilogFile); // use verilog codedocument
            this.document.CopyCharsFrom(verilogFile.CodeDocument);
            this.document.CopyLineIndexFrom(verilogFile.CodeDocument);
            this.ParseMode = parseMode;
            this.TextFile = verilogFile as codeEditor.Data.TextFile;

            this.parameterOverrides = parameterOverrides;
            File = verilogFile;
            parsedDocument = new Verilog.ParsedDocument(verilogFile);
            parsedDocument.Instance = true;
            word = new Verilog.WordScanner(VerilogDocument, parsedDocument, false);
        }

        public Verilog.WordScanner word;

        public CodeEditor.CodeDocument VerilogDocument
        {
            get
            {
                return Document as CodeEditor.CodeDocument;
            }
        }

        private Verilog.ParsedDocument parsedDocument = null;
        public override codeEditor.CodeEditor.ParsedDocument ParsedDocument { get { return parsedDocument as codeEditor.CodeEditor.ParsedDocument; } }
        public virtual Verilog.ParsedDocument VerilogParsedDocument { get { return parsedDocument; } }

        private Dictionary<string, Verilog.Expressions.Expression> parameterOverrides;

        private System.WeakReference<Data.IVerilogRelatedFile> fileRef;
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
            word.GetFirst();
            while (!word.Eof)
            {
                if (word.Text == "module")
                {
                    Verilog.Module module;
                    if (ParseMode == ParseModeEnum.LoadParse)
                    {
                        if (parameterOverrides == null)
                        {
                            module = Verilog.Module.Create(word, null, File, true);
                        }
                        else
                        {
                            module = Verilog.Module.Create(word, parameterOverrides, null, File, true);
                        }
                    }
                    else
                    {
                        if (parameterOverrides == null)
                        {
                            module = Verilog.Module.Create(word, null, File, false);
                        }
                        else
                        {
                            module = Verilog.Module.Create(word, parameterOverrides, null, File, false);
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
        }
    }
}
