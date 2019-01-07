using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class RealTime : Variable
    {
        protected RealTime() { }


        public RealTime(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            //            realtime_declaration::= realtime list_of_real_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                RealTime val = new RealTime();
                val.Name = word.Text;

                word.Color(CodeDrawStyle.ColorType.Variable);
                word.MoveNext();

                if (word.Text == "=")
                {
                    word.MoveNext();
                    Expressions.Expression initalValue = Expressions.Expression.ParseCreate(word, nameSpace);
                }
                else if (word.Text == "[")
                {
                    while (word.Text == "[")
                    {
                        Dimension dimension = Dimension.ParseCreate(word, nameSpace);
                        val.dimensions.Add(dimension);
                    }
                }

                if (nameSpace.Variables.ContainsKey(val.Name))
                {
                    word.AddError("duplicated name");
                }
                else
                {
                    nameSpace.Variables.Add(val.Name, val);
                }
                if (word.GetCharAt(0) != ',') break;
                word.MoveNext();
            }

            if (word.Eof || word.GetCharAt(0) != ';')
            {
                word.AddError("; expected");
            }
            else
            {
                word.MoveNext();
            }

            return;
        }
    }
}
