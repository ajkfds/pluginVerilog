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

        }

        private void IVerilogRunTsmi_Click(object sender, EventArgs e)
        {
            string projectName, id;
            codeEditor.Controller.NavigatePanel.GetSelectedNode(out projectName, out id);

            codeEditor.Data.Project project = codeEditor.Global.Projects[projectName];
            Data.VerilogFile topFile = project.GetRegisterdItem(id) as Data.VerilogFile;
            if (topFile == null) return;

            IcarusVerilog.SimulationTab tabPage = new IcarusVerilog.SimulationTab(topFile);
            codeEditor.Controller.Tabs.AddPage(tabPage);
        }

        private void GtkWaveTsmi_Click(object sender, EventArgs e)
        {

        }

        private void CreateVerilogFileTsmi_Click(object sender, EventArgs e)
        {
            string projectName;
            string id;
            codeEditor.Controller.NavigatePanel.GetSelectedNode(out projectName, out id);

            codeEditor.Data.Project project = codeEditor.Global.Projects[projectName];
            var item = project.GetRegisterdItem(id);

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
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(stream))
                {
                    sw.Write("//");
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
    }
}
