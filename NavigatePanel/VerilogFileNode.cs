﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.NavigatePanel
{
    public class VerilogFileNode : codeEditor.NavigatePanel.FileNode
    {
        public VerilogFileNode(string ID, codeEditor.Data.Project project) : base(ID, project)
        {
            if (NodeCreated != null) NodeCreated(this);
        }

        public static Action<VerilogFileNode> NodeCreated;

        public Action NodeSelected;


        public codeEditor.Data.ITextFile ITextFile
        {
            get { return Project.GetRegisterdItem(ID) as codeEditor.Data.ITextFile; }
        }

        public virtual Data.VerilogFile VerilogFile
        {
            get { return Project.GetRegisterdItem(ID) as Data.VerilogFile; }
        }

        public override string Text
        {
            get { return FileItem.Name; }
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

            if(VerilogFile != null && VerilogFile.ParsedDocument != null && VerilogFile.VerilogParsedDocument.ErrorCount != 0)
            {
                graphics.DrawImage(Global.Icons.Exclamation.GetImage(lineHeight, ajkControls.IconImage.ColorStyle.Red), new Point(x, y));
            }
        }

        public override void Selected()
        {
            codeEditor.Global.Controller.NavigatePanel.GetContextMenuStrip().Items["IcarusVerilogTsmi"].Visible = true;
            codeEditor.Global.Controller.CodeEditor.SetTextFile(ITextFile);

            if (NodeSelected != null) NodeSelected();
        }

        public override void Update()
        {
            VerilogFile.Update();

            List<string> currentDataIds = new List<string>();
            foreach (string key in VerilogFile.Items.Keys)
            {
                currentDataIds.Add(key);
            }

            List<codeEditor.NavigatePanel.NavigatePanelNode> removeNodes = new List<codeEditor.NavigatePanel.NavigatePanelNode>();
            foreach (codeEditor.NavigatePanel.NavigatePanelNode node in TreeNodes)
            {
                if (currentDataIds.Contains(node.ID))
                {
                    currentDataIds.Remove(node.ID);
                }
                else
                {
                    removeNodes.Add(node);
                }
            }

            foreach (codeEditor.NavigatePanel.NavigatePanelNode nodes in removeNodes)
            {
                TreeNodes.Remove(nodes);
            }

            foreach (string id in currentDataIds)
            {
                codeEditor.Data.Item item = Project.GetRegisterdItem(id);
                if (item == null) continue;
                TreeNodes.Add(item.CreateNode());
            }
        }

    }

}
