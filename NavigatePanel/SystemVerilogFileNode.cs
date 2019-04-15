﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.NavigatePanel
{
    public class SystemVerilogFileNode : codeEditor.NavigatePanel.FileNode
    {
        public SystemVerilogFileNode(string ID, codeEditor.Data.Project project) : base(ID, project)
        {

        }

        public codeEditor.Data.ITextFile ITextFile
        {
            get { return Project.GetRegisterdItem(ID) as codeEditor.Data.ITextFile; }
        }

        public virtual Data.SystemVerilogFile VerilogFile
        {
            get { return Project.GetRegisterdItem(ID) as Data.SystemVerilogFile; }
        }

        public override string Text
        {
            get { return FileItem.Name; }
        }

        private static ajkControls.IconImage icon = new ajkControls.IconImage(Properties.Resources.verilog);
        public override void DrawNode(Graphics graphics, int x, int y, Font font, Color color, Color backgroundColor, Color selectedColor, int lineHeight, bool selected)
        {
            graphics.DrawImage(icon.GetImage(lineHeight, ajkControls.IconImage.ColorStyle.White), new Point(x, y));
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

            if (VerilogFile != null && VerilogFile.ParsedDocument != null && VerilogFile.VerilogParsedDocument.Status == Verilog.ParsedDocument.FileStatus.Error)
            {
                graphics.DrawImage(Global.Icons.Exclamation.GetImage(lineHeight, ajkControls.IconImage.ColorStyle.Red), new Point(x, y));
            }
        }

        public override void Selected()
        {
            codeEditor.Global.Controller.CodeEditor.SetTextFile(ITextFile);
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
