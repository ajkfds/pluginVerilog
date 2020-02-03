﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class TaskReference : Primary
    {
        public string TaskName { get; protected set; }
        public string ModuleName { get; protected set; }

        public TaskReference(Task task, NameSpace nameSpace)
        {
            TaskName = task.Name;
            ModuleName = nameSpace.Module.Name;
        }
    }

    public class NameSpaceReference : Primary
    {
        public string Name { get; protected set; }
        public NameSpaceReference(NameSpace nameSpace)
        {
            Name = nameSpace.Name;
        }
    }

    public class ModuleInstanceReference : Primary
    {
        ModuleItems.ModuleInstantiation moduleInstantiation;
        public ModuleInstanceReference(ModuleItems.ModuleInstantiation moduleInstantiation)
        {
            this.moduleInstantiation = moduleInstantiation;
        }
    }


    public class ParameterReference : Primary
    {
        protected ParameterReference() { }
        public string ParameterName { get; protected set; }

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText(ParameterName, CodeDrawStyle.Color(CodeDrawStyle.ColorType.Paramater));
            return label;
        }

        public new static ParameterReference ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            Variables.Parameter parameter = nameSpace.GetParameter(word.Text);
            if (parameter == null) return null;

            ParameterReference val = new ParameterReference();
            val.ParameterName = word.Text;
            val.Constant = true;

            word.Color(CodeDrawStyle.ColorType.Paramater);
            word.MoveNext();

            if (parameter.Expression != null) val.Value = parameter.Expression.Value;

            if (word.GetCharAt(0) == '[')
            {
                //                word.AddError("bit select can't used for parameters");
                word.MoveNext();

                Expression exp1 = Expression.ParseCreate(word, nameSpace);
                Expression exp2;
                RangeExpression range;
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
                        range = new AbsoluteRangeExpression(exp1, exp2);
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
                        range = new RelativePlusRangeExpression(exp1, exp2);
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
                        range = new RelativeMinusRangeExpression(exp1, exp2);
                        break;
                    case "]":
                        word.MoveNext();
                        range = new SingleBitRangeExpression(exp1);
                        break;
                    default:
                        word.AddError("illegal range/dimension");
                        return null;
                }
            }

            return val;
        }
    }

}
