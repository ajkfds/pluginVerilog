﻿using System;
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

        public static ContinuousAssign ParseCreate(WordScanner word, IModuleOrGeneratedBlock module)
        {
            // continuous_assign::= assign[drive_strength][delay3] list_of_net_assignments;
            // list_of_net_assignments::= net_assignment { , net_assignment }
            // net_assignment::= net_lvalue = expression
            if(word.Text != "assign")
            {
                System.Diagnostics.Debugger.Break();
            }
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            ContinuousAssign continuousAssign = new ContinuousAssign();
            continuousAssign.VariableAssignment = Variables.VariableAssignment.ParseCreate(word, module as NameSpace);

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
