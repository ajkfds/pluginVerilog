using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Real : NonIntegerType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Real;
            }
        }

        public static new Shortreal ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "real") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            Shortreal type = new Shortreal();
            return type;
        }
    }
}
