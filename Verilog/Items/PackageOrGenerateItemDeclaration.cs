using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Items
{
    public class PackageOrGenerateItemDeclaration
    {
        /*
        package_or_generate_item_declaration ::= 
              net_declaration 
            | data_declaration 
            | task_declaration 
            | function_declaration 
            | checker_declaration 
            | dpi_import_export 
            | extern_constraint_declaration 
            | class_declaration 
            | class_constructor_declaration 
            | local_parameter_declaration ;
            | parameter_declaration ;
            | covergroup_declaration 
            | overload_declaration 
            | assertion_item_declaration 
            | ;
         */
        public static bool Parse(WordScanner word, NameSpace nameSpace)
        {
            switch (word.Text)
            {
                // net_declaration
                case "supply0":
                case "supply1":
                case "tri":
                case "triand":
                case "trior":
                case "tri0":
                case "tri1":
                case "wire":
                case "wand":
                case "wor":
                    Verilog.Variables.Net.ParseCreateFromDeclaration(word, nameSpace);
                    break;
                case "trireg":
                    Verilog.Variables.Trireg.ParseCreateFromDeclaration(word, nameSpace);
                    break;
                //  data_declaration
                //      [ const ] [var][lifetime] data_type_or_implicit
                //          data_type
                //              integer_vector_type[signing] { packed_dimension }
                //                  "bit" | "logic" | "reg"
                case "bit":
                    Verilog.Variables.Bit.ParseCreateFromDeclaration(word, nameSpace);
                    break;
                case "logic":
                    Verilog.Variables.Logic.ParseCreateFromDeclaration(word, nameSpace);
                    break;
                case "reg":
                    Verilog.Variables.Reg.ParseCreateFromDeclaration(word, nameSpace);
                    break;

                //              integer_atom_type[signing]
                //                  "byte" | "shortint" | "int" | "longint" | "integer" | "time"
                case "integer":
                    Verilog.Variables.Integer.ParseCreateFromDeclaration(word, nameSpace);
                    break;
                case "time":
                    Verilog.Variables.Time.ParseCreateFromDeclaration(word, nameSpace);
                    break;

                //              non_integer_type
                //                  "shortreal" | "real" | "realtime"
                case "real":
                    Verilog.Variables.Real.ParseCreateFromDeclaration(word, nameSpace);
                    break;
                case "realtime":
                    Verilog.Variables.RealTime.ParseCreateFromDeclaration(word, nameSpace);
                    break;

                //              struct_union["packed"[signing]] { struct_union_member { struct_union_member } }{ packed_dimension }
                // TODO

                //              "enum"[enum_base_type] {enum_name_declaration { , enum_name_declaration } { packed_dimension }
                case "enum":
                    Verilog.Variables.Enum.ParseCreateFromDataDeclaration(word, nameSpace);
                    break;

                //              "string"
                //              "chandle"
                //              "virtual"["interface"] interface_identifier[parameter_value_assignment][ . modport_identifier] [class_scope | package_scope] type_identifier { packed_dimension } class_type
                //              "event"
                case "event":
                    Verilog.Variables.Event.ParseCreateFromDeclaration(word, nameSpace);
                    break;
                //              ps_covergroup_identifier
                //              type_reference
                //          implicit_data_type
                //      type_declaration

                //      package_import_declaration

                //      net_type_declaration

                // task_declaration
                case "task":
                    Task.Parse(word, nameSpace);
                    break;

                // function_declaration
                case "function":
                    Function.Parse(word, nameSpace);
                    break;

                // checker_declaration
                // TODO

                // dpi_import_export
                // TODO

                // extern_constraint_declaration
                // TODO

                // class_declaration
                //BuildingBlocks.Class.P

                // class_constructor_declaration
                
                // local_parameter_declaration;
                // parameter_declaration;
                case "parameter":
                case "localparam":
                    Verilog.Variables.Parameter.ParseCreateDeclaration(word, nameSpace, null);
                    break;

                // covergroup_declaration
                // overload_declaration
                // assertion_item_declaration

                // ;
                case ";":
                    word.MoveNext();
                    if (!word.SystemVerilog) word.AddError("SystemVerilog description");
                    break;

                // etc
                case "(*":
                    Attribute attribute = Attribute.ParseCreate(word);
                    break;
                // errpr trap
                case "endgenerate":
                    return false;
                case "end":
                    return false;
                case "endmodule":
                    return false;

                default:
                    return false;
            }

            return true;
        }


    }
}
