using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ajkControls.ColorLabel;

namespace pluginVerilog.Verilog.Variables
{
    public class RealTime : Variable
    {
        protected RealTime() { }

        public static new RealTime Create(DataTypes.DataType dataType)
        {
            System.Diagnostics.Debug.Assert(dataType.Type == DataTypes.DataTypeEnum.Realtime);

            RealTime val = new RealTime();
            val.DataType = dataType.Type;
            return val;
        }

        public override Variable Clone()
        {
            RealTime val = new RealTime();
            val.DataType = DataType;
            return val;
        }

        public override void AppendTypeLabel(ColorLabel label)
        {
            label.AppendText("realtime ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            label.AppendText(" ");
        }

    }
}
