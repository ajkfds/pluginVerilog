using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Shortint : IntegerAtomVariable
    {
        protected Shortint() { }

        public static new Shortint Create(DataTypes.DataType dataType)
        {
            System.Diagnostics.Debug.Assert(dataType.Type == DataTypes.DataTypeEnum.Shortint);
            DataTypes.IntegerAtomType dType = dataType as DataTypes.IntegerAtomType;

            Shortint val = new Shortint();
            val.DataType = dType.Type;
            return val;
        }

        public override Variable Clone()
        {
            Shortint val = new Shortint();
            val.DataType = DataType;
            val.Signed = Signed;
            return val;
        }


    }
}
