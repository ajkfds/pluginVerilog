using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class ConditionalExpression : Expression
    {
        public Expression ConditionExpression;
        public Expression TrueExpreddion;
        public Expression FalseExpreddion;


        protected ConditionalExpression() { }
        // conditional_expression::= expression1 ? { attribute_instance } expression2: expression3

        public static ConditionalExpression ParseCreate(WordScanner word,NameSpace nameSpace,Expression conditionExpresstion)
        {
            if (word.GetCharAt(0) != '?') return null;
            word.MoveNext();

            ConditionalExpression conditionalExpression = new ConditionalExpression();
            conditionalExpression.TrueExpreddion = Expression.ParseCreate(word, nameSpace);
            if(conditionalExpression.TrueExpreddion == null)
            {
                word.AddError("illegal conditional expression");
                return null;
            }

            if(word.GetCharAt(0) == ':')
            {
                word.MoveNext();
            }
            else
            {
                word.MoveNext();
            }

            conditionalExpression.FalseExpreddion = Expression.ParseCreate(word, nameSpace);
            if (conditionalExpression.FalseExpreddion == null)
            {
                word.AddError("illegal conditional expression");
                return null;
            }

            return conditionalExpression;
        }
    }
}
