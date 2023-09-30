﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Realtime : NonIntegerType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Realtime;
            }
        }

        public static new Realtime ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "realtime") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            Realtime type = new Realtime();
            return type;
        }
    }
}
