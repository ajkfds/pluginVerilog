using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ajkControls;
using ajkControls.ColorLabel;
using pluginVerilog.Verilog.DataObjects;
using pluginVerilog.Verilog.DataObjects.DataTypes;

namespace pluginVerilog.Verilog.DataObjects.Variables
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

        public List<Range> PackedDimensions { get; set; } = new List<Range>();

        public DataType BaseType { get; protected set; } = null;
        public List<DataTypes.Enum.Item> Items = new List<DataTypes.Enum.Item>();

        public Range Range {
            get
            {
                if (PackedDimensions.Count < 1) return null;
                return PackedDimensions[0];
            }
        } 


        public override void AppendLabel(ColorLabel label)
        {
            AppendTypeLabel(label);
            label.AppendText(Name, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Register));

            foreach (Range dimension in Dimensions)
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



        public new static void ParseDeclaration(WordScanner word, NameSpace nameSpace)
        {
            DataType dataType = DataObjects.DataTypes.DataType.ParseCreate(word, nameSpace,DataTypeEnum.Int);
            if(!(dataType is DataTypes.Enum)) System.Diagnostics.Debug.Assert(true);
            DataTypes.Enum enumDataType = dataType as DataTypes.Enum;



            // list_of_variable_decl_assignments

            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal identifier");
                    word.SkipToKeyword(";");
                    return;
                }

                word.Color(CodeDrawStyle.ColorType.Variable);
                Enum enum_ = new Enum();
                enum_.Name = word.Text;
                enum_.BaseType = enumDataType.BaseType;
                foreach(var item in enumDataType.Items)
                {
                    enum_.Items.Add(item);
                }

                // register valiable
                if (!word.Active)
                {
                    // skip
                }
                else if (word.Prototype)
                {
                    if (nameSpace.Enums.ContainsKey(enum_.Name))
                    {
                        word.AddError("duplicated enum name");
                    }
                    else
                    {
                        nameSpace.Enums.Add(enum_.Name, enum_);
                    }
                    word.Color(CodeDrawStyle.ColorType.Register);
                }
                else
                {
                    if (nameSpace.Enums.ContainsKey(enum_.Name))
                    {
                        if (nameSpace.Enums[enum_.Name] is Enum)
                        {
                            enum_ = nameSpace.Enums[enum_.Name] as Enum;
                        }
                    }
                }
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


        public class Item
        {
            public string Identifier;
            public int Index;
            public Expressions.Expression Value;
        }
    }
}

