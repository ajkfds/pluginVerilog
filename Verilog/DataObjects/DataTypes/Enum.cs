using pluginVerilog.Verilog.DataObjects.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.DataObjects.DataTypes
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



        public static Enum ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "enum") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.AddSystemVerilogError();
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
                    type.BaseType = new DataTypes.IntegerAtomType();
                    type.BaseType.Type = DataTypeEnum.Int;
                    break;
            }

            if(word.Eof | word.Text != "{")
            {
                word.AddError("{ required");
                return null;
            }
            word.MoveNext(); // "{"

            long prevAssignValue = 0;
            while( !word.Eof | word.Text != "}")
            {
                Item item = Item.ParseCreate(word, nameSpace,ref prevAssignValue);
                if (item == null) break;
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
            public static Item ParseCreate(WordScanner word,NameSpace nameSpace,ref long prevAssignedNumber)
            {
                if (word.Text == "}" | word.Text == ",") return null;
                if (!General.IsIdentifier(word.Text)) return null;

                Item item = new Item();
                item.Identifier = word.Text;
                word.Color(CodeDrawStyle.ColorType.Identifier);
                word.MoveNext();

                if(word.Text == "[")
                {
                    Range range = Range.ParseCreate(word, nameSpace);
                }

                if(word.Text == "=")
                {
                    word.MoveNext();    // =
                    item.Value = Expressions.Expression.ParseCreate(word, nameSpace);
                }
                else
                {

                }

                return item;
            }
        }

    }
}
