﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Statements
{
    public class Statement
    {
        protected Statement() { }
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
        public static Statement ParseCreateStatement(WordScanner word, NameSpace nameSpace)
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
                            | { attribute_instance } event_trigger
                            | { attribute_instance } par_block
                            | { attribute_instance } procedural_continuous_assignments ;
                            | { attribute_instance } seq_block
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
                default:
                    Expressions.Expression expression = Expressions.Expression.ParseCreateVariableLValue(word, nameSpace);
                    Statement statement;
                    if (expression == null)
                    {
                        word.MoveNext();
                        break;
                    }
                    switch (word.Text)
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

        public static Statement ParseCreateStatementOrNull(WordScanner word, NameSpace nameSpace)
        {
            if(word.GetCharAt(0) == ';')
            {
                word.MoveNext();
                return null;
            }
            return ParseCreateStatement(word, nameSpace);
        }
        public static Statement ParseCreateFunctionStatement(WordScanner word, NameSpace nameSpace)
        {
            return ParseCreateStatement(word,nameSpace);
        }

        //private parseConditionalStatement(WordScanner word,NameSpace nameSpace)
        //{

        //}
    }

    public class ProceduralTimingControlStatement : Statement
    {
        protected ProceduralTimingControlStatement() { }
        public DelayControl DelayControl { get; protected set; }
        public EventControl EventControl { get; protected set; }
        public Statement Statement { get; protected set; }

        public static ProceduralTimingControlStatement ParseCreate(WordScanner word,NameSpace nameSpace)
        {
            switch (word.Text)
            {
                case "#":
                    {
                        ProceduralTimingControlStatement statement = new ProceduralTimingControlStatement();
                        statement.DelayControl = DelayControl.ParseCreate(word, nameSpace);
                        statement.Statement = Statement.ParseCreateStatementOrNull(word, nameSpace);
                        return statement;
                    }
                case "@":
                    {
                        ProceduralTimingControlStatement statement = new ProceduralTimingControlStatement();
                        statement.EventControl = EventControl.ParseCreate(word, nameSpace);
                        statement.Statement = Statement.ParseCreateStatementOrNull(word, nameSpace);
                        return statement;
                    }
                default:
                    return null;
            }
        }
    }

}
