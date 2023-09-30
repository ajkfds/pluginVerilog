using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class String : DataType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.String;
            }
        }

        public static new String ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "string") System.Diagnostics.Debugger.Break();
            if (!word.SystemVerilog) word.AddError("systemverilog description");
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            String type = new String();
            return type;
        }
    }
}
