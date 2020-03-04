using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.Verilog.Popup
{
    public class TaskPopup : codeEditor.CodeEditor.PopupItem
    {
        public TaskPopup(Task task)
        {
            label.AppendText("task ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            label.AppendText(task.Name, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Identifier));
            label.AppendText("\r\n");
            bool first = true;
            foreach (Variables.Port port in task.Ports.Values)
            {
                if (!first) label.AppendText("\r\n");
                label.AppendLabel(port.GetLabel());
                first = false;
            }


            //foreach (Variables.Port port in task.Ports.Values)
            //{
            //    switch (port.Direction)
            //    {
            //        case Variables.Port.DirectionEnum.Input:
            //            label.AppendText(" input ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            //            label.AppendText(port.Name);
            //            label.AppendText("\r\n");
            //            break;
            //        case Variables.Port.DirectionEnum.Output:
            //            label.AppendText(" output ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            //            label.AppendText(port.Name);
            //            label.AppendText("\r\n");
            //            break;
            //        case Variables.Port.DirectionEnum.Inout:
            //            label.AppendText(" inout ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            //            label.AppendText(port.Name);
            //            label.AppendText("\r\n");
            //            break;
            //        default:
            //            label.AppendText(" ");
            //            label.AppendText(port.Name);
            //            label.AppendText("\r\n");
            //            break;
            //    }
            //}
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
