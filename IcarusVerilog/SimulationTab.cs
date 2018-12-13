using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.IcarusVerilog
{
    public class SimulationTab : ajkControls.TabPage
    {
        public SimulationTab(Data.VerilogFile topFile)
        {
            Text = topFile.Name;
            CloseButtonEnable = true;

            panel = new SimulationPanel(topFile);
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            Controls.Add(panel);
        }

        private SimulationPanel panel;

        public override void CloseButtonClicked()
        {
            codeEditor.Global.Controller.Tabs.RemovePage(this);
            panel.Dispose();
            Dispose();
        }


    }
}
