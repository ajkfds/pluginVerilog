using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Bit : IntegerVectorType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Bit;
            }
        }

        public static new Bit ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "bit") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.AddSystemVerilogError();
            word.MoveNext(); // reg

            Bit type = new Bit();
            IntegerVectorType.parse(word, nameSpace, type);
            return type;
        }
    }
}
