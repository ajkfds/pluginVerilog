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
        Dictionary<string, DataObjects.DataObject> Variables { get; }
        NameSpace Parent { get; }
        Dictionary<string, DataObjects.Constants.Parameter> Parameters { get; }
        Dictionary<string, NameSpace> NameSpaces { get; }
        NameSpace GetHierNameSpace(int index);
        DataObjects.DataObject GetVariable(string identifier);
        DataObjects.Constants.Parameter GetParameter(string identifier);

        // Bulding Block
        Dictionary<string, Function> Functions { get; }
        Dictionary<string, Task> Tasks { get; }
        Dictionary<string, Class> Classes { get; }
        Dictionary<string, BuildingBlock> Elements { get; }
    }
}
