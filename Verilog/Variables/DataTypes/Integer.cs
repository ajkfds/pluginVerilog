using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Integer : IntegerAtomType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Integer;
            }
        }

        public static new Integer ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "integer") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            Integer type = new Integer();
            IntegerAtomType.parse(word, nameSpace, type);
            return type;
        }
    }
}
