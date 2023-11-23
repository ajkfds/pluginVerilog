using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.BuildingBlocks
{
    public class BuildingBlock : NameSpace, IBuildingBlock
    {
        protected BuildingBlock(BuildingBlock bulidingBlock, NameSpace parent) :base(bulidingBlock, parent)
        {
        }

        #region IDesignElementContainer

        public Dictionary<string, Function> Functions { get; set; } = new Dictionary<string, Function>();

        public Dictionary<string, Task> Tasks { get; set; } = new Dictionary<string, Task>();

        public Dictionary<string, Class> Classes { get; set; } = new Dictionary<string, Class>();

        public Dictionary<string, Variables.DataTypes.DataType> Datatypes { get; set; } = new Dictionary<string, Variables.DataTypes.DataType>();

        public Dictionary<string, BuildingBlock> Elements { get; set; } = new Dictionary<string, BuildingBlock>();

        public bool AnsiStylePortDefinition { get; set; } = false;
        public Nets.Net.NetTypeEnum DefaultNetType = Verilog.Nets.Net.NetTypeEnum.Wire;

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
