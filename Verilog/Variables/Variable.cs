using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.Verilog.Variables
{
    // Variable -+-> net
    //           +-> reg
    //           +-> real
    //           +-> integer

    public class Variable
    {
        public string Name;
        protected List<Dimension> dimensions = new List<Dimension>();
        public IReadOnlyList<Dimension> Dimensions { get { return dimensions; } }
    }

    public class VariablePopup : codeEditor.CodeEditor.PopupItem
    {
        public VariablePopup(Variable variable)
        {
            if(variable is Net)
            {
                Net val = variable as Net;
                color =　CodeDrawStyle.Color(CodeDrawStyle.ColorType.Net);
                icon = new ajkControls.IconImage(Properties.Resources.netBox);
                iconColorStyle = ajkControls.IconImage.ColorStyle.Red;
                text = variable.Name;
            }
            else if(variable is Reg)
            {
                Reg val = variable as Reg;
                color = CodeDrawStyle.Color(CodeDrawStyle.ColorType.Net);
                icon = new ajkControls.IconImage(Properties.Resources.regBox);
                iconColorStyle = ajkControls.IconImage.ColorStyle.Red;
                text = variable.Name;
                if(val.Range != null)
                {
                    text = text +" "+ val.Range.ToString();
                }
            }
            else
            {
                color = Color.Black;
                text = variable.Name;
            }
        }

        private string text;
        private ajkControls.IconImage icon = null;
        private ajkControls.IconImage.ColorStyle iconColorStyle;
        private Color color;

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








    /*



    variable_type ::=  variable_identifier [ = constant_expression ] | variable_identifier dimension { dimension } 
    list_of_variable_identifiers ::= variable_type { , variable_type }
    */
}
