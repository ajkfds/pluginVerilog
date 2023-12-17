using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.BuildingBlocks
{
    public interface IBuildingBlock
    {
        // NameSpace
        Dictionary<string, DataObjects.IVariableOrNet> Variables { get; }
        NameSpace Parent { get; }
        Dictionary<string, DataObjects.Parameter> Parameters { get; }
        Dictionary<string, DataObjects.Parameter> LocalParameters { get; }
        Dictionary<string, NameSpace> NameSpaces { get; }
        NameSpace GetHierNameSpace(int index);
        DataObjects.IVariableOrNet GetVariable(string identifier);
        DataObjects.Parameter GetParameter(string identifier);

        // Bulding Block
        Dictionary<string, Function> Functions { get; }
        Dictionary<string, Task> Tasks { get; }
        Dictionary<string, Class> Classes { get; }
        Dictionary<string, BuildingBlock> Elements { get; }
    }
}
