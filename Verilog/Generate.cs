using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class Generate
    {
        public static void ParseGenerateLoopStatement(WordScanner word, IModuleOrGeneratedBlock module)
        {
            // generate_loop_statement::=  for (genvar_assignment; constant_expression; genvar_assignment) begin : generate_block_identifier { generate_item } end
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            if (word.Text != "(")
            {
                word.AddError("( expected");
                return;
            }
            word.MoveNext();

            if (!parseGenvarAssignment(word, module as NameSpace))
            {
                return;
            }

            if (word.Text != ";")
            {
                word.AddError("; expected");
                return;
            }
            word.MoveNext();

            Expressions.Expression constant = Expressions.Expression.ParseCreate(word, module as NameSpace);
            if (constant == null) return;
            if (!constant.Constant)
            {
                constant.Reference.AddError("should be constant");
            }

            if (word.Text != ";")
            {
                word.AddError("; expected");
                return;
            }
            word.MoveNext();

            if (!parseGenvarAssignment(word, module as NameSpace))
            {
                return;
            }

            if (word.Text != ")")
            {
                word.AddError(") expected");
                return;
            }
            word.MoveNext();

            ParseGenerateBlockStatementWithName(word, module);
        }

        private static bool parseGenvarAssignment(WordScanner word, NameSpace nameSpace)
        {
            //    genvar_assignment::= genvar_identifier = constant_expression
            Expressions.VariableReference genvar = Expressions.VariableReference.ParseCreate(word, nameSpace);
            if (genvar == null) return false;
            if (!(genvar.Variable is Variables.Genvar))
            {
                word.AddError("should be genvar");
            }
            if (word.Text != "=")
            {
                word.AddError("( expected");
                return false;
            }
            word.MoveNext();
            Expressions.Expression constant = Expressions.Expression.ParseCreate(word, nameSpace);
            if (constant == null) return false;
            if (!constant.Constant)
            {
                word.AddError("should be constant");
            }
            return true;
        }

        public static void ParseGenerateConditionalStatement(WordScanner word, IModuleOrGeneratedBlock module)
        {
            // generate_conditional_statement::= if (constant_expression ) generate_item_or_null[ else generate_item_or_null]
            // true (that is, has a nonzero known value)
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            if (word.Text != "(")
            {
                word.AddError("( required");
                return;
            }
            word.MoveNext();

            Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module as NameSpace);
            if (expression != null && !expression.Constant)
            {
                word.AddError("should be constant");
            }

            if (word.Text != ")")
            {
                word.AddError(") required");
                return;
            }
            word.MoveNext();

            if(word.Active && expression.Constant && expression.Value == 0)
            {
                // false
                word.StartNonGenenerated();
                Module.ParseGenerateItem(word, module);
                word.EndNonGeneratred();
            }
            else
            {
                Module.ParseGenerateItem(word, module);
            }

            if (word.Text == "else")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();

                if (word.Active && expression.Constant && expression.Value != 0)
                {
                    // false
                    word.StartNonGenenerated();
                    Module.ParseGenerateItem(word, module);
                    word.EndNonGeneratred();
                }
                else
                {
                    Module.ParseGenerateItem(word, module);
                }
            }

        }

        public static void ParseGenerateCaseStatement(WordScanner word, IModuleOrGeneratedBlock module)
        {
            // generate_case_statement::=  case (constant_expression ) genvar_case_item { genvar_case_item } endcase
            // genvar_case_item ::=  constant_expression  { , constant_expression } : generate_item_or_null
            //                      | default [ : ] generate_item_or_null
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            if (word.Text != "(")
            {
                word.AddError("( required");
                return;
            }
            word.MoveNext();

            Expressions.Expression conditionExpression = Expressions.Expression.ParseCreate(word, module as NameSpace);
            if(conditionExpression == null)
            {
                word.AddError("illegal constant_expression");
                return;
            }
            if (!conditionExpression.Constant)
            {
                word.AddError("should be constant");
            }

            if (word.Text != ")")
            {
                word.AddError(") required");
                return;
            }
            word.MoveNext();

            bool caseSelected = false;
            while (!word.Eof)
            {
                Expressions.Expression expression = null;
                if (word.Text == "endcase")
                {
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                }
                else if (word.Text == "default")
                {
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();

                    if (word.Text == ":") word.MoveNext();

                    if(caseSelected && word.Active)
                    {
                        word.StartNonGenenerated();
                        Module.ParseGenerateItem(word, module);
                        word.EndNonGeneratred();
                    }
                    else
                    {
                        Module.ParseGenerateItem(word, module);
                    }
                }
                else
                {
                    while (!word.Eof)
                    {
                        expression = Expressions.Expression.ParseCreate(word, module as NameSpace);
                        if(expression == null)
                        {
                            word.AddError("illegal constant expression");
                            return;
                        }
                        if (!expression.Constant)
                        {
                            word.AddError("should be constant");
                        }
                        if (word.Text != ",") break;
                        word.MoveNext();
                    }
                    if (word.Text != ":")
                    {
                        word.AddError(": required");
                        return;
                    }
                    word.MoveNext();

                    if (word.Active && expression != null && conditionExpression != null && expression.Constant && conditionExpression.Constant && expression.Value != conditionExpression.Value)
                    {
                        // false
                        word.StartNonGenenerated();
                        Module.ParseGenerateItem(word, module);
                        word.EndNonGeneratred();
                    }
                    else
                    {
                        Module.ParseGenerateItem(word, module);
                        caseSelected = true;
                    }
                }
            }

        }

        public static void ParseGenerateBlockStatement(WordScanner word, IModuleOrGeneratedBlock module)
        {
            // generate_block ::= begin[ : generate_block_identifier]  { generate_item } end
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            IModuleOrGeneratedBlock block = module;

            if (word.Text == ":")
            {
                word.MoveNext();

                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("identifier required");
                    return;
                }
                word.Color(CodeDrawStyle.ColorType.Identifier);
                block = NamedGeneratedBlock.Create(module.Module, module as NameSpace, word.Text);
                if (module.NameSpaces.ContainsKey(word.Text))
                {
                    word.AddPrototypeError("duplicated identifier");
                }
                else
                {
                    module.NameSpaces.Add(word.Text, block as NamedGeneratedBlock);
                }

                word.MoveNext();
            }

            if (word.Active)
            {
                Module.ParseGenerateItems(word, module);
            }
            else
            {
                int beginCount = 0;
                word.AddError("illegal sequential block");
                while (!word.Eof && word.Text != "endgenerate")
                {
                    if (word.Text == "begin")
                    {
                        beginCount++;
                    }
                    else if (word.Text == "end")
                    {
                        if (beginCount == 0)
                        {
                            break;
                        }
                        beginCount--;
                    }
                    word.MoveNext();
                }
            }

            if (word.Text == "end")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                return;
            }
            else
            {
                word.AddError("end required");
            }
        }

        public static void ParseGenerateBlockStatementWithName(WordScanner word, IModuleOrGeneratedBlock module)
        {
            // generate_block ::= begin[ : generate_block_identifier]  { generate_item } end
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            if (word.Text != ":")
            {
                word.AddError(": required");
            }
            else
            {
                word.MoveNext();

                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("identifier required");
                    return;
                }
                word.Color(CodeDrawStyle.ColorType.Identifier);
                word.MoveNext();
            }

            if (word.Active)
            {
                Module.ParseGenerateItems(word, module);
            }
            else
            {
                int beginCount = 0;
                word.AddError("illegal sequential block");
                while (!word.Eof && word.Text != "endgenerate")
                {
                    if (word.Text == "begin")
                    {
                        beginCount++;
                    }
                    else if (word.Text == "end")
                    {
                        if (beginCount == 0)
                        {
                            break;
                        }
                        beginCount--;
                    }
                    word.MoveNext();
                }
            }

            if (word.Text == "end")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                return;
            }
            else
            {
                word.AddError("end required");
            }
        }
    }
}
