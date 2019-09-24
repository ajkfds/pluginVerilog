using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;

namespace pluginVerilog.Verilog.Snippets
{
    public class AutoCorrectSnippet : codeEditor.CodeEditor.ToolItem
    {
        public AutoCorrectSnippet() : base("autoCorrect")
        {
        }

        public override void Apply(CodeDocument codeDocument)
        {
            codeEditor.Data.ITextFile itext = codeEditor.Global.Controller.CodeEditor.GetTextFile();

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

                }
            }

//            NameSpace nameSpace = module.GetHierNameSpace(index);

//            var data = codeEditor.Global.Controller.CodeEditor.
//            codeEditor.Global.Controller.
        }

        private void writeModuleInstance(CodeDocument codeDocument,int index,ModuleItems.ModuleInstantiation moduleInstantiation)
        {
            string indent = (codeDocument as CodeEditor.CodeDocument).GetIndentString(index);

            codeDocument.CaretIndex = moduleInstantiation.BeginIndex;
            codeDocument.Replace(
                moduleInstantiation.BeginIndex,
                moduleInstantiation.LastIndex - moduleInstantiation.BeginIndex,
                0,
                moduleInstantiation.ToString("\t")
                );
            codeDocument.SelectionStart = codeDocument.CaretIndex;
            codeDocument.SelectionLast = codeDocument.CaretIndex;
        }
    }
}

