using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ajkControls;
using ajkControls.ColorLabel;

namespace pluginVerilog.Verilog.Variables
{
    public class Logic : IntegerVectorValueVariable
    {
        protected Logic() { }

        public static new Logic Create(DataTypes.DataType dataType)
        {
            System.Diagnostics.Debug.Assert(dataType.Type == DataTypes.DataTypeEnum.Logic);
            DataTypes.IntegerVectorType dType = dataType as DataTypes.IntegerVectorType;

            Logic val = new Logic();
            val.PackedDimensions = dType.PackedDimensions;
            val.DataType = dType.Type;
            return val;
        }

        public override Variable Clone()
        {
            Logic val = new Logic();
            val.DataType = DataType;
            val.PackedDimensions = PackedDimensions;
            val.Signed = Signed;
            return val;
        }

    }
}
