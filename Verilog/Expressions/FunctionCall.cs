using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class FunctionCall : Primary
    {
        protected FunctionCall() { }
        public List<Expression> Expressions = new List<Expression>();
        public WordReference Reference { get; protected set; }
        public string FunctionName { get; protected set; }

        public new static FunctionCall ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            FunctionCall functionCall = new FunctionCall();
            functionCall.Reference = word.GetReference();
            functionCall.FunctionName = word.Text;

            word.Color(CodeDrawStyle.ColorType.Identifier);
            word.MoveNext();

            if(word.GetCharAt(0) != '(')
            {
                word.AddError("illegal function call");
                return null;
            }
            word.MoveNext();

            while (!word.Eof)
            {
                Expression expression = Expression.ParseCreate(word, nameSpace);
                if(expression == null)
                {
                    return null;
                }
                functionCall.Expressions.Add(expression);

                if(word.Text == ")")
                {
                    word.MoveNext();
                    break;
                }
                else if(word.Text == ",")
                {
                    word.MoveNext();
                }
                else
                {
                    word.AddError("illegal function call");
                    return null;
                }
            }
            nameSpace.Module.functionCalls.Add(functionCall);
            return functionCall;
        }
    }
}
