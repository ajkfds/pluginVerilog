using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Statements
{
    public class TaskEnable : IStatement
    {
        public static TaskEnable ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            TaskEnable taskEnable = new TaskEnable();

            word.Color(CodeDrawStyle.ColorType.Identifier);
            word.MoveNext();

            if(word.Text != "(")
            {
                word.AddError("( required");
                return null;
            }
            word.MoveNext();

            while(!word.Eof)
            {
                Expressions.Expression expression = Expressions.Expression.ParseCreate(word, nameSpace);
                if(word.Text == ")")
                {
                    break;
                }else if(word.Text == ",")
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
            if(word.Text == ")")
            {
                word.MoveNext();
            }
            else
            {
                word.AddError(") required");
            }

            return taskEnable;
        }
    }
}
