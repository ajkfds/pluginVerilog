using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.ModuleItems
{
    public class AlwaysConstruct
    {
        protected AlwaysConstruct() { }
        public Statements.IStatement Statetment { get; protected set;}

        public static AlwaysConstruct ParseCreate(WordScanner word,Module module)
        {
        //  always_construct::= always statement
            System.Diagnostics.Debug.Assert(word.Text == "always");
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            AlwaysConstruct always = new AlwaysConstruct();
            always.Statetment = Statements.Statements.ParseCreateStatement(word, module);
            if(always.Statetment == null)
            {
                word.AddError("illegal always construct");
                return null;
            }
            return always;
        }
    }
    /*
    A.6.2 Procedural blocks and assignments
    blocking_assignment ::= variable_lvalue = [ delay_or_event_control ] expression  
    nonblocking_assignment ::= variable_lvalue <= [ delay_or_event_control ] expression  
    procedural_continuous_assignments   ::= assign variable_assignment
                                            | deassign variable_lvalue
                                            | force variable_assignment
                                            | force net_assignment
                                            | release variable_lvalue
                                            | release net_lvalue 
    function_blocking_assignment    ::= variable_lvalue = expression
    function_statement_or_null      ::= function_statement        | { attribute_instance } ;  
    */
}
