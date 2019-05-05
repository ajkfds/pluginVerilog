using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Statements
{
    public static class Statements
    {
        /*
        A.6.4 Statements
        statement   ::= { attribute_instance } blocking_assignment ;
                        | { attribute_instance } case_statement
                        | { attribute_instance } conditional_statement
                        | { attribute_instance } disable_statement
                        | { attribute_instance } event_trigger
                        | { attribute_instance } loop_statement
                        | { attribute_instance } nonblocking_assignment ;
                        | { attribute_instance } par_block
                        | { attribute_instance } procedural_continuous_assignments ;
                        | { attribute_instance } procedural_timing_control_statement
                        | { attribute_instance } seq_block
                        | { attribute_instance } system_task_enable
                        | { attribute_instance } task_enable
                        | { attribute_instance } wait_statement

        statement_or_null   ::= statement
                                | { attribute_instance } ;

        function_statement  ::= { attribute_instance } function_blocking_assignment ;
                                | { attribute_instance } function_case_statement
                                | { attribute_instance } function_conditional_statement
                                | { attribute_instance } function_loop_statement
                                | { attribute_instance } function_seq_block
                                | { attribute_instance } disable_statement
                                | { attribute_instance } system_task_enable  
        */
        public static IStatement ParseCreateStatement(WordScanner word, NameSpace nameSpace)
        {
            /*
            A.6.4 Statements
            statement   ::= 
                            | { attribute_instance } conditional_statement

                            /// done
                            | { attribute_instance } blocking_assignment ;
                            | { attribute_instance } nonblocking_assignment ;
                            | { attribute_instance } procedural_timing_control_statement
                            | { attribute_instance } loop_statement
                            | { attribute_instance } case_statement
                            | { attribute_instance } disable_statement
                            | { attribute_instance } seq_block
                            | { attribute_instance } par_block

                            | { attribute_instance } event_trigger
                            | { attribute_instance } procedural_continuous_assignments ;
                            | { attribute_instance } system_task_enable
                            | { attribute_instance } task_enable
                            | { attribute_instance } wait_statement
            procedural_timing_control_statement ::= delay_or_event_control statement_or_null 
            */
            switch (word.Text)
            {
                case "if":
                    return ConditionalStatement.ParseCreate(word, nameSpace);
                case "#":
                case "@":
                    return ProceduralTimingControlStatement.ParseCreate(word, nameSpace);
                case "begin":
                    return SequentialBlock.ParseCreate(word, nameSpace);
                case "fork":
                    return ParallelBlock.ParseCreate(word, nameSpace);
                case "forever":
                    return ForeverStatement.ParseCreate(word, nameSpace);
                case "repeat":
                    return RepeatStatement.ParseCreate(word, nameSpace);
                case "while":
                    return WhileStatememt.ParseCreate(word, nameSpace);
                case "for":
                    return ForStatememt.ParseCreate(word, nameSpace);
                case "case":
                case "casex":
                case "casez":
                    return CaseStatement.ParseCreate(word, nameSpace);
                case "disable":
                    return DisableStatement.ParseCreate(word,nameSpace);
                default:
                    string nextText = word.NextText;
                    if (nextText == "(" || nextText == ";")
                    {
                        if (word.Text.StartsWith("$"))
                        {
                            if (!word.RootParsedDocument.ProjectProperty.SystemTaskParsers.ContainsKey(word.Text))
                            {
                                word.AddError("illegal system task");
                            }
                            return TaskEnable.ParseCreate(word, nameSpace); // system task enable
                        }
                        else if (General.IsIdentifier(word.Text)){
                            return TaskEnable.ParseCreate(word, nameSpace);
                        }
                    }

                    Expressions.Expression expression = Expressions.Expression.ParseCreateVariableLValue(word, nameSpace);
                    IStatement statement;
                    if(expression == null)
                    {
                        word.MoveNext();
                    }
/*                    if(expression == null)
                    {
                        if (General.IsIdentifier(word.Text))
                        {
                            word.AddError("illegal identifier");
                            word.MoveNext();
                        }
                        else
                        {
                            word.MoveNext();
                            return null;
                        }
                    }
*/                    switch (word.Text)
                    {
                        case "=":
                            statement = BlockingAssignment.ParseCreate(word, nameSpace, expression);
                            break;
                        case "<=":
                            statement = NonBlockingAssignment.ParseCreate(word, nameSpace, expression);
                            break;
                        default:
                            word.AddError("illegal module item");
                            return null;
                    }
                    if(word.GetCharAt(0) != ';')
                    {
                        word.AddError("; expected");
                    }
                    else
                    {
                        word.MoveNext();
                    }
                    return statement;
            }
            return null;
        }

        public static IStatement ParseCreateStatementOrNull(WordScanner word, NameSpace nameSpace)
        {
            if(word.GetCharAt(0) == ';')
            {
                word.MoveNext();
                return null;
            }
            return ParseCreateStatement(word, nameSpace);
        }


        public static IStatement ParseCreateFunctionStatement(WordScanner word, NameSpace nameSpace)
        {
            return ParseCreateStatement(word,nameSpace);
        }
    }


}
