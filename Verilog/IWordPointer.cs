using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public interface IWordPointer : IDisposable
    {
        codeEditor.CodeEditor.CodeDocument Document { get; }
        Verilog.ParsedDocument ParsedDocument { get; }
        IWordPointer Clone();

        void Color(byte colorIndex);
        void AddError(string message);
        void AddWarning(string message);

        void MoveNext();
        void MoveNextUntilEol();

        bool Eof { get; }

        string Text { get; }
        WordPointer.WordTypeEnum WordType { get; }
        int Length { get;  }

        char GetCharAt(int wordIndex);
    }
}