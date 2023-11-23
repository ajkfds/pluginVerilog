using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.Verilog.Variables
{
    // #SystemVeriog 2012
    //	net												user-defined-size	4state	v
    //
    //	variabel	+ integer_vector_type	+ bit 		user-defined-size	2state	sv
    //										+ logic		user-defined-size	4state  sv
    //										+ reg		user-defined-size	4state	v
    //
    //				+ integer_atom_type		+ byte		8bit signed			2state  sv
    //										+ shortint	16bit signed		2state  sv
    //										+ int		32bit signed		2state  sv
    //										+ longint	64bit signed		2state  sv
    //										+ integer	32bit signed		4state	v
    //										+ time		64bit unsigned		        v
    //
    //            	+ non_integer_type		+ shortreal	                            sv
    //										+ real		                            v
    //										+ realtime	                            v

    // net datat type : logic/integer/reg


    /*
    data objects definition
        - variables
            data storage element
                (value type)
                    integer_vector_type (integer with range)
                        bit | logic | reg
                    integer_atom_type
                         byte | shortint | int | longint | integer | time
                    non_integer_type
                        shortreal | real | realtime
                    struct_union
                    enum
                    string
                    event
                    chandle

                (reference type?)
                class_type	// object
                interface_identifier
                ps_covergroup_identifier

                (etc)
                type_identifier	// userdefined type
        - nets
            net type
                buit-in
                    wire
                    wand
                    wor
                    tri
                    triand
                    trior
                    tri0
                    tri1
                    trireg
                    supply0
                    supply1
                    uwire
                user-defined

     */

    public class Variable : CommentAnnotated, IVariableOrNet
    {
        public string Name { set; get; }
        public string Comment { set; get; } = "";
        public WordReference DefinedReference { set; get; } = null;
        public DataTypes.DataTypeEnum DataType = DataTypes.DataTypeEnum.Reg;
        public List<Variables.Range> Dimensions { get; set; } = new List<Variables.Range>();

        public List<WordReference> UsedReferences { set; get; } = new List<WordReference>();
        public List<WordReference> AssignedReferences { set; get; } = new List<WordReference>();
        public int DisposedIndex = -1;

        protected Variable() { }

        public static Variable Create(DataTypes.DataType dataType)
        {
            switch (dataType.Type)
            {
                case DataTypes.DataTypeEnum.Logic:
                case DataTypes.DataTypeEnum.Bit:
                case DataTypes.DataTypeEnum.Reg:
                    return IntegerVectorValueVariable.Create(dataType);
                case DataTypes.DataTypeEnum.Byte:
                case DataTypes.DataTypeEnum.Shortint:
                case DataTypes.DataTypeEnum.Int:
                case DataTypes.DataTypeEnum.Longint:
                case DataTypes.DataTypeEnum.Integer:
                case DataTypes.DataTypeEnum.Time:
                    return IntegerAtomVariable.Create(dataType);

                case DataTypes.DataTypeEnum.Real:
                    return Real.Create(dataType);
                default:
                    break;
            }
            return null;
        }



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

        /*
        data_declaration ::= 
              [ const ] [ var ] [ lifetime ] data_type_or_implicit list_of_variable_decl_assignments ;
            | type_declaration 
            | package_import_declaration
              net_type_declaration

        package_import_declaration ::= 
            import package_import_item { , package_import_item } ;

        package_import_item ::= 
            package_identifier "::" identifier 
            | package_identifier ":: *"

        data_type_or_implicit ::= 
            data_type 
            | implicit_data_type
        implicit_data_type ::= [ signing ] { packed_dimension } 
        */

        // list_of_variable_decl_assignments ::= variable_decl_assignment { , variable_decl_assignment } 

        // variable_decl_assignment ::=   variable_identifier { variable_dimension } [ = expression]
        //                              | dynamic_array_variable_identifier unsized_dimension { variable_dimension }    [ = dynamic_array_new] 
        //                              | class_variable_identifier [ = class_new]

        // class_new ::= [class_scope] new [(list_of_arguments)] 
        //               | new expression

        public static bool ParseDeclaration(WordScanner word, NameSpace nameSpace)
        {
            DataTypes.DataType dataType = DataTypes.DataType.ParseCreate(word, nameSpace);
            if (dataType == null) return false;
            if(
                dataType.Type != DataTypes.DataTypeEnum.Integer &&
                dataType.Type != DataTypes.DataTypeEnum.Logic &&
                dataType.Type != DataTypes.DataTypeEnum.Reg
                )
            {
                word.AddError("should be 4-state type");
            }

            List<Variable> vars = new List<Variable>();

            while (!word.Eof && word.Text !=";")
            {
                string name = word.Text;
                if (!General.IsIdentifier(name))
                {
                    word.AddError("illegal identifier");
                    break;
                }

                Variable variable = Variable.Create(dataType);
                variable.DefinedReference = word.GetReference();

                switch (dataType.Type)
                {
                    case DataTypes.DataTypeEnum.Reg:
                        word.Color(CodeDrawStyle.ColorType.Register);
                        break;
                    default:
                        word.Color(CodeDrawStyle.ColorType.Variable);
                        break;
                }
                word.MoveNext();

                if (word.Text != "=") continue;
                word.MoveNext();    // =

                Expressions.Expression exp = Expressions.Expression.ParseCreate(word, nameSpace);

                if(word.Text == ",")
                {
                    word.MoveNext();
                }
                else
                {
                    break;
                }
            }
            if(word.Text == ";")
            {
                word.MoveNext();
            }
            else
            {
                word.AddError("; required");
            }

            string comment = word.GetFollowedComment();

            if(comment != "")
            {
                foreach (Variable variable in vars)
                {
                    variable.Comment = comment;
                }
            }

            return true;
        }

        //public static bool ParseCreateFromDataDeclaration(WordScanner word, NameSpace nameSpace)
        //{
        //    DataTypes.DataType datatype = DataTypes.DataType.ParseCreate(word, nameSpace);
        //    if (datatype == null) return false;


        //    while(!word.Eof && word.Text != ";")
        //    {
        //        string name = word.Text;
        //        if (!General.IsIdentifier(name))
        //        {
        //            word.AddError("iilegal identifier");
        //        }
        //        word.MoveNext();



        //    }

        //    return true;
        //}




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
