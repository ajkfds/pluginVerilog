using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class Item
    {
        public string Name { get; set; }
        public Attribute Attribute { get; set; }
        public WordReference DefinitionRefrecnce { get; set; }

        public codeEditor.Data.Project Project { get; protected set; }

        public ProjectProperty ProjectProperty
        {
            get
            {
                return Project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;
            }
        }
    }
}
