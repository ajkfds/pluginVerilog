using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class NonIntegerType : DataType
    {
        public virtual bool Signed { get; protected set; }

//      non_integer_type
//      non_integer_type::= "shortreal" | "real" | "realtime"

        public static new NonIntegerType ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            switch (word.Text)
            {
                case "shortreal":
                    return Shortreal.ParseCreate(word, nameSpace);
                case "real":
                    return Real.ParseCreate(word, nameSpace);
                case "realtime":
                    return null;
                default:
                    return null;
            }
        }

    }
}
