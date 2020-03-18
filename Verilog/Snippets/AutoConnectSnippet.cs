using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;

namespace pluginVerilog.Verilog.Snippets
{
    public class AutoConnectSnippet : codeEditor.CodeEditor.ToolItem
    {
        public AutoConnectSnippet() : base("autoConnect")
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

            Tools.AutoConnectForm form = new Tools.AutoConnectForm();
            codeEditor.Controller.ShowDialogForm(form);
        }


    }
}

