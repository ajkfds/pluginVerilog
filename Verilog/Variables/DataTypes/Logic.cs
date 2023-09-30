using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Logic : IntegerVectorType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Logic;
            }
        }

        public static new Logic ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "logic") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            if (!word.SystemVerilog) word.AddError("systemverilog description");
            word.MoveNext(); // reg

            Logic type = new Logic();
            IntegerVectorType.parse(word, nameSpace, type);
            return type;
        }
    }
}
