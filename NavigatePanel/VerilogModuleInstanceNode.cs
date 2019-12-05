using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.NavigatePanel
{
    public class VerilogModuleInstanceNode : codeEditor.NavigatePanel.FileNode, IVerilogNavigateNode
    {
        public VerilogModuleInstanceNode(Data.VerilogModuleInstance verilogModuleInstance) : base(verilogModuleInstance)
        {
            moduleInstanceRef = new WeakReference<Data.VerilogModuleInstance>(verilogModuleInstance);
        }

        private System.WeakReference<Data.VerilogModuleInstance> moduleInstanceRef;
        public Data.VerilogModuleInstance ModuleInstance
        {
            get
            {
                Data.VerilogModuleInstance ret;
                if (!moduleInstanceRef.TryGetTarget(out ret)) return null;
                return ret;
            }
        }

        public override codeEditor.Data.File FileItem
        {
            get
            {
                Data.VerilogModuleInstance instance = ModuleInstance;
                return instance;
            }
        }

        public Data.IVerilogRelatedFile VerilogRelatedFile
        {
            get { return Item as Data.IVerilogRelatedFile; }
        }
        public codeEditor.Data.ITextFile ITextFile
        {
            get { return Item as codeEditor.Data.ITextFile; }
        }

        public virtual Data.VerilogFile VerilogFile
        {
            get
            {
                return ModuleInstance.SourceTextFile as Data.VerilogFile;
                //                Data.VerilogModuleInstance instance = Project.GetRegisterdItem(ID) as Data.VerilogModuleInstance;
                //                return Project.GetRegisterdItem(Data.VerilogFile.GetID(instance.RelativePath, Project)) as Data.VerilogFile;
//                return null;
            }
        }

        public Data.VerilogModuleInstance VerilogModuleInstance
        {
            get
            {
                return Item as Data.VerilogModuleInstance;
            }
        }

        public override string Text
        {
            get
            {
                Data.VerilogModuleInstance instance = Item as Data.VerilogModuleInstance;
                return instance.Name + " - " + instance.ModuleName;
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
            var menu = codeEditor.Controller.NavigatePanel.GetContextMenuStrip();
            if (menu.Items.ContainsKey("icarusVerilogSimulationTsmi"))
            {
                menu.Items["icarusVerilogSimulationTsmi"].Visible = true;
            }
            codeEditor.Controller.CodeEditor.SetTextFile(ModuleInstance);
        }

        public override void Update()
        {
            if (VerilogModuleInstance == null) return;
            VerilogModuleInstance.Update();

            List<codeEditor.Data.Item> currentDataItems = new List<codeEditor.Data.Item>();
            foreach (codeEditor.Data.Item item in VerilogModuleInstance.Items.Values)
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
