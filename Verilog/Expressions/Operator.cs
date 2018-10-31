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
    }

}
