using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class VariableReference : Primary
    {
        protected VariableReference() { }
        public string VariableName { get; protected set; }
        public RangeExpression RangeExpression { get; protected set; }
        public List<Expression> Dimensions = new List<Expression>();
        public Variables.Variable Variable = null;

        public override string CreateString()
        {
            return GetLabel().CreateString();
        }
        public override void AppendLabel(ajkControls.ColorLabel label)
        {
            if (Variable is Variables.Reg)
            {
                label.AppendText(VariableName, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Register));
            }
            else if (Variable is Variables.Net)
            {
                label.AppendText(VariableName, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Net));
            }
            else
            {
                label.AppendText(VariableName);
            }
            if (RangeExpression != null)
            {
                label.AppendText(" ");
                label.AppendLabel(RangeExpression.GetLabel());
            }
            foreach (Expression expression in Dimensions)
            {
                label.AppendText(" [");
                label.AppendLabel(expression.GetLabel());
                label.AppendText("]");
            }
        }

        public override void AppendString(StringBuilder stringBuilder)
        {
            stringBuilder.Append(CreateString());
        }


        private static Variables.Variable getVariable(WordScanner word, string identifier, NameSpace nameSpace)
        {
            if (nameSpace.Variables.ContainsKey(identifier))
            {
                return nameSpace.Variables[identifier];
            }

            if (nameSpace is Function)
            {
                if (nameSpace.Parent != null)
                {
                    Variables.Variable val = nameSpace.Parent.GetVariable(identifier);
                    if (val != null) word.AddWarning("external function reference");
                    return val;
                }
            }
            else
            {
                if (nameSpace.Parent != null)
                {
                    return nameSpace.Parent.GetVariable(identifier);
                }
            }
            return null;
        }

        public new static VariableReference ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            throw new Exception("illegal access");
        }

        public static VariableReference ParseCreate(WordScanner word, NameSpace nameSpace, bool assigned)
        {
            if (nameSpace == null) System.Diagnostics.Debugger.Break();
            Variables.Variable variable = getVariable(word, word.Text, nameSpace);
            if (variable == null) return null;

            VariableReference val = new VariableReference();
            val.VariableName = variable.Name;
            val.Variable = variable;

            if (variable is Variables.Reg)
            {
                word.Color(CodeDrawStyle.ColorType.Register);
            }
            else if (variable is Variables.Net)
            {
                word.Color(CodeDrawStyle.ColorType.Net);
            }
            else
            {
                word.Color(CodeDrawStyle.ColorType.Net);
            }

            if (assigned)
            {
                val.Variable.AssignedReferences.Add(word.GetReference());
            }
            else
            {
                val.Variable.UsedReferences.Add(word.GetReference());
            }

            word.MoveNext();

            while (!word.Eof && val.Dimensions.Count < variable.Dimensions.Count)
            {
                if (word.GetCharAt(0) != '[')
                {
                    word.AddError("lacked dimension");
                    break;
                }
                word.MoveNext();
                Expression exp = Expression.ParseCreate(word, nameSpace);
                val.Dimensions.Add(exp);
                if (word.GetCharAt(0) != ']')
                {
                    word.AddError("illegal dimension");
                    break;
                }
                word.MoveNext();
            }

            if (word.GetCharAt(0) == '[')
            {
                word.MoveNext();

                Expression exp1 = Expression.ParseCreate(word, nameSpace);
                Expression exp2;
                switch (word.Text)
                {
                    case ":":
                        word.MoveNext();
                        exp2 = Expression.ParseCreate(word, nameSpace);
                        if (word.Text != "]")
                        {
                            word.AddError("illegal range");
                            return null;
                        }
                        word.MoveNext();
                        val.RangeExpression = new AbsoluteRangeExpression(exp1, exp2);
                        break;
                    case "+:":
                        word.MoveNext();
                        exp2 = Expression.ParseCreate(word, nameSpace);
                        if (word.Text != "]")
                        {
                            word.AddError("illegal range");
                            return null;
                        }
                        word.MoveNext();
                        val.RangeExpression = new RelativePlusRangeExpression(exp1, exp2);
                        break;
                    case "-:":
                        word.MoveNext();
                        exp2 = Expression.ParseCreate(word, nameSpace);
                        if (word.Text != "]")
                        {
                            word.AddError("illegal range");
                            return null;
                        }
                        word.MoveNext();
                        val.RangeExpression = new RelativeMinusRangeExpression(exp1, exp2);
                        break;
                    case "]":
                        word.MoveNext();
                        val.RangeExpression = new SingleBitRangeExpression(exp1);
                        break;
                    default:
                        word.AddError("illegal range/dimension");
                        return null;
                }
            }
            else
            {   // w/o range
                if (variable is Variables.Reg)
                {
                    if (((Variables.Reg)variable).Range != null) val.BitWidth = ((Variables.Reg)variable).Range.BitWidth;
                    else val.BitWidth = 1;
                }
                else if (variable is Variables.Net)
                {
                    if (((Variables.Net)variable).Range != null) val.BitWidth = ((Variables.Net)variable).Range.BitWidth;
                    else val.BitWidth = 1;
                }
                else if (variable is Variables.Genvar)
                {
                    val.Constant = true;
                }
            }

            return val;
        }

    }
}
