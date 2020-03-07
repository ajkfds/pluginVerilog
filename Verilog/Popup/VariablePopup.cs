﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using pluginVerilog.Verilog.Variables;

namespace pluginVerilog.Verilog.Popup
{
    public class VariablePopup : codeEditor.CodeEditor.PopupItem
    {
        public VariablePopup(Variables.Variable variable)
        {
            variable.AppendLabel(label);
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
