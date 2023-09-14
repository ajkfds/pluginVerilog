using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.BuildingBlocks
{
    public class Program : BuildingBlock, IModuleOrInterfaceOrProgram
    {
        protected Program() : base(null, null)
        {

        }

        // IModuleOrInterfaceOrProfram
        // Port
        private Dictionary<string, Variables.Port> ports = new Dictionary<string, Variables.Port>();
        public Dictionary<string, Variables.Port> Ports { get { return ports; } }
        private List<Variables.Port> portsList = new List<Variables.Port>();
        public List<Variables.Port> PortsList { get { return portsList; } }
        public WordReference NameReference;
        private List<string> portParameterNameList = new List<string>();
        public List<string> PortParameterNameList { get { return portParameterNameList; } }

    }
}
