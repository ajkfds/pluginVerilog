using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    // ## Verilog 2001
    // Variable -+-net +-> wire         4state  >=1bit      v
    //           +-data+-> reg          4state  >=1bit      v
    //                 +-> real                             v
    //                 +-> integer              32bit       v
    //                 +-> event                            v
    //                 +-> genvar                           v
    //                 +-> real                             v
    //                 +-> realtime                         v
    //                 +-> reg                              v
    //                 +-> time                             v
    //                 +-> trireg                           v
    // ## SystemVerilog2012
    // Variable -+-data+-> logic        4state  >=1bit      v
    //                 +-> bit          2state  >=1bit      v
    //                 +-> byte         2state  8bit
    //                 +-> shortint     2state  16bit
    //                 +-> int          2state  32bit
    //                 +-> longint      2state  64bit
    //                 +-> shortreal

    public class DataType
    {
        /*
        data_type::=
              integer_vector_type[signing] { packed_dimension }
            | integer_atom_type[signing]
            | non_integer_type
            | struct_union["packed"[signing]] { struct_union_member { struct_union_member } } { packed_dimension }
            | "enum" [enum_base_type] { enum_name_declaration { , enum_name_declaration } { packed_dimension }
            | "string"
            | "chandle"
            | "virtual" ["interface"] interface_identifier[parameter_value_assignment][ . modport_identifier] 
            | [class_scope | package_scope] type_identifier { packed_dimension }
            | class_type
            | "event"
            | ps_covergroup_identifier 
            | type_reference

        integer_atom_type   ::= "byte" | "shortint" | "int" | "longint" | "integer" | "time"
        integer_vector_type ::= "bit" | "logic" | "reg"
        non_integer_type    ::= "shortreal" | "real" | "realtime"

        signing ::= "signed" | "unsigned"
        */
        public static Variable ParseCrateDataType(WordScanner word, NameSpace nameSpace)
        {
            /*
            data_type::=
                  integer_vector_type[signing] { packed_dimension }
                | integer_atom_type[signing]
                | non_integer_type
                | struct_union["packed"[signing]] { struct_union_member { struct_union_member } } { packed_dimension }
                | "enum" [enum_base_type] { enum_name_declaration { , enum_name_declaration } { packed_dimension }
                | "string"
                | "chandle"
                | "virtual" ["interface"] interface_identifier[parameter_value_assignment][ . modport_identifier] 
                | [class_scope | package_scope] type_identifier { packed_dimension }
                | class_type
                | "event"
                | ps_covergroup_identifier 
                | type_reference

            integer_atom_type   ::= "byte" | "shortint" | "int" | "longint" | "integer" | "time"
            integer_vector_type ::= "bit" | "logic" | "reg"
            non_integer_type    ::= "shortreal" | "real" | "realtime"

            signing ::= "signed" | "unsigned"
            */

            // systemverilog data type does not include nets

            switch (word.Text)
            {
                //integer_vector_type::= bit | logic | reg
                case "bit":
                    return Bit.ParseCreateType(word, nameSpace);
                case "logic":
                    return Logic.ParseCreateType(word, nameSpace);
                case "reg":
                    return Reg.ParseCreateType(word, nameSpace);

                //integer_atom_type::= byte | shortint | int | longint | integer | time
                //case "byte":
                //    return Byte.ParseCreateType(word, nameSpace);

                // shortint
                // int
                case "int":
                    return Int.ParseCreateType(word, nameSpace);
                // longint
                case "integer":
                    return Integer.ParseCreateType(word, nameSpace);
                case "time":
                    return Time.ParseCreateType(word, nameSpace);


                //non_integer_type::= shortreal | real | realtime
                // shortreal
                case "real":
                    return Real.ParseCreateType(word, nameSpace);
                case "realtime":
                    return RealTime.ParseCreateType(word, nameSpace);

                // struct_union["packed"[signing]] { struct_union_member { struct_union_member } } { packed_dimension }

                // "enum" [enum_base_type] { enum_name_declaration { , enum_name_declaration } { packed_dimension }
                case "enum":
                    return Enum.ParseCreateType(word, nameSpace);

                // "string"
                // "chandle"
                // "virtual" ["interface"] interface_identifier[parameter_value_assignment][ . modport_identifier] 

                // class_type
                // "event"
                // ps_covergroup_identifier 
                // type_reference
                default:
                    // [class_scope | package_scope] type_identifier { packed_dimension }
                    {
                        //Variable variable = parseCrateDataType_TypeIdentifier(word, nameSpace);
                        //if (variable != null) return variable;
                    }
                    break;
            }
            return null;
        }
    }
}
