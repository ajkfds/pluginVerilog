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
        public static new RealTime ParseCreateType(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "realtime") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // real

            RealTime type = new RealTime();
            return type;
        }
        public static RealTime CreateFromType(string name, RealTime type)
        {
            RealTime ret = new RealTime();
            return ret;
        }
        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            RealTime type = RealTime.ParseCreateType(word, nameSpace);
            if (type == null)
            {
                word.SkipToKeyword(";");
                if (word.Text == ";") word.MoveNext();
                return;
            }

            ParseCreateFromDeclaration(word, nameSpace, type);
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace, RealTime type)
        {
            //            realtime_declaration::= realtime list_of_real_identifiers;

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                RealTime val = RealTime.CreateFromType(word.Text, type);
                val.DefinedReference = word.GetReference();

                word.Color(CodeDrawStyle.ColorType.Variable);
                word.MoveNext();

                if (word.Text == "=")
                {
                    word.MoveNext();
                    Expressions.Expression initalValue = Expressions.Expression.ParseCreate(word, nameSpace);
                    val.AssignedReferences.Add(val.DefinedReference);
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
                        if (nameSpace.Variables.ContainsKey(val.Name) && nameSpace.Variables[val.Name] is RealTime)
                        {
                            val = nameSpace.Variables[val.Name] as RealTime;
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
