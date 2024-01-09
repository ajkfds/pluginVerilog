using pluginVerilog.Verilog.BuildingBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.ModuleItems
{
    public interface IInstantiation
    {
        string ModuleName { get; }

        Dictionary<string, Expressions.Expression> ParameterOverrides { get; set; }
        Dictionary<string, Expressions.Expression> PortConnection { get; set; }
        string OverrideParameterID { get; }
        bool Prototype { get; set; }
        BuildingBlock GetInstancedModule();

    }
}
