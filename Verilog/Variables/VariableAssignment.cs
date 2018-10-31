using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class VariableAssignment
    {
        protected VariableAssignment() { }
        public Expressions.Expression NetLValue { get; protected set; }
        public Expressions.Expression Expression { get; protected set; }
 
        public static VariableAssignment ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            // variable_assignment  ::= variable_lvalue = expression
            // variable_lvalue      ::= hierarchical_variable_identifier
            //                          | hierarchical_variable_identifier[expression] { [expression] }
            //                          | hierarchical_variable_identifier[expression] { [expression] } [range_expression]    
            //                          | hierarchical_variable_identifier[range_expression]   
            //                          | variable_concatenation

            VariableAssignment variableAssign = new VariableAssignment();
            variableAssign.NetLValue = Expressions.Expression.ParseCreateVariableLValue(word, nameSpace);
            if (variableAssign.NetLValue == null) return null;
            if (word.Text != "=")
            {
                word.AddError("= expected.");
                return null;
            }
            word.MoveNext();

            variableAssign.Expression = Expressions.Expression.ParseCreate(word, nameSpace);
            if (variableAssign.Expression == null) return null;

            return variableAssign;
        }
    }
}
