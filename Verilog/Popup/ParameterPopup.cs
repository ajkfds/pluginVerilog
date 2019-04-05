﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.Verilog.Popup
{
    public class ParameterPopup : codeEditor.CodeEditor.PopupItem
    {
        public ParameterPopup(Variables.Parameter parameter)
        {
            label.AppendText("parameter ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            label.AppendText(parameter.Name, CodeDrawStyle.Color(CodeDrawStyle.ColorType.Paramater));
            if(parameter.Expression != null)
            {
                label.AppendText(" = ");
                label.AppendLabel(parameter.Expression.GetLabel());
            }
        }

        ajkControls.ColorLabel label = new ajkControls.ColorLabel();

        public override Size GetSize(Graphics graphics, Font font)
        {
            return label.GetSize(graphics, font);
        }

        public override void Draw(Graphics graphics, int x, int y, Font font, Color backgroundColor)
        {
            label.Draw(graphics, x, y, font, Color.FromArgb(20, 20, 20), backgroundColor);
        }
    }
}