using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Parameter : Item
    {
        public Expressions.Expression Expression;

        public static void ParseCreateDeclarationForPort(WordScanner word, Module module, Attribute attribute)
        {
            /*
            local_parameter_declaration ::=  (From Annex A - A.2.1.1)  
                                            localparam [ signed ] [ range ] list_of_param_assignments ; 
                                            | localparam integer list_of_param_assignments ; 
                                            | localparam real list_of_param_assignments ; 
                                            | localparam realtime list_of_param_assignments ; 
                                            | localparam time list_of_param_assignments ; 
            parameter_declaration       ::=  parameter [ signed ] [ range ] list_of_param_assignments ;
                                            | parameter integer list_of_param_assignments ; 
                                            | parameter real list_of_param_assignments ; 
                                            | parameter realtime list_of_param_assignments ; 
                                            | parameter time list_of_param_assignments ;
            list_of_param_assignments ::= (From Annex A - A.2.3) param_assignment { , param_assignment }  
            param_assignment ::= (From Annex A - A.2.4) parameter_identifier = constant_expression  
            range ::=  (From Annex A - A.2.5) [ msb_constant_expression : lsb_constant_expression ]              
            */
            bool local = false;
            if (word.Text == "parameter")
            {
                local = false;
            }
            else if (word.Text == "localparam")
            {
                local = true;
            }
            else
            {

                System.Diagnostics.Debugger.Break();
            }
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            switch(word.Text)
            {
                case "integer":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                case "real":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                case "realtime":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                case "time":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                default:
                    if(word.Text == "signed")
                    {
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
                    }
                    if(word.GetCharAt(0) == '[')
                    {
                        Range range = Range.ParseCreate(word, module);
                    }
                    break;
            }

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) break;
                string identifier = word.Text;
                word.Color(CodeDrawStyle.ColorType.Paramater);
                word.MoveNext();

                if (word.Text != "=") break;
                word.MoveNext();
                Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module);
                if (expression == null) break;

                if (local)
                {
                    if (module.LocalParameters.ContainsKey(identifier))
                    {
                        word.AddError("local parameter name duplicated");
                    }
                    else
                    {
                        Parameter param = new Parameter();
                        param.Name = identifier;
                        param.Expression = expression;
                        module.LocalParameters.Add(param.Name, param);
                    }
                }
                else
                {
                    if (module.Parameters.ContainsKey(identifier))
                    {
                        word.AddError("parameter name duplicated");
                    }
                    else
                    {
                        Parameter param = new Parameter();
                        param.Name = identifier;
                        param.Expression = expression;
                        module.Parameters.Add(param.Name, param);
                    }
                }
                if (word.Text != ",") break;
                if (word.NextText == "parameter") break;
                word.MoveNext();
            }
        }
        public static void ParseCreateDeclaration(WordScanner word, Module module, Attribute attribute)
        {
            /*
            local_parameter_declaration ::=  (From Annex A - A.2.1.1)  
                                            localparam [ signed ] [ range ] list_of_param_assignments ; 
                                            | localparam integer list_of_param_assignments ; 
                                            | localparam real list_of_param_assignments ; 
                                            | localparam realtime list_of_param_assignments ; 
                                            | localparam time list_of_param_assignments ; 
            parameter_declaration       ::=  parameter [ signed ] [ range ] list_of_param_assignments ;
                                            | parameter integer list_of_param_assignments ; 
                                            | parameter real list_of_param_assignments ; 
                                            | parameter realtime list_of_param_assignments ; 
                                            | parameter time list_of_param_assignments ;
            list_of_param_assignments ::= (From Annex A - A.2.3) param_assignment { , param_assignment }  
            param_assignment ::= (From Annex A - A.2.4) parameter_identifier = constant_expression  
            range ::=  (From Annex A - A.2.5) [ msb_constant_expression : lsb_constant_expression ]              
            */
            bool local = false;
            if (word.Text == "parameter")
            {
                local = false;
            }
            else if (word.Text == "localparam")
            {
                local = true;
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            switch (word.Text)
            {
                case "integer":
                    word.Color( CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                case "real":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                case "realtime":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                case "time":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    break;
                default:
                    if (word.Text == "signed")
                    {
                        word.Color(CodeDrawStyle.ColorType.Keyword);
                        word.MoveNext();
                    }
                    if (word.GetCharAt(0) == '[')
                    {
                        Range range = Range.ParseCreate(word, module);
                    }
                    break;
            }

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) break;
                string identifier = word.Text;
                word.Color(CodeDrawStyle.ColorType.Paramater);
                word.MoveNext();

                if (word.Text != "=") break;
                word.MoveNext();
                Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module);
                if (expression == null) break;

                if (local)
                {
                    if (module.LocalParameters.ContainsKey(identifier))
                    {
                        word.AddError("local parameter name duplicated");
                    }
                    else
                    {
                        Parameter param = new Parameter();
                        param.Name = identifier;
                        param.Expression = expression;
                        module.LocalParameters.Add(param.Name, param);
                    }
                }
                else
                {
                    if (module.Parameters.ContainsKey(identifier))
                    {
                        word.AddError("parameter name duplicated");
                    }
                    else
                    {
                        Parameter param = new Parameter();
                        param.Name = identifier;
                        param.Expression = expression;
                        module.Parameters.Add(param.Name, param);
                    }
                }
                if (word.Text != ",") break;
                word.MoveNext();
            }

            if (word.GetCharAt(0) == ';')
            {
                word.MoveNext();
            }
            else
            {
                word.AddError("; expected");
            }
        }
    }
}
