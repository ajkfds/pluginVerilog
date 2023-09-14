using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.BuildingBlocks
{
    public class BuildingBlock : NameSpace, IBuildingBlock
    {
        protected BuildingBlock(Module module, NameSpace parent) :base(module,parent)
        {
        }

        #region IDesignElementContainer

        private Dictionary<string, Function> functions = new Dictionary<string, Function>();
        public Dictionary<string, Function> Functions { get { return functions; } }

        private Dictionary<string, Task> tasks = new Dictionary<string, Task>();
        public Dictionary<string, Task> Tasks { get { return tasks; } }

        private Dictionary<string, Class> classes = new Dictionary<string, Class>();
        public Dictionary<string, Class> Classes { get { return classes; } }

        private Dictionary<string, BuildingBlock> elements = new Dictionary<string, BuildingBlock>();
        public Dictionary<string, BuildingBlock> Elements { get { return elements; } }

        private bool reparseRequested = false;
        public bool ReperseRequested
        {
            get
            {
                return reparseRequested;
            }
            set
            {
                reparseRequested = value;
            }
        }

        #endregion
    }
}
