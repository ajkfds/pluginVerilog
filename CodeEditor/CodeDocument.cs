using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.CodeEditor
{
    public class CodeDocument : codeEditor.CodeEditor.CodeDocument
    {
        // get word boundery for editor word selection

        public override void GetWord(int index, out int headIndex, out int length)
        {
            headIndex = index;
            length = 0;
            char ch = GetCharAt(index);
            if (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t') return;

            while (headIndex > 0)
            {
                ch = GetCharAt(headIndex);
                if (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t')
                {
                    break;
                }
                headIndex--;
            }
            headIndex++;

            int nextIndex;
            Verilog.WordPointer.WordTypeEnum wordType;
            Verilog.WordPointer.FetchNext(this, ref headIndex, out length, out nextIndex, out wordType);

            while(nextIndex <= index && index < Length)
            {
                headIndex = nextIndex;
                Verilog.WordPointer.FetchNext(this, ref headIndex, out length, out nextIndex, out wordType);
            }
        }

    }
}
