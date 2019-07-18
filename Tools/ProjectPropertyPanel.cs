using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pluginVerilog.Tools
{
    public partial class ProjectPropertyPanel : UserControl
    {
        public ProjectPropertyPanel(codeEditor.Data.Project project)
        {
            InitializeComponent();

            this.project = project;
            ProjectProperty property = project.ProjectProperties[Plugin.StaticID] as ProjectProperty;

            StringBuilder sb = new StringBuilder();
            foreach (var macro in property.Macros.Values)
            {
                sb.Append("`define " + macro.Name + " " + macro.MacroText);
            }
            macroTxt.Text = sb.ToString();
        }
        codeEditor.Data.Project project;

        public void PropertyAccept()
        {
            string[] macros = macroTxt.Text.Split('\n');
            foreach(string line in macros)
            {
                string text = line.Trim(new char[] {'\n','\r',' ','\t'});
            }
        }
        public void PropertyCancel()
        {

        }
    }
}
