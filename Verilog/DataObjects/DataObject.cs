using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.DataObjects
{
    /// <summary>
    /// . A data object is a named entity that has a data value and a data type associated with it, such as a parameter, a variable, or a net.
    /// </summary>
    public class DataObject
    {
        // #SystemVeriog 2012
        //	net												user-defined-size	4state	v
        //
        //	variable	+ integer_vector_type	+ bit 		user-defined-size	2state	sv
        //										+ logic		user-defined-size	4state  sv
        //										+ reg		user-defined-size	4state	v
        //
        //				+ integer_atom_type		+ byte		8bit signed			2state  sv
        //										+ shortint	16bit signed		2state  sv
        //										+ int		32bit signed		2state  sv
        //										+ longint	64bit signed		2state  sv
        //										+ integer	32bit signed		4state	v
        //										+ time		64bit unsigned		        v
        //
        //            	+ non_integer_type		+ shortreal	                            sv
        //										+ real		                            v
        //										+ realtime	                            v

        // net datat type : logic/integer/reg


    }
}
