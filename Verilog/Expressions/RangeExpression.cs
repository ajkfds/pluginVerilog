﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    //  range_expression ::=    expression        
    //                          | msb_constant_expression : lsb_constant_expression
    //                          | base_expression +: width_constant_expression
    //                          | base_expression -: width_constant_expression 
    //         | hierarchical_identifier          | hierarchical_identifier [ expression ] { [ expression ] }          | hierarchical_identifier [ expression ] { [ expression ] }  [ range_expression ]          | hierarchical_identifier [ range_expression ]  
    public class RangeExpression
    {
        public int BitWidth;
        public virtual ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            AppendLabel(label);
            return label;
        }

        public virtual void AppendLabel(ajkControls.ColorLabel label)
        {
        }
    }
    public class SingleBitRangeExpression : RangeExpression
    {
        protected SingleBitRangeExpression() { }
        public SingleBitRangeExpression(Expression expression)
        {
            Expression = expression;
            BitWidth = 1;
        }
        public Expression Expression;
        public override void AppendLabel(ajkControls.ColorLabel label)
        {
            label.AppendText("[");
            label.AppendLabel(Expression.GetLabel());
            label.AppendText("]");
        }
    }
    public class AbsoluteRangeExpression : RangeExpression
    {
        protected AbsoluteRangeExpression() { }
        public AbsoluteRangeExpression(Expression expression1, Expression expression2)
        {
            MsbExpression = expression1;
            LsbExpression = expression2;
        }
        public Expression MsbExpression;
        public Expression LsbExpression;
        public override void AppendLabel(ajkControls.ColorLabel label)
        {
            label.AppendText("[");
            label.AppendLabel(MsbExpression.GetLabel());
            label.AppendText(":");
            label.AppendLabel(LsbExpression.GetLabel());
            label.AppendText("]");
        }
    }
    public class RelativePlusRangeExpression : RangeExpression
    {
        protected RelativePlusRangeExpression() { }
        public RelativePlusRangeExpression(Expression expression1, Expression expression2)
        {
            BaseExpression = expression1;
            WidthExpression = expression2;
        }
        public Expression BaseExpression;
        public Expression WidthExpression;

        public override void AppendLabel(ajkControls.ColorLabel label)
        {
            label.AppendText("[");
            label.AppendLabel(BaseExpression.GetLabel());
            label.AppendText("+:");
            label.AppendLabel(WidthExpression.GetLabel());
            label.AppendText("]");
        }
    }
    public class RelativeMinusRangeExpression : RangeExpression
    {
        protected RelativeMinusRangeExpression() { }
        public RelativeMinusRangeExpression(Expression expression1, Expression expression2)
        {
            BaseExpression = expression1;
            WidthExpression = expression2;
        }
        public Expression BaseExpression;
        public Expression WidthExpression;

        public override void AppendLabel(ajkControls.ColorLabel label)
        {
            label.AppendText("[");
            label.AppendLabel(BaseExpression.GetLabel());
            label.AppendText("-:");
            label.AppendLabel(WidthExpression.GetLabel());
            label.AppendText("]");
        }
    }
}
