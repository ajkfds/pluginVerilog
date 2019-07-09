using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Real : Variable
    {
        protected Real() { }


        public Real(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            //            real_declaration::= real list_of_real_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                Real val = new Real();
                WordReference nameRef = word.GetReference();
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

                if (word.Active)
                {
                    if (word.Prototype)
                    {
                        if (nameSpace.Variables.ContainsKey(val.Name))
                        {
//                            nameRef.AddError("duplicated net name");
                        }
                        else
                        {
                            nameSpace.Variables.Add(val.Name, val);
                        }
                    }
                    else
                    {
                        if (nameSpace.Variables.ContainsKey(val.Name) && nameSpace.Variables[val.Name] is Real)
                        {
                            val = nameSpace.Variables[val.Name] as Real;
                        }
                    }
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
