using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class WordReference
    {
        public WordReference(int index, int length)
        {
            Index = index;
            Length = length;
        }

        public int Index { get; protected set; }
        public int Length { get; protected set; }

    }
}
