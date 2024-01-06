using pluginVerilog.Verilog.BuildingBlocks;
using pluginVerilog.Verilog.DataObjects;
using pluginVerilog.Verilog.DataObjects.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.DataObjects.Constants
{

    // parameter, localparam, and specparam.
    public class Constants : DataObject
    {
        public Expressions.Expression Expression;
        public WordReference DefinitionRefrecnce { get; set; }

        public enum ConstantTypeEnum
        {
            parameter,
            localparam,
            specparam
        }
        public ConstantTypeEnum ConstantType = ConstantTypeEnum.parameter;

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

            /* ## SystemVerilog
             * 
            local_parameter_declaration ::=
                  "localparam" data_type_or_implicit list_of_param_assignments 
                | "localparam" type list_of_type_assignments 
            parameter_declaration ::= 
                  "parameter" data_type_or_implicit list_of_param_assignments 
                | "parameter" type list_of_type_assignments 
            specparam_declaration ::= 
                  "specparam" [ packed_dimension ] list_of_specparam_assignments ;
             */
            ConstantTypeEnum constantType = ConstantTypeEnum.parameter;
            if (word.Text == "parameter")
            {
                constantType = ConstantTypeEnum.parameter;
            }
            else if (word.Text == "localparam")
            {
                constantType = ConstantTypeEnum.localparam;
            }
            else if(word.Text == "specparam")
            {
                constantType = ConstantTypeEnum.specparam;
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            DataObjects.DataTypes.DataType dataType = DataObjects.DataTypes.DataType.ParseCreate(word, module, null);

            switch (word.Text)
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

            WordReference nameReference;
            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) break;
                string identifier = word.Text;
                nameReference = word.GetReference();
                word.Color(CodeDrawStyle.ColorType.Paramater);
                word.MoveNext();

                if (word.Text != "=") break;
                word.MoveNext();
                Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module);
                if (expression == null) break;
                if (word.Active)
                {
                    //if (local)
                    //{
                    //    if (!word.Active)
                    //    {

                    //    }
                    //    else if (word.Prototype)
                    //    {
                    //        if (module.LocalParameters.ContainsKey(identifier))
                    //        {
                    //            //                                nameReference.AddError("local parameter name duplicated");
                    //        }
                    //        else
                    //        {
                    //            Parameter param = new Parameter();
                    //            param.Name = identifier;
                    //            param.Expression = expression;
                    //            param.DefinitionRefrecnce = nameReference;
                    //            module.LocalParameters.Add(param.Name, param);
                    //        }
                    //    }
                    //    else
                    //    {

                    //    }
                    //}
                    //else
                    {
                        if (!word.Active)
                        {
                            // skip
                        }
                        else if (word.Prototype)
                        {
                            if (module.Parameters.ContainsKey(identifier))
                            {
                                //                                nameReference.AddError("parameter name duplicated");
                            }
                            else
                            {
                                Parameter param = new Parameter();
                                param.Name = identifier;
                                param.Expression = expression;
                                param.DefinitionRefrecnce = nameReference;
                                param.ConstantType = constantType;
                                module.Parameters.Add(param.Name, param);

                                module.PortParameterNameList.Add(identifier);
                            }
                        }
                        else
                        {

                        }
                    }
                }
                if (word.Text != ",") break;
                if (word.NextText == "parameter") break;
                word.MoveNext();
            }
        }
        public static void ParseCreateDeclaration(WordScanner word, NameSpace nameSpace, Attribute attribute)
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

            /* ## SystemVerilog
            local_parameter_declaration     ::=   "localparam" data_type_or_implicit list_of_param_assignments 
                                                | "localparam" type list_of_type_assignments 
            parameter_declaration           ::=   "parameter" data_type_or_implicit list_of_param_assignments 
                                                | "parameter" type list_of_type_assignments 

            specparam_declaration           ::=   "specparam" [ packed_dimension ] list_of_specparam_assignments ";"

            data_type_or_implicit           ::=   data_type
                                                | implicit_data_type
            implicit_data_type              ::=   [ signing ] { packed_dimension } 

            list_of_param_assignments       ::= param_assignment { , param_assignment }
            list_of_specparam_assignments   ::= specparam_assignment { , specparam_assignment }
            list_of_type_assignments        ::= type_assignment { , type_assignment } 

            param_assignment                ::= parameter_identifier { unpacked_dimension } [ = constant_param_expression ]
            
            specparam_assignment            ::= specparam_identifier = constant_mintypmax_expression 
                                                | pulse_control_specparam 

            type_assignment                 ::= type_identifier [ = data_type ]
             */

            ConstantTypeEnum constantType = ConstantTypeEnum.parameter;
            if (word.Text == "parameter")
            {
                constantType = ConstantTypeEnum.parameter;
            }
            else if (word.Text == "localparam")
            {
                constantType = ConstantTypeEnum.localparam;
            }
            else if (word.Text == "specparam")
            {
                constantType = ConstantTypeEnum.specparam;
            }
            else
            {
                System.Diagnostics.Debugger.Break();
            }

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            DataType dataType = DataObjects.DataTypes.DataType.ParseCreate(word, nameSpace, null);
            Range range = null;
            bool signed = false;

            if (word.Text == "signed")
            {
                signed = true;
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
            }

            if (word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, nameSpace);
            }

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text)) break;
                string identifier = word.Text;
                word.Color(CodeDrawStyle.ColorType.Paramater);
                Parameter param = new Parameter();

                {
                    if (!word.Active)
                    {
                        // skip
                    }
                    else if (word.Prototype)
                    {
                        if (nameSpace.Parameters.ContainsKey(identifier))
                        {
                            word.AddError("name duplicated");
                        }
                        else
                        {
                            param.Name = identifier;
                            nameSpace.Parameters.Add(param.Name, param);
                        }
                    }
                    else
                    {
                        if (nameSpace.Parameters.ContainsKey(identifier))
                        { // re-parse after prototype parse 
                            param = nameSpace.Parameters[identifier];
                        }
                        else
                        { // for root nameSpace parameter
                            param.Name = identifier;
                            nameSpace.Parameters.Add(param.Name, param);
                        }
                    }
                }
                word.MoveNext();

                if (word.Text != "=") break;
                word.MoveNext();

                param.Expression = Expressions.Expression.ParseCreate(word, nameSpace);
                if (param.Expression == null) break;

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
                word.SkipToKeyword(";");
                if (word.Text == ";") word.MoveNext();
            }
        }
    }
}
