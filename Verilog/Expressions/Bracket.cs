﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class Bracket : Primary
    {
        protected Bracket() { }

        public Expression Expression { get; protected set; }

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText("(");
            if (Expression != null) label.AppendLabel(Expression.GetLabel());
            label.AppendText(")");
            return label;
        }

        public static Primary ParseCreateBracketOrMinTypMax(WordScanner word, NameSpace nameSpace)
        {
            word.MoveNext();
            if (word.Eof)
            {
                word.AddError("illegal bracket");
                return null;
            }
            Expression exp1 = Expression.ParseCreate(word, nameSpace);
            if (exp1 == null)
            {
                word.AddError("illegal bracket");
                return null;
            }
            if (word.Eof)
            {
                word.AddError("illegal bracket");
                return null;
            }
            if (word.GetCharAt(0) == ':')
            {
                return MinTypMax.ParseCreate(word, nameSpace, exp1);
            }
            if (word.Eof | word.GetCharAt(0) != ')')
            {
                word.AddError("illegal bracket");
                return null;
            }
            word.MoveNext();
            Bracket bracket = new Bracket();
            bracket.Expression = exp1;
            bracket.Constant = exp1.Constant;
            bracket.BitWidth = exp1.BitWidth;
            bracket.Value = exp1.Value;
            return bracket;
        }
    }
}
