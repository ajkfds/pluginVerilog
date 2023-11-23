using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ajkControls.ColorLabel;

namespace pluginVerilog.Verilog.Variables
{
    public class Real : ValueVariable
    {
        protected Real() { }

        public static new Real Create(DataTypes.DataType dataType)
        {
            System.Diagnostics.Debug.Assert(dataType.Type == DataTypes.DataTypeEnum.Real);

            Real val = new Real();
            val.DataType = dataType.Type;
            return val;
        }

        public override Variable Clone()
        {
            Real val = new Real();
            val.DataType = DataType;
            return val;
        }

        public override void AppendTypeLabel(ColorLabel label)
        {
            label.AppendText("real ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            label.AppendText(" ");
        }

    }
}
