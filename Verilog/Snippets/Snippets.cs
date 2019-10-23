using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Snippets
{
    public static class Snippets
    {
        public static void GetCurrentTaregt()
        {
            Data.IVerilogRelatedFile vfile = codeEditor.Controller.CodeEditor.GetTextFile() as Data.IVerilogRelatedFile;
            if (vfile == null) return;

            ParsedDocument parsedDocument = vfile.VerilogParsedDocument;
            int index = vfile.CodeDocument.CaretIndex;
            Module module = parsedDocument.GetModule(index);


        }
    }
}
