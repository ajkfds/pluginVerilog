using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.MessageView
{
    public class MessageNode : codeEditor.MessageView.MessageNode
    {
        public MessageNode(Verilog.ParsedDocument.Message message)
        {
            Text = message.Text;
            this.messageType = message.Type;
            this.index = message.Index;
            this.length = message.Length;
            this.itemID = message.ItemID;
            this.project = message.Project;
        }
        Verilog.ParsedDocument.Message.MessageType messageType;
        int index;
        int length;
        string itemID;
        codeEditor.Data.Project project;

        public override void Selected()
        {
            codeEditor.Data.ITextFile textFile = project.GetRegisterdItem(itemID) as codeEditor.Data.ITextFile;
            if (textFile == null) return;
            textFile.CodeDocument.SelectionStart = index;
            textFile.CodeDocument.SelectionLast = index + length;
            textFile.CodeDocument.CaretIndex = index;
            codeEditor.Global.ViewController.CodeEditor.ScrollToCaret();
            codeEditor.Global.ViewController.CodeEditor.Refresh();
        }

        private static ajkControls.Icon icon = new ajkControls.Icon(Properties.Resources.exclamationBox);
        public override void DrawNode(Graphics graphics, int x, int y, Font font, Color color, Color backgroundColor, Color selectedColor, int lineHeight, bool selected)
        {
            switch (messageType)
            {
                case Verilog.ParsedDocument.Message.MessageType.Error:
                    graphics.DrawImage(icon.GetImage(lineHeight, ajkControls.Icon.ColorStyle.Red), new Point(x, y));
                    break;
                case Verilog.ParsedDocument.Message.MessageType.Warning:
                    graphics.DrawImage(icon.GetImage(lineHeight, ajkControls.Icon.ColorStyle.Green), new Point(x, y));
                    break;
                case Verilog.ParsedDocument.Message.MessageType.Notice:
                    graphics.DrawImage(icon.GetImage(lineHeight, ajkControls.Icon.ColorStyle.Blue), new Point(x, y));
                    break;
                case Verilog.ParsedDocument.Message.MessageType.Hint:
                    graphics.DrawImage(icon.GetImage(lineHeight, ajkControls.Icon.ColorStyle.White), new Point(x, y));
                    break;
            }
            Color bgColor = backgroundColor;
            if (selected) bgColor = selectedColor;
            System.Windows.Forms.TextRenderer.DrawText(
                graphics,
                Text,
                font,
                new Point(x + lineHeight + (lineHeight >> 2), y),
                color,
                bgColor,
                System.Windows.Forms.TextFormatFlags.NoPadding
                );

            //            if (VerilogFile != null && VerilogFile.ParsedDocument != null && VerilogFile.ParsedDocument.Messages.Count != 0)
            //            {
            //                graphics.DrawImage(Style.ExclamationIcon.GetImage(lineHeight, ajkControls.Icon.ColorStyle.Red), new Point(x, y));
            //           }
        }

    }
}
