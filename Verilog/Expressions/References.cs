using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class TaskReference : Primary
    {
        public string TaskName { get; protected set; }
        public string ModuleName { get; protected set; }

        public TaskReference(Task task, NameSpace nameSpace)
        {
            TaskName = task.Name;
            ModuleName = nameSpace.Module.Name;
        }
    }

    public class NameSpaceReference : Primary
    {
        public string Name { get; protected set; }
        public NameSpaceReference(NameSpace nameSpace)
        {
            Name = nameSpace.Name;
        }
    }

    public class ModuleInstanceReference : Primary
    {
        ModuleItems.ModuleInstantiation moduleInstantiation;
        public ModuleInstanceReference(ModuleItems.ModuleInstantiation moduleInstantiation)
        {
            this.moduleInstantiation = moduleInstantiation;
        }
    }



}
