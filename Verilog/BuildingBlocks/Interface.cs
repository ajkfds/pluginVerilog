using pluginVerilog.Verilog.ModuleItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.BuildingBlocks
{
    public class Interface : BuildingBlock,IModuleOrInterface
    {
        protected Interface() : base(null, null)
        {

        }

        // IModuleOrInterfaceOrProfram
        // Port
        private Dictionary<string, DataObjects.Port> ports = new Dictionary<string, DataObjects.Port>();
        public Dictionary<string, DataObjects.Port> Ports { get { return ports; } }
        private List<DataObjects.Port> portsList = new List<DataObjects.Port>();
        public List<DataObjects.Port> PortsList { get { return portsList; } }
        public WordReference NameReference;
        private List<string> portParameterNameList = new List<string>();
        public List<string> PortParameterNameList { get { return portParameterNameList; } }


    }
}
