using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog
{
    public static class Global
    {
        public static SetupForm SetupForm = new SetupForm();
        public static CodeDrawStyle CodeDrawStyle = new CodeDrawStyle();

        public static class Icons
        {
            public static ajkControls.IconImage Exclamation = new ajkControls.IconImage(Properties.Resources.exclamation);
            public static ajkControls.IconImage ExclamationBox = new ajkControls.IconImage(Properties.Resources.exclamationBox);
            public static ajkControls.IconImage Play = new ajkControls.IconImage(Properties.Resources.play);
            public static ajkControls.IconImage Pause = new ajkControls.IconImage(Properties.Resources.pause);
            public static ajkControls.IconImage Verilog = new ajkControls.IconImage(Properties.Resources.verilog);
            public static ajkControls.IconImage VerilogHeader = new ajkControls.IconImage(Properties.Resources.verilogHeader);
            public static ajkControls.IconImage SystemVerilog = new ajkControls.IconImage(Properties.Resources.systemVerilog);
            public static ajkControls.IconImage SystemVerilogHeader = new ajkControls.IconImage(Properties.Resources.systemVerilogHeader);
        }
    }
}
