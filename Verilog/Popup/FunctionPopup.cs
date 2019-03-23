using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.Verilog.Popup
{
    public class FunctionPopup : codeEditor.CodeEditor.PopupItem
    {
        public FunctionPopup(Function function)
        {
            label.AppendText("function ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            label.AppendText(function.Name, CodeDrawStyle.Color(CodeDrawStyle.ColorType.Identifier));
            label.AppendText("\r\n");
            foreach(Variables.Port port in function.Ports.Values)
            {
                switch (port.Direction)
                {
                    case Variables.Port.DirectionEnum.Input:
                        label.AppendText(" input ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                        label.AppendText(port.Name);
                        label.AppendText("\r\n");
                        break;
                    case Variables.Port.DirectionEnum.Output:
                        label.AppendText(" output ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                        label.AppendText(port.Name);
                        label.AppendText("\r\n");
                        break;
                    case Variables.Port.DirectionEnum.Inout:
                        label.AppendText(" inout ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                        label.AppendText(port.Name);
                        label.AppendText("\r\n");
                        break;
                    default:
                        label.AppendText(" ");
                        label.AppendText(port.Name);
                        label.AppendText("\r\n");
                        break;
                }
            }
        }

        ajkControls.ColorLabel label = new ajkControls.ColorLabel();

        public override Size GetSize(Graphics graphics, Font font)
        {
            return label.GetSize(graphics, font);
        }

        public override void Draw(Graphics graphics, int x, int y, Font font, Color backgroundColor)
        {
            label.Draw(graphics, x, y, font, Color.FromArgb(20,20,20), backgroundColor);
        }


    }

}
