using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Int : Variable
    {
        protected Int() { }

        public bool Signed { get; protected set; } = false;

        public Int(string Name)
        {
            this.Name = Name;
        }

        public static new Int ParseCreateType(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "int") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // integer

            Int type = new Int();

            if (word.Eof)
            {
                word.AddError("illegal integer declaration");
                return null;
            }

            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }

            if (word.Text == "signed")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                type.Signed = true;
            }
            else if (word.Text == "unsigned")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                type.Signed = false;
            }

            if (word.Eof)
            {
                word.AddError("illegal integer declaration");
                return null;
            }
            return type;
        }

        public static Int CreateFromType(string name, Int type)
        {
            Int int_ = new Int();
            int_.Name = name;
            return int_;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            Int type = Int.ParseCreateType(word, nameSpace);
            if (type == null)
            {
                word.SkipToKeyword(";");
                if (word.Text == ";") word.MoveNext();
                return;
            }

            ParseCreateFromDeclaration(word, nameSpace, type);
        }
        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace, Int type)
        {
            //            integer_declaration::= integer list_of_variable_identifiers;

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal integer identifier");
                    return;
                }
                Int val = Int.CreateFromType(word.Text, type);
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
//                            nameRef.AddError("duplicated name");
                        }
                        else
                        {
                            nameSpace.Variables.Add(val.Name, val);
                        }
                    }
                    else
                    {
                        if (nameSpace.Variables.ContainsKey(val.Name))
                        {
                            if(nameSpace.Variables[val.Name] is Int)
                            {
                                val = nameSpace.Variables[val.Name] as Int;
                            }
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
