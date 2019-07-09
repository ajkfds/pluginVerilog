using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Time : Variable
    {
        protected Time() { }


        public Time(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            //            time_declaration::= time list_of_variable_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                Time val = new Time();
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
                        if (nameSpace.Variables.ContainsKey(val.Name) && nameSpace.Variables[val.Name] is Time)
                        {
                            val = nameSpace.Variables[val.Name] as Time;
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
