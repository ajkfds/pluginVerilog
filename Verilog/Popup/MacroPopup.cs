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
        public MacroPopup(string text)
        {
            color = CodeDrawStyle.Color(CodeDrawStyle.ColorType.Paramater);
            icon = new ajkControls.IconImage(Properties.Resources.netBox);
            iconColorStyle = ajkControls.IconImage.ColorStyle.Red;
            this.text = text;
        }

        private string text;
        private ajkControls.IconImage icon = null;
        private ajkControls.IconImage.ColorStyle iconColorStyle;
        private System.Drawing.Color color;

        public override Size GetSize(Graphics graphics, Font font)
        {
            Size tsize = System.Windows.Forms.TextRenderer.MeasureText(graphics, text, font);
            return new Size(tsize.Width + tsize.Height + (tsize.Height >> 2), tsize.Height);
        }

        public override void Draw(Graphics graphics, int x, int y, Font font, Color backgroundColor)
        {
            Size tsize = System.Windows.Forms.TextRenderer.MeasureText(graphics, text, font);
            if (icon != null) graphics.DrawImage(icon.GetImage(tsize.Height, iconColorStyle), new Point(x, y));
            Color bgColor = backgroundColor;
            System.Windows.Forms.TextRenderer.DrawText(
                graphics,
                text,
                font,
                new Point(x + tsize.Height + (tsize.Height >> 2), y),
                color,
                bgColor,
                System.Windows.Forms.TextFormatFlags.NoPadding
                );
        }
    }
}
