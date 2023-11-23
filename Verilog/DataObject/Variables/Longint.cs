using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Longint : IntegerAtomVariable
    {
        protected Longint() { }

        public static new Longint Create(DataTypes.DataType dataType)
        {
            System.Diagnostics.Debug.Assert(dataType.Type == DataTypes.DataTypeEnum.Int);
            DataTypes.IntegerAtomType dType = dataType as DataTypes.IntegerAtomType;

            Longint val = new Longint();
            val.DataType = dType.Type;
            return val;
        }

        public override Variable Clone()
        {
            Longint val = new Longint();
            val.DataType = DataType;
            val.Signed = Signed;
            return val;
        }


    }
}
