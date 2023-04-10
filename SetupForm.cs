using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pluginVerilog
{
    public partial class SetupForm : Form
    {
        public SetupForm()
        {
            InitializeComponent();

            IcarusVerilogTsmi.Image = Global.Icons.IcarusVerilog.GetImage(
                codeEditor.Controller.NavigatePanel.GetContextMenuStrip().ImageScalingSize.Height,
                ajkControls.IconImage.ColorStyle.Original );
            iVerilogRunTsmi.Image = codeEditor.Global.IconImages.Play.GetImage(
                codeEditor.Controller.NavigatePanel.GetContextMenuStrip().ImageScalingSize.Height,
                ajkControls.IconImage.ColorStyle.Blue);
            gtkWaveTsmi.Image = codeEditor.Global.IconImages.Wave0.GetImage(
                codeEditor.Controller.NavigatePanel.GetContextMenuStrip().ImageScalingSize.Height,
                ajkControls.IconImage.ColorStyle.Blue);
            CreateVerilogFileTsmi.Image = Global.Icons.Verilog.GetImage(
                codeEditor.Controller.NavigatePanel.GetContextMenuStrip().ImageScalingSize.Height,
                ajkControls.IconImage.ColorStyle.Blue);

        }

        private void IVerilogRunTsmi_Click(object sender, EventArgs e)
        {
            codeEditor.NavigatePanel.NavigatePanelNode node;
            codeEditor.Controller.NavigatePanel.GetSelectedNode(out node);

            NavigatePanel.VerilogFileNode vnode = node as NavigatePanel.VerilogFileNode;
            if (vnode == null) return;

            Data.VerilogFile topFile = vnode.VerilogFile;
            if (topFile == null) return;

            IcarusVerilog.SimulationTab tabPage = new IcarusVerilog.SimulationTab(topFile);
            codeEditor.Controller.Tabs.AddPage(tabPage);
        }

        private void GtkWaveTsmi_Click(object sender, EventArgs e)
        {

        }

        private void CreateVerilogFileTsmi_Click(object sender, EventArgs e)
        {
            codeEditor.NavigatePanel.NavigatePanelNode node;
            codeEditor.Controller.NavigatePanel.GetSelectedNode(out node);
            var item = node.Item;
            codeEditor.Data.Project project = item.Project;

            string path;
            if(item == null)
            {
                path = project.RootPath;
            }
            else
            {
                path = project.GetAbsolutePath(item.RelativePath);
            }
            if (!System.IO.Directory.Exists(path))
            {
                path = path.Substring(0, path.LastIndexOf(@"\"));
                if (!System.IO.Directory.Exists(path))
                {
                    return;
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "new.v";
            saveFileDialog.InitialDirectory = path;
            saveFileDialog.Filter = "verilog file (*.v)|*.v";
            if( codeEditor.Controller.ShowDialogForm(saveFileDialog) == DialogResult.OK)
            {
                System.IO.Stream stream = saveFileDialog.OpenFile();
                string moduleName = saveFileDialog.FileName;
                if (moduleName.Contains('\\')) moduleName = moduleName.Substring(moduleName.LastIndexOf('\\')+1);
                if (moduleName.Contains('.')) moduleName = moduleName.Substring(0, moduleName.IndexOf('.'));
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream))
                {
                    sw.Write("module ");
                    sw.Write(moduleName);
                    sw.Write(";\r\n");
                    sw.Write("\r\n");
                    sw.Write("endmodule ");
                    sw.Write("\r\n");
                }
            }
            if(item == null)
            {
                project.Update();
            }
            else
            {
                item.Update();
            }
            codeEditor.Controller.NavigatePanel.UpdateVisibleNode();
        }

        private void checkParseDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            codeEditor.NavigatePanel.NavigatePanelNode node;
            codeEditor.Controller.NavigatePanel.GetSelectedNode(out node);
            var item = node.Item;
            codeEditor.Data.Project project = item.Project;

            string path;
            if (item == null) return;
            path = project.GetAbsolutePath(item.RelativePath);


        }
    }
}
