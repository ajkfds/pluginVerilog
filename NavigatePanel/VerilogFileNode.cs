using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.NavigatePanel
{
    public class VerilogFileNode : codeEditor.NavigatePanel.FileNode, IVerilogNavigateNode
    {
        public VerilogFileNode(Data.VerilogFile verilogFile) : base(verilogFile)
        {
            if (NodeCreated != null) NodeCreated(this);
        }
        public static Action<VerilogFileNode> NodeCreated;

        public Action NodeSelected;

        public Data.IVerilogRelatedFile VerilogRelatedFile
        {
            get { return Item as Data.IVerilogRelatedFile; }
        }

        public codeEditor.Data.TextFile TextFile
        {
            get { return Item as codeEditor.Data.TextFile; }
        }

        public virtual Data.VerilogFile VerilogFile
        {
            get { return Item as Data.VerilogFile; }
        }

        public override string Text
        {
            get {
                if (FileItem == null) return "-";
                return FileItem.Name;
            }
        }

        public override void DrawNode(Graphics graphics, int x, int y, Font font, Color color, Color backgroundColor, Color selectedColor, int lineHeight, bool selected)
        {
            graphics.DrawImage(Global.Icons.Verilog.GetImage(lineHeight, ajkControls.IconImage.ColorStyle.Blue), new Point(x, y));
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

            if (VerilogFile != null && VerilogFile.ParsedDocument != null && VerilogFile.VerilogParsedDocument.ErrorCount != 0)
            {
                graphics.DrawImage(Global.Icons.Exclamation.GetImage(lineHeight, ajkControls.IconImage.ColorStyle.Red), new Point(x, y));
            }

            if (VerilogFile != null && VerilogFile.Dirty)
            {
                graphics.DrawImage(Global.Icons.NewBadge.GetImage(lineHeight, ajkControls.IconImage.ColorStyle.Orange), new Point(x, y));
            }
        }

        public override void Selected()
        {
            // activate navigate panel context menu
            var menu = codeEditor.Controller.NavigatePanel.GetContextMenuStrip();
            if (menu.Items.ContainsKey("openWithExploererTsmi")) menu.Items["openWithExploererTsmi"].Visible = true;
            if (menu.Items.ContainsKey("icarusVerilogTsmi")) menu.Items["icarusVerilogTsmi"].Visible = true;

            codeEditor.Controller.CodeEditor.SetTextFile(TextFile);
            if (NodeSelected != null) NodeSelected();
        }

        public override void Update()
        {
            if(VerilogFile == null)
            {
                return;
            }
            VerilogFile.Update();

            List<codeEditor.Data.Item> currentDataItems = new List<codeEditor.Data.Item>();
            foreach (codeEditor.Data.Item item in VerilogFile.Items.Values)
            {
                currentDataItems.Add(item);
            }

            List<codeEditor.NavigatePanel.NavigatePanelNode> removeNodes = new List<codeEditor.NavigatePanel.NavigatePanelNode>();
            foreach (codeEditor.NavigatePanel.NavigatePanelNode node in TreeNodes)
            {
                if (currentDataItems.Contains(node.Item))
                {
                    currentDataItems.Remove(node.Item);
                }
                else
                {
                    removeNodes.Add(node);
                }
            }

            foreach (codeEditor.NavigatePanel.NavigatePanelNode node in removeNodes)
            {
                TreeNodes.Remove(node);
                node.Dispose();
            }

            foreach (codeEditor.Data.Item item in currentDataItems)
            {
                if (item == null) continue;
                TreeNodes.Add(item.CreateNode());
            }
        }

    }

}
