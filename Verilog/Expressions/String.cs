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
            str.text = word.Text;

            word.MoveNext();
            return str;
        }

        protected string text;
        public string Text
        {
            get { return text; }
        }

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText(Text);
            return label;
        }

    }
}
