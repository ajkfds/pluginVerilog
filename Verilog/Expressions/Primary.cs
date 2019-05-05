using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class Primary : ExpressionItem
    {
        protected Primary() {
            Constant = false;
        }
        public virtual bool Constant { get; protected set; }
        public virtual double? Value { get; protected set; }
        public virtual int? BitWidth { get; protected set; }
        //        public bool Signed { get; protected set; }

        public static Primary Create(bool constant, double? value, int? bitWidth)
        {
            Primary primary = new Primary();
            primary.Constant = constant;
            primary.Value = value;
            primary.BitWidth = bitWidth;
            return primary;
        }

        public virtual ajkControls.ColorLabel GetLabel()
        {
            return null;
        }

        /*
         * 
         * 
         A.8.4 Primaries
        constant_primary    ::= constant_concatenation
                                | constant_function_call
                                | ( constant_mintypmax_expression )
                                | constant_multiple_concatenation
                                | genvar_identifier
                                | number
                                | parameter_identifier
                                | specparam_identifier  
        module_path_primary ::= number
                                | identifier
                                | module_path_concatenation
                                | module_path_multiple_concatenation
                                | function_call          
                                | system_function_call          
                                | constant_function_call          
                                | ( module_path_mintypmax_expression )  
        primary             ::= number
                                | concatenation          
                                | multiple_concatenation
                                | function_call 
                                | system_function_call
                                | constant_function_call
                                | ( mintypmax_expression )
                                | hierarchical_identifier
                                | hierarchical_identifier [ expression ] { [ expression ] }
                                | hierarchical_identifier [ expression ] { [ expression ] }  [ range_expression ]
                                | hierarchical_identifier [ range_expression ]
        */
        public static Primary ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (nameSpace == null) System.Diagnostics.Debugger.Break();
            switch (word.WordType)
            {
                case WordPointer.WordTypeEnum.Number:
                    return Number.ParseCreate(word);
                case WordPointer.WordTypeEnum.Symbol:
                    if (word.GetCharAt(0) == '{')
                    {
                        return Concatenation.ParseCreateConcatenationOrMultipleConcatenation(word, nameSpace);
                    }else if(word.GetCharAt(0) == '(')
                    {
                        return Bracket.ParseCreateBracketOrMinTypMax(word, nameSpace);
                    }
                    return null;
                case WordPointer.WordTypeEnum.String:
                    return ConstantString.ParseCreate(word);
                case WordPointer.WordTypeEnum.Text:
                    {
                        var variable = VariableReference.ParseCreate(word, nameSpace);
                        if (variable != null) return variable;

                        var parameter = ParameterReference.ParseCreate(word, nameSpace);
                        if (parameter != null) return parameter;

                        if (word.Text.StartsWith("$") && word.RootParsedDocument.ProjectProperty.SystemFunctions.Keys.Contains(word.Text)) {
                            return FunctionCall.ParseCreate(word, nameSpace);
                        }

                        if (word.NextText == "(")
                        {
                            return FunctionCall.ParseCreate(word, nameSpace);
                        }

                        if(word.NextText == ".")
                        {
                            if (nameSpace.Module.ModuleInstantiations.ContainsKey(word.Text))
                            {
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                string moduleName = nameSpace.Module.ModuleInstantiations[word.Text].ModuleName;
                                Module module = word.RootParsedDocument.ProjectProperty.GetModule(moduleName);
                                if (module == null) return null;
                                word.MoveNext();
                                word.MoveNext(); // .

                                Primary primary = subParseCreate(word, module);
                                if (primary == null) word.AddError("illegal variable");
                                return primary;
                            }
                            if (nameSpace.NameSpaces.ContainsKey(word.Text))
                            {
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                NameSpace space = nameSpace.NameSpaces[word.Text];
                                if (space == null) return null;
                                word.MoveNext();
                                word.MoveNext(); // .

                                Primary primary = subParseCreate(word, space);
                                if (primary == null) word.AddError("illegal variable");
                                return primary;
                            }
                        }

                        if(word.Eof) return null;
                        if (General.ListOfKeywords.Contains(word.Text)) return null;
                        if(General.IsIdentifier(word.Text) && !nameSpace.Variables.ContainsKey(word.Text) && !word.Prototype)
                        {   // undefined net
                            word.AddWarning("undefined");
                            Variables.Net net = new Variables.Net();
                            net.Name = word.Text;
                            net.Signed = false;
                            if (word.Active)
                            {
                                nameSpace.Variables.Add(net.Name, net);
                            }
                            variable = VariableReference.ParseCreate(word, nameSpace);
                            if (variable != null) return variable;
                        }
                    }
                    break;
            }
            return null;
        }

        private static Primary subParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (nameSpace == null) System.Diagnostics.Debugger.Break();
            switch (word.WordType)
            {
                case WordPointer.WordTypeEnum.Number:
                    return null;
                case WordPointer.WordTypeEnum.Symbol:
                    return null;
                case WordPointer.WordTypeEnum.String:
                    return null;
                case WordPointer.WordTypeEnum.Text:
                    {
                        var variable = VariableReference.ParseCreate(word, nameSpace);
                        if (variable != null) return variable;

                        var parameter = ParameterReference.ParseCreate(word, nameSpace);
                        if (parameter != null) return parameter;

                        if (word.NextText == "(")
                        {
                            return FunctionCall.ParseCreate(word, nameSpace);
                        }

                        if (word.NextText == ".")
                        {
                            if (nameSpace.Module.ModuleInstantiations.ContainsKey(word.Text))
                            {
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                string moduleName = nameSpace.Module.ModuleInstantiations[word.Text].ModuleName;
                                Module module = word.RootParsedDocument.ProjectProperty.GetModule(moduleName);
                                if (module == null) return null;
                                word.MoveNext();
                                word.MoveNext(); // .

                                Primary primary = subParseCreate(word, module);
                                if (primary == null) word.AddError("illegal variable");
                                return primary;
                            }
                            if (nameSpace.NameSpaces.ContainsKey(word.Text))
                            {
                                word.Color(CodeDrawStyle.ColorType.Identifier);
                                NameSpace space = nameSpace.NameSpaces[word.Text];
                                if (space == null) return null;
                                word.MoveNext();
                                word.MoveNext(); // .

                                Primary primary = subParseCreate(word, space);
                                if (primary == null) word.AddError("illegal variable");
                                return primary;
                            }
                        }

                        if (word.Eof) return null;
                        if (General.ListOfKeywords.Contains(word.Text)) return null;
                    }
                    break;
            }
            return null;
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

            if(parameter.Expression != null) val.Value = parameter.Expression.Value;

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

    public class VariableReference : Primary
    {
        protected VariableReference() { }
        public string VariableName { get; protected set; }
        public RangeExpression RangeExpression { get; protected set; }
        public List<Expression> Dimensions = new List<Expression>();
        public Variables.Variable Variable = null;

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();

            if(Variable is Variables.Reg)
            {
                label.AppendText(VariableName,CodeDrawStyle.Color(CodeDrawStyle.ColorType.Register));
            }
            else if(Variable is Variables.Net)
            {
                label.AppendText(VariableName, CodeDrawStyle.Color(CodeDrawStyle.ColorType.Net));
            }
            else
            {
                label.AppendText(VariableName);
            }
            if(RangeExpression != null)
            {
                label.AppendText(" ");
                label.AppendLabel(RangeExpression.GetLabel());
            }
            foreach(Expression expression in Dimensions)
            {
                label.AppendText(" [");
                label.AppendLabel(expression.GetLabel());
                label.AppendText("]");
            }
            return label;
        }


        private static Variables.Variable getVariable(WordScanner word, string identifier, NameSpace nameSpace)
        {
            if (nameSpace.Variables.ContainsKey(identifier))
            {
                return nameSpace.Variables[identifier];
            }

            if(nameSpace is Function)
            {
                if (nameSpace.Parent != null)
                {
                    Variables.Variable val =  nameSpace.Parent.GetVariable(identifier);
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

        public new static VariableReference ParseCreate(WordScanner word,NameSpace nameSpace)
        {
            if (nameSpace == null) System.Diagnostics.Debugger.Break();
            Variables.Variable variable = getVariable(word,word.Text, nameSpace);
            if (variable == null) return null;

            VariableReference val = new VariableReference();
            val.VariableName = variable.Name;
            val.Variable = variable;

            if(variable is Variables.Reg)
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
            word.MoveNext();

            while(!word.Eof && val.Dimensions.Count < variable.Dimensions.Count)
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
                    if(((Variables.Reg)variable).Range != null) val.BitWidth = ((Variables.Reg)variable).Range.BitWidth;
                    else val.BitWidth = 1;
                }
                else if (variable is Variables.Net)
                {
                    if (((Variables.Net)variable).Range != null) val.BitWidth = ((Variables.Net)variable).Range.BitWidth;
                    else val.BitWidth = 1;
                }else if(variable is Variables.Genvar)
                {
                    val.Constant = true;
                }
            }

            return val;
        }

    }
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
            return null;
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
        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText("[");
            label.AppendLabel(Expression.GetLabel());
            label.AppendText("]");
            return label;
        }
    }
    public class AbsoluteRangeExpression : RangeExpression
    {
        protected AbsoluteRangeExpression() { }
        public AbsoluteRangeExpression(Expression expression1,Expression expression2)
        {
            MsbExpression = expression1;
            LsbExpression = expression2;
        }
        public Expression MsbExpression;
        public Expression LsbExpression;
        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText("[");
            label.AppendLabel(MsbExpression.GetLabel());
            label.AppendText(":");
            label.AppendLabel(LsbExpression.GetLabel());
            label.AppendText("]");
            return label;
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

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText("[");
            label.AppendLabel(BaseExpression.GetLabel());
            label.AppendText("+:");
            label.AppendLabel(WidthExpression.GetLabel());
            label.AppendText("]");
            return label;
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

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText("[");
            label.AppendLabel(BaseExpression.GetLabel());
            label.AppendText("-:");
            label.AppendLabel(WidthExpression.GetLabel());
            label.AppendText("]");
            return label;
        }
    }


    public class Bracket : Primary
    {
        protected Bracket() { }

        public Expression Expression { get; protected set; }

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText("(");
            if(Expression != null) label.AppendLabel(Expression.GetLabel());
            label.AppendText(")");
            return label;
        }

        public static Primary ParseCreateBracketOrMinTypMax(WordScanner word,NameSpace nameSpace)
        {
            word.MoveNext();
            if (word.Eof)
            {
                word.AddError("illegal bracket");
                return null;
            }
            Expression exp1 =  Expression.ParseCreate(word, nameSpace);
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

    public class MinTypMax : Primary
    {
        protected MinTypMax() { }
        public Expression MinExpression { get; protected set; }
        public Expression TypExpression { get; protected set; }
        public Expression MaxExpression { get; protected set; }

        public static MinTypMax ParseCreate(WordScanner word,NameSpace nameSpace,Expression minExpresstion)
        {
            word.MoveNext();
            if (word.Eof)
            {
                word.AddError("illegal MinTypMax");
                return null;
            }
            Expression exp2 = Expression.ParseCreate(word, nameSpace);
            if (exp2 == null)
            {
                word.AddError("illegal MinTypMax");
                return null;
            }
            if (word.Eof)
            {
                word.AddError("illegal MinTypMax");
                return null;
            }
            if (word.GetCharAt(0) != ':')
            {
                word.AddError("illegal MinTypMax");
                return null;
            }
            word.MoveNext();
            if (word.Eof)
            {
                word.AddError("illegal MinTypMax");
                return null;
            }
            Expression exp3 = Expression.ParseCreate(word, nameSpace);
            if (exp3 == null)
            {
                word.AddError("illegal MinTypMax");
                return null;
            }
            if (word.Eof | word.GetCharAt(0) != ')')
            {
                word.AddError("illegal MinTypMax");
                return null;
            }

            MinTypMax minTypMax = new MinTypMax();
            minTypMax.MinExpression = minExpresstion;
            minTypMax.TypExpression = exp2;
            minTypMax.MaxExpression = exp3;

            return minTypMax;
        }
    }

    public class Concatenation : Primary
    {
        protected Concatenation() { }
        public List<Expression> Expressions = new List<Expression>();


        public static Primary ParseCreateConcatenationOrMultipleConcatenation(WordScanner word,NameSpace nameSpace)
        {
            word.MoveNext(); // {

            Expression exp1 = Expression.ParseCreate(word, nameSpace);
            if(exp1 == null)
            {
                word.AddError("illegal concatenation");
                return null;
            }
            if(word.GetCharAt(0) == '{')
            {
                return MultipleConcatenation.ParseCreate(word, nameSpace, exp1);
            }
            Concatenation concatenation = new Concatenation();
            concatenation.Expressions.Add(exp1);
            concatenation.Constant = exp1.Constant;
            concatenation.BitWidth = exp1.BitWidth;

            while(word.GetCharAt(0) != '}')
            {
                if (word.GetCharAt(0) != ',')
                {
                    return null;
                }
                word.MoveNext();

                if (word.Eof)
                {
                    word.AddError("illegal concatenation");
                    return null;
                }
                exp1 = Expression.ParseCreate(word, nameSpace);
                if (exp1 != null)
                {
                    concatenation.Constant = concatenation.Constant & exp1.Constant;
                    concatenation.BitWidth = concatenation.BitWidth + exp1.BitWidth;
                }
                if (exp1 == null)
                {
                    word.AddError("illegal concatenation");
                    return null;
                }
            }
            word.MoveNext(); // }
            return concatenation;
        }
        /*
        concatenation           ::= { expression { , expression } }
        multiple_concatenation  ::= { constant_expression concatenation }

        constant_concatenation  ::= { constant_expression { , constant_expression } }
        constant_multiple_concatenation ::= { constant_expression constant_concatenation }

        module_path_concatenation ::= { module_path_expression { , module_path_expression } }
        module_path_multiple_concatenation ::= { constant_expression module_path_concatenation }


        net_concatenation       ::= { net_concatenation_value { , net_concatenation_value } }
        net_concatenation_value ::= hierarchical_net_identifier
                                    | hierarchical_net_identifier [ expression ] { [ expression ] }
                                    | hierarchical_net_identifier [ expression ] { [ expression ] } [ range_expression ]
                                    | hierarchical_net_identifier [ range_expression ]
                                    | net_concatenation  

        variable_concatenation ::= { variable_concatenation_value { , variable_concatenation_value } }  
        variable_concatenation_value    ::= hierarchical_variable_identifier
                                        | hierarchical_variable_identifier [ expression ] { [ expression ] }
                                        | hierarchical_variable_identifier [ expression ] { [ expression ] } [ range_expression ]
                                        | hierarchical_variable_identifier [ range_expression ]          
                                        | variable_concatenation  
        */
    }

    public class MultipleConcatenation : Primary
    {
        protected MultipleConcatenation() { }

        public Expression MultipleExpression { get; protected set; }
        public Expression Expression { get; protected set; }

        public static MultipleConcatenation ParseCreate(WordScanner word, NameSpace nameSpace,Expression multipleExpression)
        {
            word.MoveNext(); // {

            Expression exp = Expression.ParseCreate(word, nameSpace);
            if(word.Eof || word.GetCharAt(0) != '}')
            {
                word.AddError("illegal multiple concatenation");
                return null;
            }
            word.MoveNext(); // }

            if(word.Eof || word.GetCharAt(0) != '}')
            {
                word.AddError("illegal multiple concatenation");
                return null;
            }
            word.MoveNext(); // }

            MultipleConcatenation multipleConcatenation = new MultipleConcatenation();
            multipleConcatenation.MultipleExpression = multipleExpression;
            multipleConcatenation.Expression = exp;
            return multipleConcatenation;
        }

    }


}



