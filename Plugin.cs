using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog
{
    public class Plugin : codeEditorPlugin.IPlugin
    {
        public void Initialize()
        {
            // register filetype
            {
                FileTypes.VerilogFile fileType = new FileTypes.VerilogFile();
                codeEditor.Global.FileTypes.Add(fileType.ID, fileType);
            }
            {
                FileTypes.VerilogHeaderFile fileType = new FileTypes.VerilogHeaderFile();
                codeEditor.Global.FileTypes.Add(fileType.ID, fileType);
            }
            {
                FileTypes.SystemVerilogFile fileType = new FileTypes.SystemVerilogFile();
                codeEditor.Global.FileTypes.Add(fileType.ID, fileType);
            }

            // test rtl project
            string absolutePath = System.IO.Path.GetFullPath(@"..\\..\\..\\..\\pluginVerilog\\TestRTL");
            codeEditor.Data.Project project = codeEditor.Data.Project.Create(absolutePath);
            codeEditor.Global.Controller.AddProject(project);

            // append menu items
            System.Windows.Forms.ContextMenuStrip menu = codeEditor.Global.Controller.NavigatePanel.GetContextMenuStrip();
            menu.Items.Add(Global.SetupForm.icarusVerilogSimulationToolStripMenuItem);
        }
        public string Id { get { return StaticID; } }

        public static string StaticID = "Verilog";
    }
}
