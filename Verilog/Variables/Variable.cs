using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.Verilog.Variables
{
    // Variable -+-> net
    //           +-> reg
    //           +-> real
    //           +-> integer

    public class Variable
    {
        public string Name;
        protected List<Dimension> dimensions = new List<Dimension>();
        public IReadOnlyList<Dimension> Dimensions { get { return dimensions; } }
    }

    /*
    variable_type ::=  variable_identifier [ = constant_expression ] | variable_identifier dimension { dimension } 
    list_of_variable_identifiers ::= variable_type { , variable_type }
    */
}
