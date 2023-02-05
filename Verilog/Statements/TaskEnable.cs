using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Statements
{
    public class TaskEnable : IStatement
    {
        // task_enable ::= (From Annex A - A.6.9) hierarchical_task_identifier [ ( expression { , expression } ) ] ;
        public void DisposeSubReference()
        {
        }
        public static TaskEnable ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            return ParseCreate(word, nameSpace, nameSpace);
        }
        public static TaskEnable ParseCreate(WordScanner word, NameSpace nameSpace,NameSpace taskNameSpace)
        {
            TaskEnable taskEnable = new TaskEnable();

            word.Color(CodeDrawStyle.ColorType.Identifier);
            word.MoveNext();

            if(word.Text == "(")
            {
                word.MoveNext();
                if (word.Text == ")")
                {
                    word.MoveNext();
                    word.AddWarning("remove ()");
                }
                else
                {

                    while (!word.Eof)
                    {
                        Expressions.Expression expression = Expressions.Expression.ParseCreate(word, nameSpace);
                        if (expression == null) word.AddError("missed expression");
                        if (word.Text == ")")
                        {
                            break;
                        }
                        else if (word.Text == ",")
                        {
                            word.MoveNext();
                            continue;
                        }
                        else
                        {
                            word.AddError("illegal expression");
                            return null;
                        }
                    }
                    if (word.Text == ")") word.MoveNext();
                    else word.AddError(") required");
                }
            }

            if (word.Text == ";") word.MoveNext();
            else word.AddError("; required");

            return taskEnable;
        }

        public static TaskEnable ParseCreate(Expressions.TaskReference taskReference,WordScanner word,NameSpace nameSpace)
        {
            TaskEnable taskEnable = new TaskEnable();

            Task task = taskReference.Task;
            int portCount = 0;

            if (word.Text == "(")
            {
                word.MoveNext();
                while (!word.Eof)
                {
                    Expressions.Expression expression;
                    if (task == null)
                    {
                        expression = Expressions.Expression.ParseCreate(word, nameSpace);
                    }
                    else if (portCount == task.PortsList.Count)
                    {
                        word.AddError("too many expressions");
                        expression = Expressions.Expression.ParseCreate(word, nameSpace);
                    }
                    else if (portCount > task.PortsList.Count)
                    {
                        expression = Expressions.Expression.ParseCreate(word, nameSpace);
                    }
                    else
                    {
                        Verilog.Variables.Port port = task.PortsList[portCount];
                        if (port.Direction == Variables.Port.DirectionEnum.Input)
                        {
                            expression = Expressions.Expression.ParseCreate(word, nameSpace);
                        }
                        else
                        {
                            expression = Expressions.Expression.ParseCreateVariableLValue(word, nameSpace);
                        }
                    }

                    if (expression == null) word.AddError("missed expression");
                    if (word.Text == ")")
                    {
                        break;
                    }
                    else if (word.Text == ",")
                    {
                        word.MoveNext();
                        portCount++;
                        continue;
                    }
                    else
                    {
                        word.AddError("illegal expression");
                        return null;
                    }
                }
                if (word.Text == ")") word.MoveNext();
                else word.AddError(") required");
            }

            if (word.Text == ";")
            {
                if (task != null && task.Ports.Count != 0) word.AddError("missing ports.");
                word.MoveNext();
            }
            else word.AddError("; required");

            return taskEnable;
        }
    }
}
