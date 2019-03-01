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

            Function function = null;
            if (nameSpace.Module.Functions.ContainsKey(functionCall.FunctionName))
            {
                function = nameSpace.Module.Functions[functionCall.FunctionName];
            }
            else
            {
                word.AddError("undefined");
            }

            word.Color(CodeDrawStyle.ColorType.Identifier);
            word.MoveNext();

            if(word.GetCharAt(0) != '(')
            {
                word.AddError("illegal function call");
                return null;
            }
            word.MoveNext();

            int i = 0;
            while (!word.Eof)
            {
                Expression expression = Expression.ParseCreate(word, nameSpace);
                if(expression == null)
                {
                    return null;
                }
                functionCall.Expressions.Add(expression);
                if(function != null)
                {
                    if (i > function.Ports.Count)
                    {
                        word.AddError("illegal argument");
                    }
                    else
                    {
                        if(function.PortsList[i] != null && expression != null & function.PortsList[i].Range != null && function.PortsList[i].Range.BitWidth != expression.BitWidth)
                        {
                            word.AddWarning("bitwidth mismatch");
                        }
                    }
                }


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
                i++;
            }
            return functionCall;
        }
    }
}
