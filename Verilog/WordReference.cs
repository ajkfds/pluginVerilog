using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class WordReference : IDisposable
    {
        public WordReference(int index, int length, codeEditor.CodeEditor.ParsedDocument parsedDocument)
        {
            Index = index;
            Length = length;
            ParsedDocument = parsedDocument;

            ParsedDocument.ShouldDisposeObjects.Add(this);
        }
        public void Dispose()
        {
            ParsedDocument = null;
        }

        public int Index { get; protected set; }
        public int Length { get; protected set; }

        public codeEditor.CodeEditor.ParsedDocument ParsedDocument { get; protected set; }

    }
}
