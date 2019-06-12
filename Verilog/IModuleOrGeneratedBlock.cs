using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public interface IModuleOrGeneratedBlock
    {
        Dictionary<string, Variables.Variable> Variables { get; }
        NameSpace Parent { get; }
        Dictionary<string, Variables.Parameter> Parameters { get; }
        Dictionary<string, Variables.Parameter> LocalParameters { get; }
        Module Module { get; }
        Dictionary<string, NameSpace> NameSpaces { get; }

        NameSpace GetHierNameSpace(int index);

        Variables.Variable GetVariable(string identifier);

        Variables.Parameter GetParameter(string identifier);

        // module
        Dictionary<string, Function> Functions { get; }
        Dictionary<string, Task> Tasks { get; }
        Dictionary<string, Class> Classes { get; }
        Dictionary<string, ModuleItems.ModuleInstantiation> ModuleInstantiations { get; }

    }
}
