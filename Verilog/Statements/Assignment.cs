using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Statements
{

    public class NonBlockingAssignment : IStatement
    {
        protected NonBlockingAssignment() { }

        public Expressions.Expression LValue { get; protected set; }
        public Expressions.Expression Expression { get; protected set; }

        /*
        A.6.2 Procedural blocks and assignments
        initial_construct   ::= initial statement
        always_construct    ::= always statement
        blocking_assignment ::= variable_lvalue = [ delay_or_event_control ] expression
        nonblocking_assignment ::= variable_lvalue <= [ delay_or_event_control ] expression
        procedural_continuous_assignments   ::= assign variable_assignment
                                                | deassign variable_lvalue
                                                | force variable_assignment
                                                | force net_assignment
                                                | release variable_lvalue
                                                | release net_lvalue
         */
        public static NonBlockingAssignment ParseCreate(WordScanner word,NameSpace nameSpace,Expressions.Expression lExpression)
        {
            if(word.Text != "<=")
            {
                System.Diagnostics.Debugger.Break();
                return null;
            }
            WordScanner equalPointer = word.Clone();
            word.MoveNext();    // <=

            if (word.GetCharAt(0) == '#')
            {
                DelayControl delayControl = DelayControl.ParseCreate(word, nameSpace);
            }
            else if (word.GetCharAt(0) == '@')
            {
                EventControl eventControl = EventControl.ParseCreate(word, nameSpace);
            }

            Expressions.Expression expression = Expressions.Expression.ParseCreate(word, nameSpace);
            if(expression == null)
            {
                word.SkipToKeyword(";");
                word.AddError("illegal non blocking assignment");
                return null;
            }
            if (lExpression != null && lExpression.BitWidth != null && expression.BitWidth != null)
            {
                if (lExpression.BitWidth != expression.BitWidth)
                {
                    equalPointer.AddWarning("bitwidth mismatch " + lExpression.BitWidth + " vs " + expression.BitWidth);
                }
            }

            NonBlockingAssignment assignment = new NonBlockingAssignment();
            assignment.LValue = lExpression;
            assignment.Expression = expression;
            return assignment;
        }
    }
    public class BlockingAssignment : IStatement
    {
        protected BlockingAssignment() { }

        public Expressions.Expression LValue { get; protected set; }
        public Expressions.Expression Expression { get; protected set; }
        /*
        A.6.2 Procedural blocks and assignments
        initial_construct   ::= initial statement
        always_construct    ::= always statement
        blocking_assignment ::= variable_lvalue = [ delay_or_event_control ] expression
        nonblocking_assignment ::= variable_lvalue <= [ delay_or_event_control ] expression
        procedural_continuous_assignments   ::= assign variable_assignment
                                                | deassign variable_lvalue
                                                | force variable_assignment
                                                | force net_assignment
                                                | release variable_lvalue
                                                | release net_lvalue
         */
        public static BlockingAssignment ParseCreate(WordScanner word, NameSpace nameSpace, Expressions.Expression lExpression)
        {
            if (word.Text != "=")
            {
                System.Diagnostics.Debugger.Break();
                return null;
            }
            WordScanner equalPointer = word.Clone();
            word.MoveNext();    // <=

            if(word.GetCharAt(0) == '#')
            {
                DelayControl delayControl = DelayControl.ParseCreate(word, nameSpace);
            }else if(word.GetCharAt(0) == '@')
            {
                EventControl eventControl = EventControl.ParseCreate(word, nameSpace);
            }

            // delay or event control

            Expressions.Expression expression = Expressions.Expression.ParseCreate(word, nameSpace);
            if (expression == null)
            {
                word.SkipToKeyword(";");
                word.AddError("illegal non blocking assignment");
                return null;
            }
            if(lExpression != null && lExpression.BitWidth != null && expression.BitWidth != null)
            {
                if(lExpression.BitWidth != expression.BitWidth)
                {
                    equalPointer.AddWarning("bitwidth mismatch "+lExpression.BitWidth+" vs "+expression.BitWidth);
                }
            }

            BlockingAssignment assignment = new BlockingAssignment();
            assignment.LValue = lExpression;
            assignment.Expression = expression;
            return assignment;
        }
    }
}
