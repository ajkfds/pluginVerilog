using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ajkControls.ColorLabel;

namespace pluginVerilog.Verilog.Variables
{
    public class IntegerAtomVariable : ValueVariable
    {
        //integer_atom_type::= byte | shortint | int | longint | integer | time
        public bool Signed { get; set; }

        public new static IntegerAtomVariable Create(DataTypes.DataType dataType)
        {
            switch (dataType.Type)
            {
                case DataTypes.DataTypeEnum.Byte:
                    return Byte.Create(dataType);
                case DataTypes.DataTypeEnum.Shortint:
                    return Shortint.Create(dataType);
                case DataTypes.DataTypeEnum.Int:
                    return Int.Create(dataType);
                case DataTypes.DataTypeEnum.Longint:
                    return Longint.Create(dataType);
                case DataTypes.DataTypeEnum.Integer:
                    return Longint.Create(dataType);
                case DataTypes.DataTypeEnum.Time:
                    return Time.Create(dataType);
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
                case DataTypes.DataTypeEnum.Byte:
                    label.AppendText("byte ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case DataTypes.DataTypeEnum.Shortint:
                    label.AppendText("shortint ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case DataTypes.DataTypeEnum.Int:
                    label.AppendText("int ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case DataTypes.DataTypeEnum.Longint:
                    label.AppendText("longint ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case DataTypes.DataTypeEnum.Integer:
                    label.AppendText("integer ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case DataTypes.DataTypeEnum.Time:
                    label.AppendText("time ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
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

        }

    }
}
