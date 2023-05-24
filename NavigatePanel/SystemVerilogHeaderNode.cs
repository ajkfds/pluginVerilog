using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.NavigatePanel
{
    //public class SystemVerilogHeaderNode : codeEditor.NavigatePanel.FileNode, IVerilogNavigateNode
    //{
    //    public SystemVerilogHeaderNode(string ID, codeEditor.Data.Project project) : base(ID, project)
    //    {

    //    }
    //    public Data.IVerilogRelatedFile VerilogRelatedFile
    //    {
    //        get { return Item as Data.IVerilogRelatedFile; }
    //    }

    //    public codeEditor.Data.ITextFile ITextFile
    //    {
    //        get { return Item as codeEditor.Data.ITextFile; }
    //    }

    //    public override string Text
    //    {
    //        get { return FileItem.Name; }
    //    }

    //    public override void DrawNode(Graphics graphics, int x, int y, Font font, Color color, Color backgroundColor, Color selectedColor, int lineHeight, bool selected)
    //    {
    //        graphics.DrawImage(Global.Icons.SystemVerilogHeader.GetImage(lineHeight, IconImage.ColorStyle.Blue), new Point(x, y));
    //        Color bgColor = backgroundColor;
    //        if (selected) bgColor = selectedColor;
    //        System.Windows.Forms.TextRenderer.DrawText(
    //            graphics,
    //            Text,
    //            font,
    //            new Point(x + lineHeight + (lineHeight >> 2), y),
    //            color,
    //            bgColor,
    //            System.Windows.Forms.TextFormatFlags.NoPadding
    //            );
    //    }

    //    public override void Selected()
    //    {
    //        codeEditor.Controller.CodeEditor.SetTextFile(ITextFile);
    //    }
    //}
}
