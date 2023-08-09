using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ajkControls;
using ajkControls.ColorLabel;

namespace pluginVerilog.Verilog.Variables
{
    /*
    type_declaration::= 
        typedef data_type type_identifier { variable_dimension};
        | typedef interface_instance_identifier constant_bit_select.type_identifier type_identifier;
        | typedef[enum | struct | union | class | interface class ] type_identifier ;


        data_type ::=
            ... 
            | "enum" [ enum_base_type ] { enum_name_declaration { , enum_name_declaration } } { packed_dimension } 
            ... 

        enum_base_type ::= 
            integer_atom_type [ signing ] 
            | integer_vector_type [ signing ] [ packed_dimension ] 
            | type_identifier [ packed_dimension ]

        enum_name_declaration ::= 
            enum_identifier [ [ integral_number [ : integral_number ] ] ] [ = constant_expression ]

        integer_type            ::= integer_vector_type | integer_atom_type 
        integer_atom_type       ::= byte | shortint | int | longint | integer | time
        integer_vector_type     ::= bit | logic | reg

        non_integer_type ::= shortreal | real | realtime

    */
    public class Enum : Variable
    {
        protected Enum() { }

        public Range Range { get; protected set; }


        public Enum(string Name, Range range)
        {
            this.Name = Name;
            this.Range = range;
        }

        public override void AppendLabel(ColorLabel label)
        {
            AppendTypeLabel(label);
            label.AppendText(Name, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Register));

            foreach (Dimension dimension in Dimensions)
            {
                label.AppendText(" ");
                label.AppendLabel(dimension.GetLabel());
            }

            if (Comment != "")
            {
                label.AppendText(" ");
                label.AppendText(Comment.Trim(new char[] { '\r', '\n', '\t', ' ' }), Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Comment));
            }

            label.AppendText("\r\n");
        }

        public override void AppendTypeLabel(ColorLabel label)
        {
        }


        public static Variable ParseCrateBaseDataType(WordScanner word, NameSpace nameSpace)
        {
            /*
            type_declaration::= 
                typedef data_type type_identifier { variable_dimension};
                | typedef interface_instance_identifier constant_bit_select.type_identifier type_identifier;
                | typedef[enum | struct | union | class | interface class ] type_identifier ;


                data_type ::=
                    ... 
                    | enum [ enum_base_type ] { enum_name_declaration { , enum_name_declaration } } { packed_dimension } 
                    ... 

                enum_base_type ::= 
                    integer_atom_type [ signing ] 
                    | integer_vector_type [ signing ] [ packed_dimension ] 
                    | type_identifier [ packed_dimension ]

                enum_name_declaration ::= 
                    enum_identifier [ [ integral_number [ : integral_number ] ] ] [ = constant_expression ]
                integer_type            ::= integer_vector_type | integer_atom_type 
                integer_atom_type       ::= byte | shortint | int | longint | integer | time
                integer_vector_type     ::= bit | logic | reg

                non_integer_type ::= shortreal | real | realtime

            */
            Variable val;

            switch (word.Text)
            {
                // integer_atom_type[signing]
                //case "byte":
                //case "shortint":
                //case "int":
                //case "longint":
                case "integer":
                    val = Integer.ParseCreateType(word, nameSpace);
                    break;
                //case "time":
                //    break;
                // integer_vector_type[signing][packed_dimension]
                case "bit":
                    val = Bit.ParseCreateType(word, nameSpace);
                    break;
                //case "logic":
                case "reg":
                    val = Reg.ParseCreateType(word, nameSpace);
                    break;
                //  type_identifier[packed_dimension]

                default:
                    return null;
            }
            return val;

        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            /*
                data_declaration ::= 
                        [ const ] [ var ] [ lifetime ] data_type_or_implicit list_of_variable_decl_assignments ;
                        | ...

                data_type_or_implicit ::= 
                        data_type 
                        |...

                data_type ::=
                    ... 
                    | enum [ enum_base_type ] { enum_name_declaration { , enum_name_declaration } } { packed_dimension } 
                    ... 
             */

            if (word.Text != "enum") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // enum
            if (!word.SystemVerilog) word.AddError("systemverilog description");

            Variable type = ParseCrateBaseDataType(word, nameSpace);

            if (word.Eof || word.Text != "{")
            {
                word.AddError("( expected");
                word.SkipToKeyword(":");
                return;
            }
            word.MoveNext();

            // { enum_name_declaration { , enum_name_declaration } }
            while ( !word.Eof)
            {
                /*
                                enum_name_declaration::=
                                    enum_identifier[ [integral_number[ : integral_number]] ] [ = constant_expression ]
                */
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal enum_identifier");
                    word.SkipToKeyword(";");
                    return;
                }
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();

                if(word.Text == "[")
                {
                    word.MoveNext();
                    Expressions.Expression integral_number = Expressions.Expression.ParseCreate(word, nameSpace);

                    if (integral_number == null)
                    {
                        word.AddError("should be integral_number");
                        word.SkipToKeyword(";");
                        return;
                    }
                    if(word.Text != "]")
                    {
                        word.SkipToKeyword(";");
                        return;
                    }

                    word.MoveNext();
                }

                // [ = constant_expression ]
                if (word.Text == "=")
                {
                    word.MoveNext();
                    Expressions.Expression ex = Expressions.Expression.ParseCreate(word, nameSpace);
                    if (ex != null && !ex.Constant) ex.Reference.AddError("should be constant");
                }

                if(word.Text == ",")
                {
                    word.MoveNext();
                    continue;
                }
                break;
            }

            if (word.Eof || word.Text != "}")
            {
                word.AddError("} expected");
                word.SkipToKeyword(":");
                return;
            }
            word.MoveNext();

            // todo { packed_dimension }

            // list_of_variable_decl_assignments

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal identifier");
                    word.SkipToKeyword(";");
                    return;
                }
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();

                if (word.Text == ",")
                {
                    word.MoveNext();
                    continue;
                }
                break;
            }


            // ;
            if (word.Text == ";")
            {
                word.MoveNext();
            }else{
                word.AddError("; expected");
            }

            return;
        }
    }
}

