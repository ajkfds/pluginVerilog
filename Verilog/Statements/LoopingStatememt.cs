using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Statements
{
    public class ForeverStatement : Statement
    {
        protected ForeverStatement() { }
        public Statement Statement;
        //A.6.8 Looping statements
        //function_loop_statement ::= forever function_statement          
        //                            | repeat(expression ) function_statement
        //                            | while (expression ) function_statement
        //                            | for (variable_assignment ;  expression ; variable_assignment ) function_statement
        //loop_statement   ::= forever statement
        //                            | repeat (expression ) statement
        //                            | while (expression ) statement
        //                            | for (variable_assignment ; expression ; variable_assignment ) statement
        public static ForeverStatement ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            ForeverStatement foreverStatement = new ForeverStatement();
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext();

            foreverStatement.Statement = Statement.ParseCreateStatement(word, nameSpace);

            return foreverStatement;
        }
    }

    public class RepeatStatement : Statement
    {
        protected RepeatStatement() { }
        public Expressions.Expression Expression;
        public Statement Statement;
        //A.6.8 Looping statements
        //function_loop_statement ::= forever function_statement          
        //                            | repeat(expression ) function_statement
        //                            | while (expression ) function_statement
        //                            | for (variable_assignment ;  expression ; variable_assignment ) function_statement
        //loop_statement   ::= forever statement
        //                            | repeat (expression ) statement
        //                            | while (expression ) statement
        //                            | for (variable_assignment ; expression ; variable_assignment ) statement
        public static RepeatStatement ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            RepeatStatement repeatStatement = new RepeatStatement();
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext();

            if (word.GetCharAt(0) != '(')
            {
                word.AddError("( expected");
                return null;
            }

            repeatStatement.Expression = Expressions.Expression.ParseCreate(word, nameSpace);

            if (word.GetCharAt(0) != ')')
            {
                word.AddError("( expected");
                return null;
            }

            repeatStatement.Statement = Statement.ParseCreateStatement(word, nameSpace);

            return repeatStatement;
        }
    }

    public class WhileStatememt : Statement
    {
        protected WhileStatememt() { }
        public Expressions.Expression Expression;
        public Statement Statement;
        //A.6.8 Looping statements
        //function_loop_statement ::= forever function_statement          
        //                            | repeat(expression ) function_statement
        //                            | while (expression ) function_statement
        //                            | for (variable_assignment ;  expression ; variable_assignment ) function_statement
        //loop_statement   ::= forever statement
        //                            | repeat (expression ) statement
        //                            | while (expression ) statement
        //                            | for (variable_assignment ; expression ; variable_assignment ) statement
        public static WhileStatememt ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            WhileStatememt whileStatement = new WhileStatememt();
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext();

            if (word.GetCharAt(0) != '(')
            {
                word.AddError("( expected");
                return null;
            }

            whileStatement.Expression = Expressions.Expression.ParseCreate(word, nameSpace);

            if (word.GetCharAt(0) != ')')
            {
                word.AddError("( expected");
                return null;
            }

            whileStatement.Statement = Statement.ParseCreateStatement(word, nameSpace);

            return whileStatement;
        }
    }

    public class ForStatememt : Statement
    {
        protected ForStatememt() { }
        public Statement Statement;

        public Variables.VariableAssignment VariableAssignment;
        public Expressions.Expression Expression;
        public Variables.VariableAssignment VariableUpdate;

        //A.6.8 Looping statements
        //function_loop_statement ::= forever function_statement          
        //                            | repeat(expression ) function_statement
        //                            | while (expression ) function_statement
        //                            | for (variable_assignment ;  expression ; variable_assignment ) function_statement
        //loop_statement   ::= forever statement
        //                            | repeat (expression ) statement
        //                            | while (expression ) statement
        //                            | for (variable_assignment ; expression ; variable_assignment ) statement
        public static ForStatememt ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            ForStatememt forStatement = new ForStatememt();
            word.Color((byte)Style.Color.Keyword);
            word.MoveNext();

            if (word.GetCharAt(0) != '(')
            {
                word.AddError("( expected");
                return null;
            }
            forStatement.VariableAssignment = Variables.VariableAssignment.ParseCreate(word, nameSpace);
            if (word.GetCharAt(0) != ';')
            {
                word.AddError("( expected");
                return null;
            }
            forStatement.Expression = Expressions.Expression.ParseCreate(word, nameSpace);

            if (word.GetCharAt(0) != ';')
            {
                word.AddError("( expected");
                return null;
            }
            forStatement.VariableUpdate = Variables.VariableAssignment.ParseCreate(word, nameSpace);

            if (word.GetCharAt(0) != ')')
            {
                word.AddError("( expected");
                return null;
            }


            forStatement.Statement = Statement.ParseCreateStatement(word, nameSpace);
            return forStatement;
        }
    }

}
