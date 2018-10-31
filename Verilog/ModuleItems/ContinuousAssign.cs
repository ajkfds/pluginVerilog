using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.ModuleItems
{
    public class ContinuousAssign
    {
        protected ContinuousAssign() { }

        public Variables.VariableAssignment VariableAssignment { get; protected set; }

        public static ContinuousAssign ParseCreate(WordScanner word,Module module)
        {
            // continuous_assign::= assign[drive_strength][delay3] list_of_net_assignments;
            // list_of_net_assignments::= net_assignment { , net_assignment }
            // net_assignment::= net_lvalue = expression
            if(word.Text != "assign")
            {
                System.Diagnostics.Debugger.Break();
            }
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext();

            ContinuousAssign continuousAssign = new ContinuousAssign();
            continuousAssign.VariableAssignment = Variables.VariableAssignment.ParseCreate(word, module);

            if(word.GetCharAt(0) == ';')
            {
                word.MoveNext();
            }
            else
            {
                word.AddError("; expected");
            }
            return continuousAssign;
        }
    }
}
