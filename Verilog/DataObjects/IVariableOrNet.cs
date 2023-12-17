using pluginVerilog.Verilog.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.DataObjects
{
    public interface IVariableOrNet
    {
        string Name { set; get; }
        string Comment { set; get; }
        WordReference DefinedReference { set; get; }
        List<Range> Dimensions { get; set; }
            
        List<WordReference> UsedReferences { set; get; }
        List<WordReference> AssignedReferences { set; get; }

        void AppendLabel(ajkControls.ColorLabel.ColorLabel label);
        void AppendTypeLabel(ajkControls.ColorLabel.ColorLabel label);

    }
}
