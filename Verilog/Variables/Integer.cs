using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Integer : Variable
    {
        protected Integer() { }


        public Integer(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            //            integer_declaration::= integer list_of_variable_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // integer

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal integer identifier");
                    return;
                }
                Integer val = new Integer();
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
