using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Int : IntegerAtomType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Int;
            }
        }

        public static new Int ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "int") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.AddSystemVerilogError();
            word.MoveNext();

            Int type = new Int();
            IntegerAtomType.parse(word, nameSpace, type);
            return type;
        }
    }
}
