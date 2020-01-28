using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Expressions
{
    public class Operator : ExpressionItem
    {
        protected Operator(string text,byte precedence)
        {
            this.Text = text;
            this.Precedence = precedence;
        }

        public readonly string Text = "";
        public readonly byte Precedence;

        public ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText(Text);
            return label;
        }

        public override string ToString()
        {
            return Text;
        }
    }
    /*
    Table 12—Precedence rules for operators
    1   + - ! ~ (unary)             Highest precedence
    2   **
    3   *  /  %
    4   +  - (binary)
    5   <<  >>  <<<  >>>
    6   <  <=  >  >=
    7   ==  !=  ===  !==
    8   &  ~&
    9   ^  ^~  ~^
    10  |  ~|
    11  &&
    12  ||
    13  ?: (conditional operator)   Lowest precedence
    */

    public class TenaryOperator : Operator
    {
        protected TenaryOperator(string text, byte precedence) : base(text, precedence) { }

        public static TenaryOperator Create()
        {
            return new TenaryOperator("?",0);
        }

        public delegate void OperatedAction(Primary result, Primary condition, Primary primary1, Primary primary2);
        public static OperatedAction Operated;

        public Primary Operate(Primary condition, Primary primary1, Primary primary2)
        {
            bool constant = false;
            double? value = null;
            int? bitWidth = null;

            if (primary1.Constant && primary2.Constant & condition.Constant ) constant = true;
            if (condition.Value != null && primary1.Value != null && primary2.Value != null) value = getValue((double)condition.Value, (double)primary1.Value, (double)primary2.Value);
            if (primary1.BitWidth != null && primary2.BitWidth != null) bitWidth = getBitWidth(Text, (int)primary1.BitWidth, (int)primary2.BitWidth);

            Primary ret = Primary.Create(constant, value, bitWidth);
            if(Operated!=null) Operated(ret, condition, primary1, primary2);
            return ret;
        }

        private int? getBitWidth(string operatorText, int bitWidth1, int bitWidth2)
        {
            int maxWidth = bitWidth1;
            if (bitWidth2 > bitWidth1) maxWidth = bitWidth2;


            return bitWidth1;
        }

        private double? getValue(double condition, double value1, double value2)
        {
            if(condition != 0)
            {
                return value1;
            }
            else
            {
                return value2;
            }
        }
    }

    public class UnaryOperator : Operator
    {
        protected UnaryOperator(string text,byte precedence) : base(text,precedence) { }
        /*
        unary_operator  ::=   + 
                            | - 
                            | ! 
                            | ~ 
                            | & 
                            | | 
                            | ^ 
                            | ~^ 
                            | ~& 
                            | ~| 
                            | ^~
        */
        public static UnaryOperator ParseCreate(WordScanner word)
        {
            switch (word.Length)
            {
                case 1:
                    if (word.GetCharAt(0) == '+') { word.MoveNext(); return new UnaryOperator("+",1); }
                    if (word.GetCharAt(0) == '-') { word.MoveNext(); return new UnaryOperator("-",1); }
                    if (word.GetCharAt(0) == '!') { word.MoveNext(); return new UnaryOperator("!",1); }
                    if (word.GetCharAt(0) == '~') { word.MoveNext(); return new UnaryOperator("~",1); }
                    if (word.GetCharAt(0) == '&') { word.MoveNext(); return new UnaryOperator("&",8); }
                    if (word.GetCharAt(0) == '|') { word.MoveNext(); return new UnaryOperator("|",10); }
                    if (word.GetCharAt(0) == '^') { word.MoveNext(); return new UnaryOperator("^",9); }
                    return null;
                case 2:
                    if (word.GetCharAt(0) == '~')
                    {
                        if (word.GetCharAt(1) == '^') { word.MoveNext(); return new UnaryOperator("~^",9); }
                        if (word.GetCharAt(1) == '&') { word.MoveNext(); return new UnaryOperator("~&",8); }
                        if (word.GetCharAt(1) == '|') { word.MoveNext(); return new UnaryOperator("~|",10); }
                        return null;
                    }
                    else if (word.GetCharAt(0) == '^')
                    {
                        if (word.GetCharAt(1) == '~') { word.MoveNext(); return new UnaryOperator("^~",9); }
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        public delegate void OperatedAction(Primary result, Primary primary);
        public static OperatedAction Operated;

        public Primary Operate(Primary primary)
        {
            bool constant = false;
            double? value = null;
            int? bitWidth = null;

            if (primary.Constant) constant = true;
            if (primary.Value != null) value = getValue(Text, (double)primary.Value);
            if (primary.BitWidth != null) bitWidth = getBitWidth(Text, (int)primary.BitWidth);

            Primary ret = Primary.Create(constant, value, bitWidth);
            if (Operated != null) Operated(ret, primary);
            return ret;
        }

        private double? getValue(string text,double value)
        {
            switch (text)
            {
                // alithmetic operators
                case "+":
                    return value;
                case "-":
                    return -value;

                // logical negation
                case "!":
                    return null;

                // reduction operators
                case "&":
                case "|":
                case "^":
                case "~^":
                case "~&":
                case "~|":
                case "^~":
                    return null;

                default:
                    return null;
            }
        }

        private int? getBitWidth(string text,int bitWidth)
        {
            switch (text)
            {
                // alithmetic operators
                case "+":
                case "-":
                    return bitWidth+1;

                // logical negation
                case "!":
                    return bitWidth;

                // reduction operators
                case "&":
                case "|":
                case "^":
                case "~^":
                case "~&":
                case "~|":
                case "^~":
                    return 1;
                default:
                    return null;
            }
        }

    }
    public class BinaryOperator : Operator
    {
        protected BinaryOperator(string text, byte precedence) : base(text, precedence) { }
        /*
        binary_operator ::=   + 
                            | - 
                            | * 
                            | / 
                            | %
                            | < 
                            | > 
                            | & 
                            | | 
                            | ^ 

                            | == 
                            | != 
                            | <= 
                            | >= 

                            | && 
                            | || 
                            | **
                            | >> 
                            | << 
                            | ^~ 
                            | ~^ 
                            | === 
                            | !== 
                            | >>> 
                            | <<< 
        */
        public static BinaryOperator ParseCreate(WordScanner word)
        {
            switch (word.Length)
            {
                case 1:
                    if (word.GetCharAt(0) == '+') { word.MoveNext(); return new BinaryOperator("+",4); }
                    if (word.GetCharAt(0) == '-') { word.MoveNext(); return new BinaryOperator("-",4); }
                    if (word.GetCharAt(0) == '*') { word.MoveNext(); return new BinaryOperator("*",3); }
                    if (word.GetCharAt(0) == '/') { word.MoveNext(); return new BinaryOperator("/",3); }
                    if (word.GetCharAt(0) == '%') { word.MoveNext(); return new BinaryOperator("%",3); }
                    if (word.GetCharAt(0) == '<') { word.MoveNext(); return new BinaryOperator("<",6); }
                    if (word.GetCharAt(0) == '>') { word.MoveNext(); return new BinaryOperator(">",6); }
                    if (word.GetCharAt(0) == '&') { word.MoveNext(); return new BinaryOperator("&",8); }
                    if (word.GetCharAt(0) == '|') { word.MoveNext(); return new BinaryOperator("|",10); }
                    if (word.GetCharAt(0) == '^') { word.MoveNext(); return new BinaryOperator("^",9); }
                    return null;
                case 2:
                    if (word.GetCharAt(1) == '=')
                    {
                        if (word.GetCharAt(0) == '=') { word.MoveNext(); return new BinaryOperator("==",7); }
                        if (word.GetCharAt(0) == '!') { word.MoveNext(); return new BinaryOperator("!=",7); }
                        if (word.GetCharAt(0) == '<') { word.MoveNext(); return new BinaryOperator("<=",6); }
                        if (word.GetCharAt(0) == '>') { word.MoveNext(); return new BinaryOperator(">=",6); }
                        return null;
                    }
                    else if (word.GetCharAt(0) == word.GetCharAt(1))
                    {
                        if (word.GetCharAt(0) == '&') { word.MoveNext(); return new BinaryOperator("&&",11); }
                        if (word.GetCharAt(0) == '|') { word.MoveNext(); return new BinaryOperator("||",12); }
                        if (word.GetCharAt(0) == '*') { word.MoveNext(); return new BinaryOperator("**",2); }
                        if (word.GetCharAt(0) == '>') { word.MoveNext(); return new BinaryOperator(">>",5); }
                        if (word.GetCharAt(0) == '<') { word.MoveNext(); return new BinaryOperator("<<",5); }
                        if (word.GetCharAt(0) == '^' && word.GetCharAt(1) == '~') { word.MoveNext(); return new BinaryOperator("^~",9); }
                        if (word.GetCharAt(0) == '~' && word.GetCharAt(1) == '^') { word.MoveNext(); return new BinaryOperator("~^",9); }
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                case 3:
                    if (word.GetCharAt(1) != word.GetCharAt(2)) return null;
                    if(word.GetCharAt(1) == '=')
                    {
                        if (word.GetCharAt(0) == '=') { word.MoveNext(); return new BinaryOperator("===",7); }
                        if (word.GetCharAt(0) == '!') { word.MoveNext(); return new BinaryOperator("!==",7); }
                        return null;
                    }
                    else if (word.GetCharAt(0) == word.GetCharAt(1))
                    {
                        if (word.GetCharAt(0) == '>') { word.MoveNext(); return new BinaryOperator(">>>",5); }
                        if (word.GetCharAt(0) == '<') { word.MoveNext(); return new BinaryOperator("<<<",5); }
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                default:
                    return null;
            }
        }

        public Primary Operate(Primary primary1, Primary primary2)
        {
            bool constant = false;
            double? value = null;
            int? bitWidth = null;

            if (primary1.Constant && primary2.Constant) constant = true;
            if (primary1.Value != null && primary2.Value != null) value = getValue(Text, (double)primary1.Value, (double)primary2.Value);
            if (primary1.BitWidth != null && primary2.BitWidth != null) bitWidth = getBitWidth(Text, (int)primary1.BitWidth, (int)primary2.BitWidth);

            Primary ret = Primary.Create(constant, value, bitWidth);
            if (Operated != null) Operated(ret, primary1, primary2);
            return ret;
        }

        public delegate void OperatedAction(Primary result, Primary primary1, Primary primary2);
        public static OperatedAction Operated;

        private int? getBitWidth(string operatorText,int bitWidth1,int bitWidth2)
        {
            int maxWidth = bitWidth1;
            if (bitWidth2 > bitWidth1) maxWidth = bitWidth2;

            switch (operatorText)
            {
                // alithmetic operator
                case "+":
                    return maxWidth + 1;
                case "-":
                    return maxWidth;
                case "*":
                    return bitWidth1 + bitWidth2;
                case "/":
                    return maxWidth;
                case "**":
                    return bitWidth1 * bitWidth2;

                // modulus operator
                case "%":
                    return bitWidth2;

                // relational operators
                case "<":
                case ">":
                case "<=":
                case ">=":
                    return 1;
                // equality operators
                case "==":
                case "!=":
                case "===":
                case "!==":
                    return 1;
                // logical operators
                case "&&":
                case "||":
                    return 1;

                // bit-wise binary operators
                case "&":
                case "|":
                case "^":
                case "^~":
                case "~^":
                    return maxWidth;

                // logical shoft
                case ">>":
                case "<<":
                    return bitWidth1;
                // alithmetic shift
                case ">>>":
                case "<<<":
                    return bitWidth1;

                default:
                    return null;
            }

        }

        private double? getValue(string operatorText, double value1, double value2)
        {
            switch (Text)
            {
                // alithmetic operator
                case "+":
                    return value1 + value2;
                case "-":
                    return value1 - value2;
                case "*":
                    return value1 * value2;
                case "/":
                    return value1 / value2;
                case "**":
                    return Math.Pow(value1, value2);

                // modulus operator
                case "%":
                    return value1 % value2;

                // relational operators
                case "<":
                    return (value1 < value2) ? 1:0;
                case ">":
                    return (value1 > value2) ? 1 : 0;
                case "<=":
                    return (value1 <= value2) ? 1 : 0;
                case ">=":
                    return (value1 >= value2) ? 1 : 0;

                // equality operators
                case "==":
                    return (value1 == value2) ? 1 : 0;
                case "!=":
                    return (value1 != value2) ? 1 : 0;
                case "===":
                    return (value1 == value2) ? 1 : 0;
                case "!==":
                    return (value1 != value2) ? 1 : 0;

                // logical operators
                case "&&":
                    return (value1 != 0) & (value2 != 0) ? 1 : 0;
                case "||":
                    return (value1 != 0) | (value2 != 0) ? 1 : 0;

                // bit-wise binary operators
                case "&":
                case "|":
                case "^":
                case "^~":
                case "~^":
                
                // logical shoft
                case ">>":
                case "<<":
                // alithmetic shift
                case ">>>":
                case "<<<":
                default:
                    return null;
            }
        }


    }

    }
