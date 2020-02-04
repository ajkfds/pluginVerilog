using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.Verilog.Popup
{
    public class MacroPopup : codeEditor.CodeEditor.PopupItem
    {
        public MacroPopup(string macroName,string text)
        {
            label.AppendText("define ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            label.AppendText(macroName, CodeDrawStyle.Color(CodeDrawStyle.ColorType.Identifier));
            label.AppendText(" " + text);
        }

        ajkControls.ColorLabel label = new ajkControls.ColorLabel();

        public override Size GetSize(Graphics graphics, Font font)
        {
            return label.GetSize(graphics, font);
        }

        public override void Draw(Graphics graphics, int x, int y, Font font, Color backgroundColor)
        {
            label.Draw(graphics, x, y, font, Color.FromArgb(210, 210, 210), backgroundColor);
        }
    }
}
