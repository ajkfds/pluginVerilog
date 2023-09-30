using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class IntegerVectorType : DataType
    {
        public virtual Range Range { get; protected set; }
        public virtual bool Signed { get; protected set; }

        //      data_type::= integer_vector_type[signing] { packed_dimension }
        //                   ...
        //      integer_vector_type::= "bit" | "logic" | "reg"

        // reg          4state  >=1bit      
        // logic        4state  >=1bit      
        // bit          2state  >=1bit      

        public virtual IReadOnlyList<Variables.Dimension> Dimensions { get; }
        public virtual int BitWidth { get; }

        public static IntegerVectorType ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            switch (word.Text)
            {
                case "bit":
                    return Bit.ParseCreate(word, nameSpace);
                case "logic":
                    return Logic.ParseCreate(word, nameSpace);
                case "reg":
                    return Reg.ParseCreate(word, nameSpace);
                default:
                    return null;
            }
        }

        protected static IntegerVectorType parse(WordScanner word,NameSpace nameSpace,IntegerVectorType integerVectorType)
        {
            integerVectorType.Signed = false;

            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }
            if (word.Text == "signed")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                integerVectorType.Signed = true;
            }
            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }

            integerVectorType.Range = null;
            if (word.GetCharAt(0) == '[')
            {
                integerVectorType.Range = Range.ParseCreate(word, nameSpace);
                if (word.Eof || integerVectorType.Range == null)
                {
                    word.AddError("illegal reg declaration");
                    return null;
                }
            }
            return integerVectorType;
        }

    }
}
