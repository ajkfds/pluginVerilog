﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.BuildingBlocks
{
    public interface IBuildingBlockWithModuleInstance
    {
        Dictionary<string, ModuleItems.IInstantiation> ModuleInstantiations { get; }
    }
}
