using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Shortint : IntegerAtomType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Shortint;
            }
        }

        public static new Shortint ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "shortint") System.Diagnostics.Debugger.Break();
            word.AddSystemVerilogError();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            Shortint type = new Shortint();
            IntegerAtomType.parse(word, nameSpace, type);
            return type;
        }
    }
}
