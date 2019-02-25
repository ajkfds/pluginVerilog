using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class Expression
    {
        protected Expression()
        {
            Constant = false;
        }

        public List<ExpressionItem> RpnExpressionItems = new List<ExpressionItem>();
        public bool Constant { get; protected set; }
        public double? Value { get; protected set; }
        public int? BitWidth { get; protected set; }
        public WordReference Reference { get; protected set; }

        public override string ToString()
        {
            if (Constant)
            {
                if (Value == null)
                {
                    return "?";
                }
                else
                {
                    return ((double)Value).ToString();
                }
            }
            else
            {
                return "x";
            }
        }

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
            WordReference reference = word.GetReference();

            parseExpression(word, nameSpace, expression.RpnExpressionItems, operatorsStock,ref reference);
            expression.Reference = reference;
            while(operatorsStock.Count != 0)
            {
                expression.RpnExpressionItems.Add(operatorsStock.Last());
                operatorsStock.RemoveAt(operatorsStock.Count - 1);
            }
            if(expression.RpnExpressionItems.Count == 0)
            {
                return null;
            }

            if (expression.RpnExpressionItems.Count == 1 && expression.RpnExpressionItems[0] is Primary)
            {
                Primary primary = expression.RpnExpressionItems[0] as Primary;
                expression.Constant = primary.Constant;
                expression.Value = primary.Value;
                expression.BitWidth = primary.BitWidth;
                return expression;
            }

            if (!word.Active) return expression;
            // parse rpn
            List<Primary> primaryStock = new List<Primary>();
            while(expression.RpnExpressionItems.Count > 0)
            {
                ExpressionItem item = expression.RpnExpressionItems[0];
                if (item is Primary)
                {
                    primaryStock.Add(item as Primary);
                }
                else if (item is BinaryOperator)
                {
                    if (primaryStock.Count < 2) return null;
                    BinaryOperator op = item as BinaryOperator;
                    Primary primary = op.Operate(primaryStock[0], primaryStock[1]);
                    primaryStock.RemoveAt(0);
                    primaryStock.RemoveAt(0);
                    primaryStock.Add(primary);
                }
                else if (item is UnaryOperator)
                {
                    if (primaryStock.Count < 1) return null;
                    UnaryOperator op = item as UnaryOperator;
                    Primary primary = op.Operate(primaryStock[0]);
                    primaryStock.RemoveAt(0);
                    primaryStock.Add(primary);
                }
                else if (item is TenaryOperator)
                {
                    if (primaryStock.Count < 3) return null;
                    TenaryOperator op = item as TenaryOperator;
                    Primary primary = op.Operate(primaryStock[0], primaryStock[1], primaryStock[2]);
                    primaryStock.RemoveAt(0);
                    primaryStock.RemoveAt(0);
                    primaryStock.RemoveAt(0);
                    primaryStock.Add(primary);
                }
                else
                {
                    return null;
                }
                expression.RpnExpressionItems.RemoveAt(0);
            }
            if(primaryStock.Count == 1)
            {
                expression.Constant = primaryStock[0].Constant;
                expression.BitWidth = primaryStock[0].BitWidth;
                expression.Value = primaryStock[0].Value;
            }
            else
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
            while (operatorsStock.Count != 0)
            {
                expression.RpnExpressionItems.Add(operatorsStock.Last());
                operatorsStock.RemoveAt(operatorsStock.Count - 1);
            }
            if (expression.RpnExpressionItems.Count == 0)
            {
                return null;
            }

            if (expression.RpnExpressionItems.Count == 1 && expression.RpnExpressionItems[0] is Primary)
            {
                Primary primary = expression.RpnExpressionItems[0] as Primary;
                expression.Constant = primary.Constant;
                expression.Value = primary.Value;
                expression.BitWidth = primary.BitWidth;
                return expression;
            }
            // parse rpn
            List<Primary> primaryStock = new List<Primary>();
            while (expression.RpnExpressionItems.Count > 0)
            {
                ExpressionItem item = expression.RpnExpressionItems[0];
                if (item is Primary)
                {
                    primaryStock.Add(item as Primary);
                }
                else if (item is BinaryOperator)
                {
                    if (primaryStock.Count < 2) return null;
                    BinaryOperator op = item as BinaryOperator;
                    Primary primary = op.Operate(primaryStock[0], primaryStock[1]);
                    primaryStock.RemoveAt(0);
                    primaryStock.RemoveAt(0);
                    primaryStock.Add(primary);
                }
                else if (item is UnaryOperator)
                {
                    if (primaryStock.Count < 1) return null;
                    UnaryOperator op = item as UnaryOperator;
                    Primary primary = op.Operate(primaryStock[0]);
                    primaryStock.RemoveAt(0);
                    primaryStock.Add(primary);
                }else if(item is TenaryOperator)
                {
                    if (primaryStock.Count < 3) return null;
                    TenaryOperator op = item as TenaryOperator;
                    Primary primary = op.Operate(primaryStock[0], primaryStock[1], primaryStock[2]);
                    primaryStock.RemoveAt(0);
                    primaryStock.RemoveAt(0);
                    primaryStock.RemoveAt(0);
                    primaryStock.Add(primary);
                }
                else
                {
                    return null;
                }
                expression.RpnExpressionItems.RemoveAt(0);
            }
            if (primaryStock.Count == 1)
            {
                expression.Constant = primaryStock[0].Constant;
                expression.BitWidth = primaryStock[0].BitWidth;
                expression.Value = primaryStock[0].Value;
            }
            else
            {
                return null;
            }

            return expression;
        }

        private static bool parseExpression(WordScanner word,NameSpace nameSpace,List<ExpressionItem> expressionItems,List<Operator> operatorStock,ref WordReference reference)
        {
            reference = word.GetReference(reference);
            Primary primary = Primary.ParseCreate(word, nameSpace);
            if (primary != null)
            {
                expressionItems.Add(primary);
            }
            else
            {
                UnaryOperator unaryOperator = UnaryOperator.ParseCreate(word);
                if (unaryOperator != null)
                {
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
                    expressionItems.Add(primary);
                    AddOperator(unaryOperator, expressionItems, operatorStock);
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
                    if (!parseExpression(word, nameSpace, expressionItems, operatorStock,ref reference))
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
                    if (!parseExpression(word, nameSpace, expressionItems, operatorStock, ref reference))
                    {
                        word.AddError("illegal binary Operator");
                        break;
                    }
                    expressionItems.Add(TenaryOperator.Create());
                } while (false);
            }

            BinaryOperator binaryOperator = BinaryOperator.ParseCreate(word);
            if (binaryOperator == null) return true;

            AddOperator(binaryOperator, expressionItems, operatorStock);

            if (!parseExpression(word, nameSpace, expressionItems, operatorStock, ref reference))
            {
                word.AddError("illegal binary Operator");
            }

            return true;
        }

        private static bool parseVariableLValue(WordScanner word, NameSpace nameSpace, List<ExpressionItem> expressionItems, List<Operator> operatorStock)
        {
            Primary primary = Primary.ParseCreate(word, nameSpace);
            if (primary != null)
            {
                expressionItems.Add(primary);
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

}
