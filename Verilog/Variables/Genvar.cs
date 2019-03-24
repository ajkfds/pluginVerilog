﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Genvar : Variable
    {
        protected Genvar() { }


        public Genvar(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            //            genvar_declaration::= genvar list_of_genvar_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                Genvar val = new Genvar();
                WordReference nameRef = word.GetReference();
                val.Name = word.Text;
                if (nameSpace.Variables.ContainsKey(val.Name))
                {
                    if (nameSpace.Variables[val.Name] is Net)
                    {
                        nameSpace.Variables.Remove(val.Name);
                        nameSpace.Variables.Add(val.Name, val);
                    }
                    else
                    {
                        nameRef.AddError("duplicated real name");
                    }
                }
                else
                {
                    nameSpace.Variables.Add(val.Name, val);
                }

                word.Color(CodeDrawStyle.ColorType.Variable);
                word.MoveNext();

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
