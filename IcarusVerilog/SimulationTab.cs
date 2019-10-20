using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.IcarusVerilog
{
    public class SimulationTab : codeEditor.Tabs.MainTabPage
    {
        public SimulationTab(pluginVerilog.Data.VerilogFile topFile) : base(new SimulationPanel(topFile), topFile.Name)
        {
            Text = topFile.Name;
            CloseButtonEnable = true;
            IconImage = codeEditor.Global.IconImages.Wave0;

            (panel as SimulationPanel).RequestTabIconChange += changeIcon;
            Controls.Add(panel);
        }

        private void changeIcon(ajkControls.IconImage iconImage, ajkControls.IconImage.ColorStyle color)
        {
            Invoke(new Action(
                () => {
                    IconImage = iconImage;
                    IconColor = color;
                    Invalidate();
                }
                ));
        }

    }
}
