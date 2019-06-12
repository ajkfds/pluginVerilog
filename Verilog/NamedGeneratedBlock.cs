using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class NamedGeneratedBlock : NameSpace,IModuleOrGeneratedBlock
    {
        private Dictionary<string, Function> functions = new Dictionary<string, Function>();
        private Dictionary<string, Task> tasks = new Dictionary<string, Task>();
        private Dictionary<string, Class> classes = new Dictionary<string, Class>();
        private Dictionary<string, ModuleItems.ModuleInstantiation> moduleInstantiations = new Dictionary<string, ModuleItems.ModuleInstantiation>();
        public Dictionary<string, Function> Functions { get { return functions; } }
        public Dictionary<string, Task> Tasks { get { return tasks; } }
        public Dictionary<string, Class> Classes { get { return classes; } }
        public Dictionary<string, ModuleItems.ModuleInstantiation> ModuleInstantiations { get { return moduleInstantiations; } }

        protected NamedGeneratedBlock(Module module, NameSpace parent) : base(module, parent)
        {
        }

        public static NamedGeneratedBlock Create(Module module, NameSpace parent, string name)
        {
            NamedGeneratedBlock block = new NamedGeneratedBlock(module, parent);
            block.Name = name;
            return block;
        }

    }
}
