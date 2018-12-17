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
        }

        private void icarusVerilogSimulationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string projectName, id;
            codeEditor.Global.Controller.NavigatePanel.GetSelectedNode(out projectName, out id);

            codeEditor.Data.Project project = codeEditor.Global.Projects[projectName];
            Data.VerilogFile topFile = project.GetRegisterdItem(id) as Data.VerilogFile;
            if (topFile == null) return;

            //ajkControls.TabPage page = new IcarusVerilog.SimulationTab(topFile);
            //codeEditor.Global.Controller.Tabs.AddPage(page);



            IcarusVerilog.SimulationPanel panel = new IcarusVerilog.SimulationPanel(topFile);
            codeEditor.Controller.MainTabPage mainTabPage = new codeEditor.Controller.MainTabPage(panel, topFile.Name);
            codeEditor.Global.Controller.Tabs.AddPage(mainTabPage);
        }
    }
}
