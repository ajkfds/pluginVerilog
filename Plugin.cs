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
            // register filetypes
            {
                FileTypes.VerilogFile fileType = new FileTypes.VerilogFile();
                codeEditor.Global.FileTypes.Add(fileType.ID, fileType);
            }
            {
                FileTypes.VerilogHeaderFile fileType = new FileTypes.VerilogHeaderFile();
                codeEditor.Global.FileTypes.Add(fileType.ID, fileType);
            }

            // append navigate context menu items
            System.Windows.Forms.ContextMenuStrip menu = codeEditor.Controller.NavigatePanel.GetContextMenuStrip();
            menu.Items.Insert(0,Global.SetupForm.IcarusVerilogTsmi);

            foreach(var menuItem in menu.Items)
            {
                if(menuItem is System.Windows.Forms.ToolStripMenuItem)
                {
                    var tsmi = menuItem as System.Windows.Forms.ToolStripMenuItem;
                    if(tsmi.Text == "Add")
                    {
                        tsmi.DropDownItems.Add(Global.SetupForm.CreateVerilogFileTsmi);
                    }
                }
            }

            // register project property creator
            codeEditor.Data.Project.ProjectPropertyCreated.Add(Id, new Func<codeEditor.Data.Project, codeEditor.Data.ProjectProperty>(
                (project) => { return new ProjectProperty(project); }
            ));

            return true;
        }
        public bool Initialize()
        {
            // add test rtl project
            string absolutePath = System.IO.Path.GetFullPath(@"..\\..\\..\\..\\pluginVerilog\\TestRTL");
            codeEditor.Data.Project project = codeEditor.Data.Project.Create(absolutePath);
            codeEditor.Controller.AddProject(project);

            // register project property form tab
            codeEditor.Tools.ProjectPropertyForm.FormCreated += Tools.ProjectPropertyTab.ProjectPropertyFromCreated;

            return true;
        }
        public string Id { get { return StaticID; } }

        public static string StaticID = "Verilog";
    }
}
