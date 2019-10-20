using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;

namespace pluginVerilog.Verilog.Snippets
{
    public class AutoFormatSnippet : codeEditor.CodeEditor.ToolItem
    {
        public AutoFormatSnippet() : base("autoFormat")
        {
        }

        public override void Apply(CodeDocument codeDocument)
        {
            codeEditor.Data.ITextFile itext = codeEditor.Controller.CodeEditor.GetTextFile();

            if (!(itext is Data.IVerilogRelatedFile)) return;
            var vfile = itext as Data.IVerilogRelatedFile;
            ParsedDocument parsedDocument = vfile.VerilogParsedDocument;
            if (parsedDocument == null) return;

            int index = codeDocument.CaretIndex;
            Module module = parsedDocument.GetModule(index);

            foreach(var inst in module.ModuleInstantiations.Values)
            {
                if(inst.BeginIndex<index && index < inst.LastIndex)
                {
                    writeModuleInstance(codeDocument, index, inst);
                    return;
                }
            }

//            NameSpace nameSpace = module.GetHierNameSpace(index);

//            var data = codeEditor.Controller.CodeEditor.
//            codeEditor.Controller.
        }

        private void writeModuleInstance(CodeDocument codeDocument,int index,ModuleItems.ModuleInstantiation moduleInstantiation)
        {
            string indent = (codeDocument as CodeEditor.CodeDocument).GetIndentString(index);

            codeDocument.CaretIndex = moduleInstantiation.BeginIndex;
            codeDocument.Replace(
                moduleInstantiation.BeginIndex,
                moduleInstantiation.LastIndex - moduleInstantiation.BeginIndex + 1,
                0,
                moduleInstantiation.ToString("\t")
                );
            codeDocument.SelectionStart = codeDocument.CaretIndex;
            codeDocument.SelectionLast = codeDocument.CaretIndex;
        }
    }
}

