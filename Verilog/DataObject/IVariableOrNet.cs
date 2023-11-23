using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public interface IVariableOrNet
    {
        string Name { set; get; }
        string Comment { set; get; }
        WordReference DefinedReference { set; get; }
        List<Variables.Range> Dimensions { get; set; }
            
        List<WordReference> UsedReferences { set; get; }
        List<WordReference> AssignedReferences { set; get; }
    }
}
