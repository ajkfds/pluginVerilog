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
        }
        public string Id { get { return StaticID; } }

        public static string StaticID = "Verilog";
    }
}
