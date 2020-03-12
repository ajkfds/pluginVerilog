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
        public override string CreateString()
        {
            return Text;
        }
        public override void AppendLabel(ajkControls.ColorLabel label)
        {
            label.AppendText(Text, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Variable));
        }

        public override void AppendString(StringBuilder stringBuilder)
        {
            stringBuilder.Append(Text);
        }


    }
}
