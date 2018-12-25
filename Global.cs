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
            public static ajkControls.Icon Exclamation = new ajkControls.Icon(Properties.Resources.exclamation);
            public static ajkControls.Icon ExclamationBox = new ajkControls.Icon(Properties.Resources.exclamationBox);
            public static ajkControls.Icon Play = new ajkControls.Icon(Properties.Resources.play);
            public static ajkControls.Icon Pause = new ajkControls.Icon(Properties.Resources.pause);
        }
    }
}
