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
                codeEditor.Global.Controller.NavigatePanel.GetContextMenuStrip().ImageScalingSize.Height,
                ajkControls.IconImage.ColorStyle.Original );
            iVerilogRunTsmi.Image = codeEditor.Global.IconImages.Play.GetImage(
                codeEditor.Global.Controller.NavigatePanel.GetContextMenuStrip().ImageScalingSize.Height,
                ajkControls.IconImage.ColorStyle.Blue);
            gtkWaveTsmi.Image = codeEditor.Global.IconImages.Wave0.GetImage(
                codeEditor.Global.Controller.NavigatePanel.GetContextMenuStrip().ImageScalingSize.Height,
                ajkControls.IconImage.ColorStyle.Blue);

        }

        private void IVerilogRunTsmi_Click(object sender, EventArgs e)
        {
            string projectName, id;
            codeEditor.Global.Controller.NavigatePanel.GetSelectedNode(out projectName, out id);

            codeEditor.Data.Project project = codeEditor.Global.Projects[projectName];
            Data.VerilogFile topFile = project.GetRegisterdItem(id) as Data.VerilogFile;
            if (topFile == null) return;

            IcarusVerilog.SimulationPanel panel = new IcarusVerilog.SimulationPanel(topFile);
            codeEditor.Controller.MainTabPage mainTabPage = new codeEditor.Controller.MainTabPage(panel, topFile.Name);
            codeEditor.Global.Controller.Tabs.AddPage(mainTabPage);
        }

        private void GtkWaveTsmi_Click(object sender, EventArgs e)
        {

        }
    }
}
