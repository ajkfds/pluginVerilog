using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.NavigatePanel
{
    //public class SystemVerilogFileNode : codeEditor.NavigatePanel.FileNode, IVerilogNavigateNode
    //{
    //    public SystemVerilogFileNode(Data.SystemVerilogFile systemVerilogFile) : base(systemVerilogFile)
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

    //    public virtual Data.SystemVerilogFile VerilogFile
    //    {
    //        get { return Item as Data.SystemVerilogFile; }
    //    }

    //    public override string Text
    //    {
    //        get { return FileItem.Name; }
    //    }

    //    public override void DrawNode(Graphics graphics, int x, int y, Font font, Color color, Color backgroundColor, Color selectedColor, int lineHeight, bool selected)
    //    {
    //        graphics.DrawImage(Global.Icons.SystemVerilog.GetImage(lineHeight, IconImage.ColorStyle.Blue), new Point(x, y));
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

    //        if (VerilogFile != null && VerilogFile.ParsedDocument != null && VerilogFile.VerilogParsedDocument.ErrorCount != 0)
    //        {
    //            graphics.DrawImage(Global.Icons.Exclamation.GetImage(lineHeight, IconImage.ColorStyle.Red), new Point(x, y));
    //        }
    //    }

    //    public override void Selected()
    //    {
    //        codeEditor.Controller.CodeEditor.SetTextFile(ITextFile);
    //    }

    //    public override void Update()
    //    {
    //        if (VerilogFile == null)
    //        {
    //            return;
    //        }
    //        VerilogFile.Update();

    //        List<codeEditor.Data.Item> currentDataItems = new List<codeEditor.Data.Item>();
    //        foreach (codeEditor.Data.Item item in VerilogFile.Items.Values)
    //        {
    //            currentDataItems.Add(item);
    //        }

    //        List<codeEditor.NavigatePanel.NavigatePanelNode> removeNodes = new List<codeEditor.NavigatePanel.NavigatePanelNode>();
    //        foreach (codeEditor.NavigatePanel.NavigatePanelNode node in TreeNodes)
    //        {
    //            if (currentDataItems.Contains(node.Item))
    //            {
    //                currentDataItems.Remove(node.Item);
    //            }
    //            else
    //            {
    //                removeNodes.Add(node);
    //            }
    //        }

    //        foreach (codeEditor.NavigatePanel.NavigatePanelNode node in removeNodes)
    //        {
    //            TreeNodes.Remove(node);
    //            node.Dispose();
    //        }

    //        foreach (codeEditor.Data.Item item in currentDataItems)
    //        {
    //            if (item == null) continue;
    //            TreeNodes.Add(item.CreateNode());
    //        }
    //    }

    //}

}
