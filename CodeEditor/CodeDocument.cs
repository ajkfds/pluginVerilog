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

            while (headIndex >= 0)
            {
                ch = GetCharAt(headIndex);
                if (ch == ' ' || ch == '\r' || ch == '\n' || ch == '\t')
                {
                    break;
                }
                headIndex--;
            }
            headIndex++;
            if (index < headIndex) headIndex = index;

            int nextIndex;
            Verilog.WordPointer.WordTypeEnum wordType;
            Verilog.WordPointer.FetchNext(this, ref headIndex, out length, out nextIndex, out wordType);

            while(nextIndex <= index && index < Length)
            {
                headIndex = nextIndex;
                Verilog.WordPointer.FetchNext(this, ref headIndex, out length, out nextIndex, out wordType);
            }
        }

        public List<string> GetHierWords(int index)
        {
            List<string> ret = new List<string>();
            int headIndex = GetLineStartIndex(GetLineAt(index));
            int length;
            int nextIndex = headIndex;
            Verilog.WordPointer.WordTypeEnum wordType;

            while (headIndex < Length)
            {
                Verilog.WordPointer.FetchNext(this, ref headIndex, out length, out nextIndex, out wordType);
                if (length == 0) break;
                if (headIndex > index) break;
                ret.Add(CreateString(headIndex, length));
                headIndex = nextIndex;
            }

            int i= ret.Count - 1;
            if (i >= 0 && ret[i] != ".")
            {
//                ret.RemoveAt(i);
                i--;
            }
            while (i>=0)
            {
                if (ret[i] != ".") break;
                ret.RemoveAt(i);
                i--;

                if (i == 0) break;
                i--;
            }

            for(int j = 0; j <= i; j++)
            {
                ret.RemoveAt(0);
            }

            return ret;
        }

        public string GetIndentString(int index)
        {
            StringBuilder sb = new StringBuilder();
            int line = GetLineAt(index);
            int headIndex = GetLineStartIndex(GetLineAt(index));
            int lineLength = GetLineLength(line);

            int i = headIndex;
            while( i < headIndex + lineLength)
            {
                if (GetCharAt(i) != '\t' && GetCharAt(i) != ' ') break;
                sb.Append(GetCharAt(i));
                i++;
            }
            return sb.ToString();
        }

    }
}
