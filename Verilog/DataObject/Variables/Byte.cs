using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Byte : IntegerAtomVariable
    {
        protected Byte() { }

        public static new Byte Create(DataTypes.DataType dataType)
        {
            System.Diagnostics.Debug.Assert(dataType.Type == DataTypes.DataTypeEnum.Byte);
            DataTypes.IntegerAtomType dType = dataType as DataTypes.IntegerAtomType;

            Byte val = new Byte();
            val.DataType = dType.Type;
            return val;
        }

        public override Variable Clone()
        {
            Byte val = new Byte();
            val.DataType = DataType;
            val.Signed = Signed;
            return val;
        }


    }
}
