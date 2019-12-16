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
                label.AppendLabel(port.GetLabel());
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
