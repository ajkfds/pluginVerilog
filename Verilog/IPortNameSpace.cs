using pluginVerilog.Verilog.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public interface IPortNameSpace
    {
        Dictionary<string, Variables.Variable> Variables { get; }
        NameSpace Parent { get; }
        Dictionary<string, Variables.Parameter> Parameters { get; }
        Dictionary<string, Variables.Parameter> LocalParameters { get; }

        BuildingBlocks.BuildingBlock BuildingBlock { get; }

        Dictionary<string, NameSpace> NameSpaces { get; }

        Dictionary<string, Variables.Port> Ports { get; }
        List<Variables.Port> PortsList { get; }
    }
}
