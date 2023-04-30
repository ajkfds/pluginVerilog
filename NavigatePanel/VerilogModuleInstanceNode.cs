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

            // error mark
            if( VerilogModuleInstance != null && VerilogModuleInstance.VerilogParsedDocument != null && VerilogModuleInstance.VerilogParsedDocument.ErrorCount != 0)
            {
                graphics.DrawImage(Global.Icons.Exclamation.GetImage(lineHeight, ajkControls.IconImage.ColorStyle.Red), new Point(x, y));
            }

            // dirty mark
            if (VerilogFile != null && VerilogFile.Dirty)
            {
                graphics.DrawImage(Global.Icons.NewBadge.GetImage(lineHeight, ajkControls.IconImage.ColorStyle.Orange), new Point(x, y));
            }
        }

        public override void Selected()
        {
            var menu = codeEditor.Controller.NavigatePanel.GetContextMenuStrip();
            if (menu.Items.ContainsKey("openWithExploererTsmi")) menu.Items["openWithExploererTsmi"].Visible = true;

            codeEditor.Controller.CodeEditor.SetTextFile(ModuleInstance);

            if (!ModuleInstance.ParseValid || ModuleInstance.ReparseRequested)
            {
                codeEditor.Tools.ParseHierarchyForm pform = new codeEditor.Tools.ParseHierarchyForm(this);
                codeEditor.Controller.ShowDialogForm(pform);
            }
        }

        public override void Update()
        {
            if (VerilogModuleInstance == null) return;
            VerilogModuleInstance.Update();

            List<codeEditor.Data.Item> targetDataItems = new List<codeEditor.Data.Item>();
            List<codeEditor.Data.Item> addDataItems = new List<codeEditor.Data.Item>();
            foreach (codeEditor.Data.Item item in VerilogModuleInstance.Items.Values)
            {
                targetDataItems.Add(item);
                addDataItems.Add(item);
            }

            List<codeEditor.NavigatePanel.NavigatePanelNode> removeNodes = new List<codeEditor.NavigatePanel.NavigatePanelNode>();
            foreach (codeEditor.NavigatePanel.NavigatePanelNode node in TreeNodes)
            {
                if (node.Item != null && targetDataItems.Contains(node.Item))
                {
                    addDataItems.Remove(node.Item);
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

            int treeIndex = 0;
            foreach (codeEditor.Data.Item item in targetDataItems)
            {
                if (item == null) continue;
                if (addDataItems.Contains(item))
                {
                    TreeNodes.Insert(treeIndex, item.NavigatePanelNode);
                }
                treeIndex++;
            }


        }

    }

}
