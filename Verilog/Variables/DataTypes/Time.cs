using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Time : IntegerAtomType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Time;
            }
        }

        public static new Time ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "time") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            Time type = new Time();
            IntegerAtomType.parse(word, nameSpace, type);
            return type;
        }
    }
}
