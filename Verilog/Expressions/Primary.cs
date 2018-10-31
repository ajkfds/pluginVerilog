using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class Primary : ExpressionItem
    {
        protected Primary() { }
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
                    break;
                case WordPointer.WordTypeEnum.Text:
                    {
                        var variable = VariableReference.ParseCreate(word, nameSpace);
                        if (variable != null) return variable;

                        var parameter = ParameterReference.ParseCreate(word, nameSpace);
                        if (parameter != null) return parameter;

                        if(word.NextText == "(")
                        {
                            var functionCall = FunctionCall.ParseCreate(word, nameSpace);
                        }
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

        public new static ParameterReference ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            Variables.Parameter parameter = nameSpace.GetParameter(word.Text);
            if (parameter == null) return null;

            ParameterReference val = new ParameterReference();
            val.ParameterName = word.Text;

            word.Color((byte)Style.Color.Paramater);
            word.MoveNext();

            return val;
        }
    }

    public class VariableReference : Primary
    {
        protected VariableReference() { }
        public string VariableName { get; protected set; }
        public Variables.Range Range { get; protected set; }

        public new static VariableReference ParseCreate(WordScanner word,NameSpace nameSpace)
        {
            Variables.Variable variable = nameSpace.GetVariable(word.Text);
            if (variable == null) return null;

            VariableReference val = new VariableReference();
            val.VariableName = variable.Name;

            if(variable is Variables.Reg)
            {
                word.Color((byte)Style.Color.Register);
            }
            else if (variable is Variables.Net)
            {
                word.Color((byte)Style.Color.Net);
            }
            else
            {
                word.Color((byte)Style.Color.Net);
            }
            word.MoveNext();

            if(word.GetCharAt(0) == '[')
            {
                val.Range = Variables.Range.ParseCreate(word, nameSpace);
                if (val.Range == null) return null;
            }

            return val;
        }

    }



    public class Bracket : Primary
    {
        protected Bracket() { }

        public Expression Expression { get; protected set; }

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



