using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog
{
    public class Plugin : codeEditorPlugin.IPlugin
    {
        public bool Register()
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

            // append menu items
            System.Windows.Forms.ContextMenuStrip menu = codeEditor.Global.Controller.NavigatePanel.GetContextMenuStrip();
            menu.Items.Insert(0,Global.SetupForm.IcarusVerilogTsmi);

            return true;
        }
        public bool Initialize()
        {
            // test rtl project
            string absolutePath = System.IO.Path.GetFullPath(@"..\\..\\..\\..\\pluginVerilog\\TestRTL");
            codeEditor.Data.Project project = codeEditor.Data.Project.Create(absolutePath);
            codeEditor.Global.Controller.AddProject(project);

            codeEditor.Tools.ProjectPropertyForm.FormCreated += Tools.ProjectPropertyTab.ProjectPropertyFromCreated;

            return true;
        }
        public string Id { get { return StaticID; } }

        public static string StaticID = "Verilog";
    }
}
