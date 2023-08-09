﻿using System;
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
            this.document = new CodeEditor.CodeDocument(verilogFile); // use verilog codedocument
            this.document.CopyTextOnlyFrom(verilogFile.CodeDocument);
            this.ParseMode = parseMode;
            this.TextFile = verilogFile as codeEditor.Data.TextFile;

            File = verilogFile;
            parsedDocument = new Verilog.ParsedDocument(verilogFile, parseMode);
            parsedDocument.Version = verilogFile.CodeDocument.Version;
            if (verilogFile is Data.VerilogFile && (verilogFile as Data.VerilogFile).SystemVerilog)
            {
                parsedDocument.SystemVerilog = true;
            }
             word = new Verilog.WordScanner(VerilogDocument, parsedDocument, parsedDocument.SystemVerilog);
        }

        public VerilogParser(
            Data.IVerilogRelatedFile verilogFile,
            string moduleName,
            Dictionary<string, Verilog.Expressions.Expression> parameterOverrides,
            codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode
            ) : base(verilogFile as codeEditor.Data.TextFile, parseMode)
        {
            this.document = new CodeEditor.CodeDocument(verilogFile); // use verilog codedocument

            this.document.CopyTextOnlyFrom(verilogFile.CodeDocument);
            this.ParseMode = parseMode;
            this.TextFile = verilogFile as codeEditor.Data.TextFile;
            this.parameterOverrides = parameterOverrides;
            this.targetModuleName = moduleName;

            File = verilogFile;
            parsedDocument = new Verilog.ParsedDocument(verilogFile, parseMode);
            if (verilogFile is Data.VerilogFile && (verilogFile as Data.VerilogFile).SystemVerilog)
            {
                parsedDocument.SystemVerilog = true;
            }
            parsedDocument.Version = verilogFile.CodeDocument.Version;
            parsedDocument.Instance = true;
            word = new Verilog.WordScanner(VerilogDocument, parsedDocument, parsedDocument.SystemVerilog);
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
        private string targetModuleName = null;

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


        /* Verilog 2001
            source_text ::= { description }
            description ::= module_declaration
                            | udp_declaration
            module_declaration ::=      { attribute_instance } module_keyword module_identifier [ module_parameter_port_list ]
                                        [ list_of_ports ] ; { module_item }
                                        endmodule

                                        | { attribute_instance } module_keyword module_identifier [ module_parameter_port_list ]
                                        [ list_of_port_declarations ] ; { non_port_module_item }
                                        endmodule
            module_keyword ::= module | macromodule
        */

        /* System Verilog 2012
            source_text ::= [ timeunits_declaration ] { description } 
            description ::= 
                module_declaration 
                | udp_declaration 
                | interface_declaration 
                | program_declaration         
                | package_declaration 
                | { attribute_instance } package_item 
                | { attribute_instance } bind_directive 
                | config_declaration 

        module_nonansi_header ::= 
            { attribute_instance } module_keyword [ lifetime ] module_identifier 
            { package_import_declaration } [ parameter_port_list ] list_of_ports ;

        module_ansi_header ::= 
            { attribute_instance } module_keyword [ lifetime ] module_identifier 
            { package_import_declaration }1 [ parameter_port_list ] [ list_of_port_declarations ] ;

        module_declaration ::= 
                module_nonansi_header [ timeunits_declaration ] { module_item } 
                endmodule [ : module_identifier ] 

                | module_ansi_header [ timeunits_declaration ] { non_port_module_item } 
                endmodule [ : module_identifier ] 

                | { attribute_instance } module_keyword [ lifetime ] module_identifier ( .* ) ;
                [ timeunits_declaration ] { module_item } endmodule [ : module_identifier ] 

                | extern module_nonansi_header 
                | extern module_ansi_header 
        module_keyword ::= module | macromodule

        */

        public override void Parse()
        {
            word.GetFirst();
            while (!word.Eof)
            {
                if (word.Text == "module")
                {
                    if(targetModuleName != null)
                    {
                        string moduleName = word.NextText;
                        if(moduleName != targetModuleName)
                        {
                            word.SkipToKeyword("endmodule");
                            word.MoveNext();
                            continue;
                        }
                    }


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
                        if(module.ModuleInstantiations.Count != 0) // prepare reparse (instanced module could have un-refferenced link)
                        {
                            module.ReperseRequested = true;
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
                        if (module.ReperseRequested) parsedDocument.ReparseRequested = true;
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
