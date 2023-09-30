using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class IntegerAtomType : DataType
    {
        public virtual bool Signed { get; protected set; }

        //        data_type::=   integer_atom_type[signing]
        //                     | ...
        //        integer_atom_type::= "byte" | "shortint" | "int" | "longint" | "integer" | "time"

        public static new IntegerAtomType ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            switch (word.Text)
            {
                case "byte":
                    return Byte.ParseCreate(word, nameSpace);
                case "shortint":
                    return Shortint.ParseCreate(word, nameSpace);
                case "int":
                    return Int.ParseCreate(word, nameSpace);
                case "longint":
                    return Longint.ParseCreate(word, nameSpace);
                case "integer":
                    return Integer.ParseCreate(word, nameSpace);
                case "time":
                    return Time.ParseCreate(word, nameSpace);
                default:
                    return null;
            }
        }

        protected static IntegerAtomType parse(WordScanner word, NameSpace nameSpace, IntegerAtomType integerAtomType)
        {
            integerAtomType.Signed = false;

            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }
            if (word.Text == "signed")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                integerAtomType.Signed = true;
            }
            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }

            return integerAtomType;
        }
    }
}
