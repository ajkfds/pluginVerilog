using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Shortreal : NonIntegerType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Shortint;
            }
        }

        public static new Shortreal ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "shortreal") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            Shortreal type = new Shortreal();
            return type;
        }
    }
}
