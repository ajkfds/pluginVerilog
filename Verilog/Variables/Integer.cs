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

        public static new Integer ParseCreateType(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "integer") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // integer

            Integer type = new Integer();
            if (word.Eof)
            {
                word.AddError("illegal integer declaration");
                return null;
            }
            return type;
        }

        public static Integer CreateFromType(string name, Integer type)
        {
            Integer integer = new Integer();
            integer.Name = name;
            return integer;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            Integer type = Integer.ParseCreateType(word, nameSpace);
            if (type == null)
            {
                word.SkipToKeyword(";");
                if (word.Text == ";") word.MoveNext();
                return;
            }

            ParseCreateFromDeclaration(word, nameSpace, type);
        }
        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace, Integer type)
        {
            //            integer_declaration::= integer list_of_variable_identifiers;

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal integer identifier");
                    return;
                }
                Integer val = Integer.CreateFromType(word.Text, type);
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
                            if(nameSpace.Variables[val.Name] is Integer)
                            {
                                val = nameSpace.Variables[val.Name] as Integer;
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
