using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ajkControls.ColorLabel;

namespace pluginVerilog.Verilog.Variables
{
    public class IntegerVectorValueVariable : ValueVariable
    {
        //integer_vector_type::= bit | logic | reg

        public bool Signed { get; set; }

        public List<Range> PackedDimensions = new List<Range>();

        public new static IntegerVectorValueVariable Create(DataTypes.DataType dataType)
        {
            switch (dataType.Type)
            {
                case DataTypes.DataTypeEnum.Bit:
                    return Bit.Create(dataType);
                case DataTypes.DataTypeEnum.Logic:
                    return Logic.Create(dataType);
                case DataTypes.DataTypeEnum.Reg:
                    return Reg.Create(dataType);
                default:
                    System.Diagnostics.Debugger.Break();
                    break;
            }
            return null;
        }


        public override void AppendTypeLabel(ColorLabel label)
        {
            switch (DataType)
            {
                case DataTypes.DataTypeEnum.Bit:
                    label.AppendText("bit ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case DataTypes.DataTypeEnum.Logic:
                    label.AppendText("logic ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case DataTypes.DataTypeEnum.Reg:
                    label.AppendText("reg ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                default:
                    System.Diagnostics.Debugger.Break();
                    break;
            }
            label.AppendText(" ");
            if (Signed)
            {
                label.AppendText("signed ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            }

            foreach(Range dimension in PackedDimensions)
            {
                label.AppendLabel(dimension.GetLabel());
                label.AppendText(" ");
            }
        }

    }
}
