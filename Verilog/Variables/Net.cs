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

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            switch (NetType)
            {
                case NetTypeEnum.Supply0:
                    label.AppendText("Supply0", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case NetTypeEnum.Supply1:
                    label.AppendText("Supply1", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case NetTypeEnum.Tri:
                    label.AppendText("Tri", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case NetTypeEnum.Triand:
                    label.AppendText("Triand", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case NetTypeEnum.Trior:
                    label.AppendText("Trior", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case NetTypeEnum.Tri0:
                    label.AppendText("Tri0", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case NetTypeEnum.Tri1:
                    label.AppendText("Tri1", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case NetTypeEnum.Wire:
                    label.AppendText("Wire", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case NetTypeEnum.Wand:
                    label.AppendText("Wand", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
                case NetTypeEnum.Wor:
                    label.AppendText("Wor", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
                    break;
            }

            label.AppendText(" ");
            if (Signed)
            {
                label.AppendText("signed ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            }

            if (Range != null)
            {
                label.AppendLabel(Range.GetLabel());
                label.AppendText(" ");
            }

            label.AppendText(Name, CodeDrawStyle.Color(CodeDrawStyle.ColorType.Net));
            label.AppendText("\r\n");
            return label;
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
            //                      | trireg[drive_strength][vectored | scalared][signed] range[delay3] list_of_net_decl_assignments; 
            //
            //
            // list_of_net_decl_assignments ::= net_decl_assignment { , net_decl_assignment }
            // list_of_net_identifiers      ::= net_identifier [ dimension { dimension }]    { , net_identifier [ dimension { dimension }] }
            // net_decl_assignment          ::= net_identifier = expression
            // dimension                    ::= [ dimension_constant_expression : dimension_constant_expression ]
            // range                        ::= [ msb_constant_expression : lsb_constant_expression ] 

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
                WordReference nameRef = word.GetReference();
                net.Name = word.Text;
                if (word.Active)
                {
                    if (word.Prototype)
                    {
                        if (nameSpace.Variables.ContainsKey(net.Name))
                        {
                            nameRef.AddError("duplicated net name");
                        }
                        else
                        {
                            nameSpace.Variables.Add(net.Name, net);
                        }
                    }
                    else
                    {
                        if(nameSpace.Variables.ContainsKey(net.Name) && nameSpace.Variables[net.Name] is Net)
                        {
                            net = nameSpace.Variables[net.Name] as Net;
                        }
                    }
                }

                word.Color(CodeDrawStyle.ColorType.Net);
                word.MoveNext();

                if (word.Text == "=")
                {
                    word.MoveNext();
                    Expressions.Expression initalValue = Expressions.Expression.ParseCreate(word, nameSpace);
                }
                else if (word.Text == "[")
                {
                    while (word.Text == "[")
                    {
                        Dimension dimension = Dimension.ParseCreate(word, nameSpace);
                        net.dimensions.Add(dimension);
                    }
                }

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
