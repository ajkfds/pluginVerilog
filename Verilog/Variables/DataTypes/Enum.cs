using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables.DataTypes
{
    public class Enum : DataType
    {
        public override DataTypeEnum Type
        {
            get
            {
                return DataTypeEnum.Enum;
            }
        }

        public DataType BaseType { get; protected set; } = null;
        public List<Item> Items = new List<Item>();


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

        // A type_identifier shall be legal as an enum_base_type if it denotes an integer_atom_type, with which an additional
        // packed dimension is not permitted, or an integer_vector_type.

        public static new Enum ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "enum") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            if (!word.SystemVerilog) word.AddError("systemverilog description");
            word.MoveNext();

            Enum type = new Enum();

            // baseType
            switch (word.Text)
            {
                //integer_vector_type::= bit | logic | reg
                case "bit":
                case "logic":
                case "reg":
                    type.BaseType = IntegerVectorType.ParseCreate(word, nameSpace);
                    break;
                //integer_atom_type::= byte | shortint | int | longint | integer | time
                case "byte":
                case "shortint":
                case "int":
                case "longint":
                case "integer":
                case "time":
                    type.BaseType =  IntegerAtomType.ParseCreate(word, nameSpace);
                    break;
                default:
                    // In the absence of a data type declaration, the default data type shall be int.
                    // Any other data type used with enumerated types shall require an explicit data type declaration.
                    type.BaseType = new DataTypes.Int();
                    break;
            }

            if(word.Eof | word.Text != "{")
            {
                word.AddError("{ required");
                return null;
            }

            while( !word.Eof | word.Text != "}")
            {
                Item item = Item.ParseCreate(word, nameSpace);
                if (item != null) type.Items.Add(item);

                if (word.Text == ",")
                {
                    word.MoveNext();
                    if (word.Text == "}") word.AddError("illegal comma");
                }
            }

            if (word.Eof | word.Text != "}")
            {
                word.AddError("{ required");
                return null;
            }
            word.MoveNext();

            return type;
        }

        public class Item
        {
            public string Identifier;
            public int Index;
            public Expressions.Expression Value;

            /*
            enum_name_declaration::=
                enum_identifier[ [integral_number[ : integral_number]] ] [ = constant_expression ]
            */
            public static Item ParseCreate(WordScanner word,NameSpace nameSpace)
            {
                if (word.Text == "}" | word.Text == ",") return null;
                if (!General.IsIdentifier(word.Text)) return null;

                Item item = new Item();
                item.Identifier = word.Text;
                word.Color(CodeDrawStyle.ColorType.Identifier);
                word.MoveNext();

                if (word.Text != "=") return item;
                word.MoveNext();

                item.Value = Expressions.Expression.ParseCreate(word, nameSpace);

                return item;
            }
        }

    }
}
