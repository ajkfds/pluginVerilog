using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Byte : IntegerAtomType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Byte;
            }
        }

        public static new Byte ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "byte") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.AddSystemVerilogError();
            word.MoveNext();

            Byte type = new Byte();
            IntegerAtomType.parse(word, nameSpace, type);
            return type;
        }
    }
}
