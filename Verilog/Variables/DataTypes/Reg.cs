using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Reg : IntegerVectorType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Reg;
            }
        }

        public static new Reg ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "reg") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg

            Reg type = new Reg();
            IntegerVectorType.parse(word, nameSpace, type);
            return type;
        }
    }
}
