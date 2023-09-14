using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.BuildingBlocks
{
    public interface IModuleOrInterfaceOrProgram : IBuildingBlock
    {

        // Port
        Dictionary<string, Variables.Port> Ports { get; }
        List<Variables.Port> PortsList { get; }
        List<string> PortParameterNameList { get; }

        // Generate


    }
}
