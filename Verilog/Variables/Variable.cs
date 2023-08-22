using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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


    public class Variable : CommentAnnotated
    {
        public string Name;
        protected List<Dimension> dimensions = new List<Dimension>();
        public IReadOnlyList<Dimension> Dimensions { get { return dimensions; } }
        public string Comment = "";
        public WordReference DefinedReference = null;
        public List<WordReference> UsedReferences = new List<WordReference>();
        public List<WordReference> AssignedReferences = new List<WordReference>();
        public int DisposedIndex = -1;

        public virtual void AppendLabel(ajkControls.ColorLabel.ColorLabel label)
        {
            label.AppendText(Name);
        }

        public virtual void AppendTypeLabel(ajkControls.ColorLabel.ColorLabel label)
        {

        }

        public virtual Variable Clone()
        {
            Variable val = new Variable();
            return val;
        }

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
                        Variable variable = parseCrateDataType_TypeIdentifier(word, nameSpace);
                        if (variable != null) return variable;
                    }
                    break;
            }
            return null;
        }
        public static void ParseCreateFromDataDeclaration(WordScanner word, NameSpace nameSpace)
        {
            Variable type = ParseCrateDataType(word, nameSpace);
            ParseCreateFromDataDeclaration(word, nameSpace,type);
        }
        public static void ParseCreateFromDataDeclaration(WordScanner word, NameSpace nameSpace,Variable type)
        {

            // integer_vector_type
            // integer_vector_type::= "bit" | "logic" | "reg"
            // bit
            if (type is Logic)
            {
                Logic.ParseCreateFromDeclaration(word, nameSpace, type as Logic);
                return;
            }
            if (type is Reg)
            {
                Reg.ParseCreateFromDeclaration(word, nameSpace, type as Reg);
                return;
            }

            // integer_atom_type
            // integer_atom_type::= byte | shortint | int | longint | integer | time
            // byte
            // shortint
            // int
            // longint
            // integer
            if(type is Integer)
            {
                Integer.ParseCreateFromDeclaration(word, nameSpace, type as Integer);
                return;
            }
            // time
            if (type is Time)
            {
                Time.ParseCreateFromDeclaration(word, nameSpace, type as Time);
                return;
            }

            // non_integer_type
            // non_integer_type::= "shortreal" | "real" | "realtime"
            // shortreal
            // real
            if (type is Real)
            {
                Real.ParseCreateFromDeclaration(word, nameSpace, type as Real);
                return;
            }
            // realtime
            if (type is RealTime)
            {
                RealTime.ParseCreateFromDeclaration(word, nameSpace, type as RealTime);
                return;
            }

            // struct_union

            // enum
            if (type is Enum)
            {
                Enum.ParseCreateFromDeclaration(word, nameSpace, type as Enum);
                return;
            }

            //| "string"
            //| "chandle"
            //| interface_identifier
            //| type_identifier
            //| class_type
            //| event
            //| ps_covergroup_identifier
            //| type_reference

            if (type is Enum)
            {
                Enum.ParseCreateFromDeclaration(word, nameSpace, type as Enum);
                return;
            }
        }


        private static Variable parseCrateDataType_TypeIdentifier(WordScanner word, NameSpace nameSpace)
        {
            // [class_scope | package_scope] type_identifier { packed_dimension }
            // class_scope::= class_type"::"
            // class_type ::= ps_class_identifier [parameter_value_assignment] { ::class_identifier[parameter_value_assignment] }
            // package_scope ::=    package_identifier "::"
            //                      | $unit"::"
            // parameter_identifier ::= identifier port_identifier::= identifier

            if (!nameSpace.Typedefs.ContainsKey(word.Text)) return null;

            return nameSpace.Typedefs[word.Text].VariableType;
        }

        public static Variable ParseCreateType(WordScanner word, NameSpace nameSpace)
        {
            Variable val = ParseCrateDataType(word, nameSpace);
            if (val != null) return val;

            switch (word.Text)
            {
                //net_type::= supply0 | supply1 | tri | triand | trior | trireg | tri0 | tri1 | uwire | wire | wand | wor
                case "supply0":
                case "supply1":
                case "tri":
                case "triand":
                case "trior":
                case "trireg":
                case "tri0":
                case "tri1":
                case "uwire":
                case "wire":
                case "wand":
                case "wor":
                    return Net.ParseCreateType(word, nameSpace);
            }
            return null;
        }


        // comment annotation

        private Dictionary<string, string> commentAnnotations = new Dictionary<string, string>();
        public Dictionary<string, string> CommentAnnotations { get { return commentAnnotations; } }
        public void AppendAnnotation(string key, string value)
        {
            if (commentAnnotations.ContainsKey(key))
            {
                string oldValue = commentAnnotations[key];
                string newValue = oldValue + "," + value;
                commentAnnotations[key] = newValue;
            }
            else
            {
                commentAnnotations.Add(key, value);
            }
        }
        public List<string> GetAnnotations(string key)
        {
            if (commentAnnotations.ContainsKey(key))
            {
                string values = commentAnnotations[key];
                return values.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }
            else
            {
                return null;
            }
        }

    }

    /* Verilog 2001
    variable_type ::=  variable_identifier [ = constant_expression ] | variable_identifier dimension { dimension } 
    list_of_variable_identifiers ::= variable_type { , variable_type }
    */

    /* SystemVerilog
        A.2.2.1 Net and variable types 

        casting_type ::= simple_type | constant_primary | signing | string | const

        data_type ::= 
              integer_vector_type [ signing ] { packed_dimension } 
                    // bit, logic, reg
            | integer_atom_type [ signing ] 
                    // byte, shortint, int, longint, integer, time
            | non_integer_type
                    // shortreal | real | realtime
            | struct_union [ packed [ signing ] ] { struct_union_member { struct_union_member } } { packed_dimension }
                    // struct, union
            | enum [ enum_base_type ] { enum_name_declaration { , enum_name_declaration } } { packed_dimension } 
            | string
            | chandle
            | virtual [ interface ] interface_identifier [ parameter_value_assignment ] [ . modport_identifier ] 
            | [ class_scope | package_scope ] type_identifier { packed_dimension } 
            | class_type 
            | event
            | ps_covergroup_identifier 
            | type_reference

        data_type_or_implicit ::= 
            data_type 
            | implicit_data_type 

        implicit_data_type ::= [ signing ] { packed_dimension } 

        enum_base_type ::= 
            integer_atom_type [ signing ] 
            | integer_vector_type [ signing ] [ packed_dimension ] 
            | type_identifier [ packed_dimension ]

        enum_name_declaration ::= 
            enum_identifier [ [ integral_number [ : integral_number ] ] ] [ = constant_expression ] 

        class_scope ::= class_type ::

        class_type ::= 
            ps_class_identifier [ parameter_value_assignment ] 
            { :: class_identifier [ parameter_value_assignment ] }
    
        integer_type            ::= integer_vector_type | integer_atom_type 
        integer_atom_type       ::= byte | shortint | int | longint | integer | time
        integer_vector_type     ::= bit | logic | reg

        non_integer_type ::= shortreal | real | realtime

        net_type ::= supply0 | supply1 | tri | triand | trior | trireg| tri0 | tri1 | uwire| wire | wand | wor
        net_port_type:= 
            [ net_type ] data_type_or_implicit 
            | net_type_identifier 
            | interconnect implicit_data_type 

        variable_port_type ::= var_data_type 

        var_data_type ::= data_type | var data_type_or_implicit 

        signing ::= signed | unsigned

        simple_type ::= integer_type | non_integer_type | ps_type_identifier | ps_parameter_identifier

        struct_union_member ::= 
            { attribute_instance } [random_qualifier] data_type_or_void list_of_variable_decl_assignments ;

        data_type_or_void ::= data_type | void
        struct_union ::= struct | union [ tagged ] 
        type_reference ::= 
                            type ( expression )
                            | type ( data_type )
    */


    /* SystemVerilog
     * 
        net_declaration ::=
            net_type [ drive_strength | charge_strength ] [ vectored | scalared ] data_type_or_implicit [ delay3 ] list_of_net_decl_assignments ;
                //
            | net_type_identifier [ delay_control ] list_of_net_decl_assignments ;
                // net_type_identifier : user defined nettype
            | interconnect implicit_data_type [ # delay_value ] net_identifier { unpacked_dimension } [ , net_identifier { unpacked_dimension }] ;
                // 

        net_type ::= supply0 | supply1 | tri | triand | trior | trireg | tri0 | tri1 | uwire | wire | wand | wor

        drive_strength ::=        ( strength0 , strength1 )
                                | ( strength1 , strength0 )
                                | ( strength0 , highz1 )
                                | ( strength1 , highz0 )
                                | ( highz0 , strength1 )
                                | ( highz1 , strength0 )
        strength0 ::= supply0 | strong0 | pull0 | weak0
        strength1 ::= supply1 | strong1 | pull1 | weak1
        charge_strength ::= ( small ) | ( medium ) | ( large )

        delay3 ::= # delay_value | # ( mintypmax_expression [ , mintypmax_expression [ , mintypmax_expression ] ] )
        delay2 ::= # delay_value | # ( mintypmax_expression [ , mintypmax_expression ] )
        delay_value ::= 
            unsigned_number 
            | real_number 
            | ps_identifier 
            | time_literal 
            | 1step
        list_of_net_decl_assignments ::= net_decl_assignment { , net_decl_assignment }
        net_decl_assignment ::= net_identifier { unpacked_dimension } [ = expression ]      

        data_type_or_implicit ::=   data_type | implicit_data_type 

        implicit_data_type ::= [ signing ] { packed_dimension } 
     */
    /*
        net_declaration ::= 
            net_type [ drive_strength | charge_strength ] [ vectored | scalared ]  data_type_or_implicit [ delay3 ] list_of_net_decl_assignments ;
            | net_type_identifier [ delay_control ] list_of_net_decl_assignments ;
            | interconnect implicit_data_type [ # delay_value ] net_identifier { unpacked_dimension } [ , net_identifier { unpacked_dimension }] ;
            net_type ::= supply0 | supply1 | tri | triand | trior | trireg| tri0 | tri1 | uwire| wire | wand | wor
     */

    /* 
    * DataDeclaration
        data_declaration ::= 
                        [ const ] [ var ] [ lifetime ] data_type_or_implicit list_of_variable_decl_assignments ;
                        | type_declaration 
                                : typedef
                        | package_import_declaration net_type_declaration
                                : nettype

        data_type_or_implicit ::= 
                        data_type 
                        | implicit_data_type 

        data_type ::= 
                integer_vector_type [ signing ] { packed_dimension } 
                | integer_atom_type [ signing ] 
                | non_integer_type 
                | struct_union [ packed [ signing ] ] { struct_union_member { struct_union_member } }
                { packed_dimension }
                | enum [ enum_base_type ] { enum_name_declaration { , enum_name_declaration } }
                { packed_dimension } 
                | string
                | chandle
                | virtual [ interface ] interface_identifier [ parameter_value_assignment ] [ . modport_identifier ] 
                | [ class_scope | package_scope ] type_identifier { packed_dimension } 
                | class_type 
                | event
                | ps_covergroup_identifier 
                | type_reference

        implicit_data_type ::= [ signing ] { packed_dimension } 

     * DataType
        integer_vector_type : bit
                            : logic
                            : reg
        integer_atom_type   : byte
                            : shortint
                            : int
                            : longint
                            : integer
                            : time
        non_integer_type    : shortreal
                            : real
                            : realtime
                            | struct_union
                            | enum
                            | string
                            | chandle
                            | virtual [ interface ] interface_identifier [ parameter_value_assignment ] [ . modport_identifier ] 
                            | [ class_scope | package_scope ] type_identifier { packed_dimension } 
                            | class_type 
                            | event
                            | ps_covergroup_identifier 
                            | type_reference
    */
}
