using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class Expression
    {
        protected Expression() { }

        public List<ExpressionItem> RpnExpressionItems = new List<ExpressionItem>();

        /*
        A.8.3 Expressions
        base_expression                 ::= expression
        expression1                     ::= expression 
        expression2                     ::= expression 
        expression3                     ::= expression 
        conditional_expression          ::= expression1 ? { attribute_instance } expression2 : expression3

        expression                      ::= primary 
                                            | unary_operator { attribute_instance } primary 
                                            | expression binary_operator { attribute_instance } expression 
                                            | conditional_expression 
                                            | string  

        constant_base_expression        ::= constant_expression
        constant_expression             ::= constant_primary          | unary_operator { attribute_instance } constant_primary          | constant_expression binary_operator { attribute_instance } constant_expression          | constant_expression ? { attribute_instance } constant_expression : constant_expression          | string
        constant_mintypmax_expression   ::= constant_expression          | constant_expression : constant_expression : constant_expression
        constant_range_expression       ::= constant_expression        | msb_constant_expression : lsb_constant_expression          | constant_base_expression +: width_constant_expression          | constant_base_expression -: width_constant_expression
        dimension_constant_expression   ::= constant_expression  
        lsb_constant_expression         ::= constant_expression  
        mintypmax_expression            ::= expression | expression : expression : expression  
        module_path_conditional_expression  ::= module_path_expression ? { attribute_instance } module_path_expression : module_path_expression 
        module_path_expression              ::= module_path_primary | unary_module_path_operator { attribute_instance } module_path_primary  | module_path_expression binary_module_path_operator { attribute_instance }                  module_path_expression          | module_path_conditional_expression 
        module_path_mintypmax_expression    ::= module_path_expression | module_path_expression : module_path_expression : module_path_expression
        msb_constant_expression         ::= constant_expression  
        range_expression                ::= expression  msb_constant_expression : lsb_constant_expression | base_expression +: width_constant_expression | base_expression -: width_constant_expression
        width_constant_expression ::= constant_expression
        */
        public static Expression ParseCreate(WordScanner word,NameSpace nameSpace)
        {
            Expression expression = new Expression();
            List<Operator> operatorsStock = new List<Operator>();

            parseExpression(word, nameSpace, expression.RpnExpressionItems, operatorsStock);
            while(operatorsStock.Count != 0)
            {
                expression.RpnExpressionItems.Add(operatorsStock.Last());
                operatorsStock.RemoveAt(operatorsStock.Count - 1);
            }
            if(expression.RpnExpressionItems.Count == 0)
            {
                return null;
            }
            return expression;
        }

        public static Expression ParseCreateVariableLValue(WordScanner word, NameSpace nameSpace)
        {
            Expression expression = new Expression();
            List<Operator> operatorsStock = new List<Operator>();

            parseVariableLValue(word, nameSpace, expression.RpnExpressionItems, operatorsStock);
            if (expression.RpnExpressionItems.Count == 0)
            {
                return null;
            }
            return expression;
        }

        private static bool parseExpression(WordScanner word,NameSpace nameSpace,List<ExpressionItem> expressioItems,List<Operator> operatorStock)
        {
            Primary primary = Primary.ParseCreate(word, nameSpace);
            if (primary != null)
            {
                expressioItems.Add(primary);
            }
            else
            {
                UnaryOperator unaryOperator = UnaryOperator.ParseCreate(word);
                if (unaryOperator != null)
                {
                    AddOperator(unaryOperator, expressioItems, operatorStock);
                    if (word.Eof)
                    {
                        word.AddError("illegal unary Operator");
                        return false;
                    }
                    primary = Primary.ParseCreate(word, nameSpace);
                    if (primary == null)
                    {
                        word.AddError("illegal unary Operator");
                        return false;
                    }
                    expressioItems.Add(primary);
                }
                else
                {
                    return false;
                }
            }
            if (word.Eof) return true;


            if (word.GetCharAt(0) == '?')
            {
                word.MoveNext();
                do
                {
                    if (!parseExpression(word, nameSpace, expressioItems, operatorStock))
                    {
                        word.AddError("illegal binary Operator");
                        break;
                    }
                    if (word.GetCharAt(0) == ':')
                    {
                        word.MoveNext();
                    }
                    else
                    {
                        word.AddError(": expected");
                        break;
                    }
                    if (!parseExpression(word, nameSpace, expressioItems, operatorStock))
                    {
                        word.AddError("illegal binary Operator");
                        break;
                    }
                } while (false);
            }


            BinaryOperator binaryOperator = BinaryOperator.ParseCreate(word);
            if (binaryOperator == null) return true;

            AddOperator(binaryOperator, expressioItems, operatorStock);

            if (!parseExpression(word, nameSpace, expressioItems, operatorStock))
            {
                word.AddError("illegal binary Operator");
            }

            // conditional_expression::= expression1 ? { attribute_instance } expression2: expression3
            //if(word.GetCharAt(0) == '?')
            //{
            //    ConditionalExpression 
            //}



            return true;
        }

        private static bool parseVariableLValue(WordScanner word, NameSpace nameSpace, List<ExpressionItem> expressioItems, List<Operator> operatorStock)
        {
            Primary primary = Primary.ParseCreate(word, nameSpace);
            if (primary != null)
            {
                expressioItems.Add(primary);
            }
            return true;
        }

        private static void AddOperator(Operator newOperator,List<ExpressionItem> expressioItems, List<Operator> operatorStock)
        {
            while (true)
            {
                if (operatorStock.Count == 0 || operatorStock.Last().Precedence >= newOperator.Precedence)
                {
                    operatorStock.Add(newOperator);
                    return;
                }
                Operator popOperator = operatorStock.Last();
                expressioItems.Add(popOperator);
                operatorStock.RemoveAt(operatorStock.Count-1);
            }
        }

    }

    public interface ExpressionItem
    {

    }


}
