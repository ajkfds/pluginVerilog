using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class ConstantString : Primary
    {
        protected ConstantString() { }

        public static ConstantString ParseCreate(WordScanner word)
        {
            word.Color(CodeDrawStyle.ColorType.Number);
            ConstantString str = new ConstantString();

            word.MoveNext();
            return str;
        }
    }
}
