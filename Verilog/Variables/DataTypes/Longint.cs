using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Longint : IntegerAtomType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Longint;
            }
        }

        public static new Longint ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "longint") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg

            Longint type = new Longint();
            IntegerAtomType.parse(word, nameSpace, type);
            return type;
        }
    }
}
