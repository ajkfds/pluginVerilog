using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Net : Variable
    {
        public NetTypeEnum NetType = NetTypeEnum.Wire;
        public bool Signed = false;
        public int DefinedIndex = 0;

        public Range Range { get; set; }

        //net_type::= supply0 | supply1 | tri     | triand  | trior | tri0 | tri1 | wire  | wand   | wor
        public enum NetTypeEnum
        {
            Supply0,
            Supply1,
            Tri,
            Triand,
            Trior,
            Tri0,
            Tri1,
            Wire,
            Wand,
            Wor
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            // net_declaration ::=    net_type                                          [signed]        [delay3] list_of_net_identifiers;
            //                      | net_type[drive_strength]                          [signed]        [delay3] list_of_net_decl_assignments;
            //                      | net_type                  [vectored | scalared]   [signed] range  [delay3] list_of_net_identifiers;
            //                      | net_type[drive_strength]  [vectored | scalared]   [signed] range  [delay3] list_of_net_decl_assignments;

            //                      | trireg[charge_strength][signed][delay3] list_of_net_identifiers;
            //                      | trireg[drive_strength][signed][delay3] list_of_net_decl_assignments;
            //                      | trireg[charge_strength][vectored | scalared][signed] range[delay3] list_of_net_identifiers;          
            //                      | trireg[drive_strength][vectored | scalared][signed] range[delay3] list_of_net_decl_assignments;              /*
            NetTypeEnum netType = NetTypeEnum.Wire;
            switch (word.Text)
            {
                case "supply0":
                    netType = NetTypeEnum.Supply0;
                    break;
                case "supply1":
                    netType = NetTypeEnum.Supply1;
                    break;
                case "tri":
                    netType = NetTypeEnum.Tri;
                    break;
                case "triand":
                    netType = NetTypeEnum.Triand;
                    break;
                case "trior":
                    netType = NetTypeEnum.Trior;
                    break;
                case "tri0":
                    netType = NetTypeEnum.Tri0;
                    break;
                case "tri1":
                    netType = NetTypeEnum.Tri1;
                    break;
                case "wire":
                    netType = NetTypeEnum.Wire;
                    break;
                case "wand":
                    netType = NetTypeEnum.Wand;
                    break;
                case "wor":
                    netType = NetTypeEnum.Wor;
                    break;
                default:
                    System.Diagnostics.Debugger.Break();
                    break;
            }
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            // [drive_strength]

            // [vectored | scalared]

            // [signed]

            // [signed]


            bool signed = false;

            if (word.Eof)
            {
                word.AddError("illegal net declaration");
                return;
            }
            if (word.Text == "signed")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                signed = true;
            }
            if (word.Eof)
            {
                word.AddError("illegal net declaration");
                return;
            }

            // [range]
            Range range = null;
            if (word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, nameSpace);
                if (word.Eof || range == null)
                {
                    word.AddError("illegal net declaration");
                    return;
                }
            }
            if (!General.IsSimpleIdentifier(word.Text))
            {
                word.AddError("illegal net identifier");
                return;
            }
            //[delay3]
            // TODO

            while (!word.Eof)
            {
                Net net = new Net();
                net.Signed = signed;
                net.Range = range;
                net.Name = word.Text;
                if (nameSpace.Variables.ContainsKey(net.Name))
                {
                    word.AddError("duplicated net name");
                }
                else
                {
                    nameSpace.Variables.Add(net.Name, net);
                }

                word.Color(CodeDrawStyle.ColorType.Net);
                word.MoveNext();

                if (word.GetCharAt(0) != ',') break;
                word.MoveNext(); // ,
            }

            if (word.Eof || word.GetCharAt(0) != ';')
            {
                word.AddError("; expected");
            }
            else
            {
                word.MoveNext();
            }

            return;
        }
    }
}
