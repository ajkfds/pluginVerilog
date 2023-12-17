﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.BuildingBlocks
{
    public interface IModuleOrGeneratedBlock
    {
        Dictionary<string, DataObjects.Variables.Variable> Variables { get; }
        NameSpace Parent { get; }
        Dictionary<string, DataObjects.Parameter> Parameters { get; }
        Dictionary<string, DataObjects.Parameter> LocalParameters { get; }
        Module Module { get; }
        Dictionary<string, NameSpace> NameSpaces { get; }

        NameSpace GetHierNameSpace(int index);

        DataObjects.Variables.Variable GetVariable(string identifier);

        DataObjects.Parameter GetParameter(string identifier);

        // module
        Dictionary<string, Function> Functions { get; }
        Dictionary<string, Task> Tasks { get; }
        Dictionary<string, ModuleItems.ModuleInstantiation> ModuleInstantiations { get; }

    }
}
