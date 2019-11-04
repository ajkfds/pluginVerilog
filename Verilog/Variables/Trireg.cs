using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Trireg : Variable
    {
        public ChargeStrengthEnum ChargeStrength = ChargeStrengthEnum.none;
        public bool Signed = false;
        public DriveStrength DriveStrength = null;

        public Range Range { get; set; }
       
        protected Trireg() { }

        //net_type::= supply0 | supply1 | tri     | triand  | trior | tri0 | tri1 | wire  | wand   | wor
        public enum ChargeStrengthEnum
        {
            none,
            small,
            medium,
            learge
        }

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText("trireg", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));

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

            foreach (Dimension dimension in Dimensions)
            {
                label.AppendText(" ");
                label.AppendLabel(dimension.GetLabel());
            }

            if (Comment != "")
            {
                label.AppendText(" ");
                label.AppendText(Comment.Trim(new char[] { '\r', '\n', '\t', ' ' }), CodeDrawStyle.Color(CodeDrawStyle.ColorType.Comment));
            }

            label.AppendText("\r\n");
            return label;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            // net_declaration ::=    net_type                                          [signed]        [delay3] list_of_net_identifiers;
            //                      | net_type[drive_strength]                          [signed]        [delay3] list_of_net_decl_assignments;
            //                      | net_type                  [vectored | scalared]   [signed] range  [delay3] list_of_net_identifiers;
            //                      | net_type[drive_strength]  [vectored | scalared]   [signed] range  [delay3] list_of_net_decl_assignments;

            //                      | trireg    [charge_strength]                           [signed]        [delay3] list_of_net_identifiers;
            //                      | trireg    [drive_strength]                            [signed]        [delay3] list_of_net_decl_assignments;
            //                      | trireg    [charge_strength]   [vectored | scalared]   [signed] range  [delay3] list_of_net_identifiers;          
            //                      | trireg    [drive_strength]    [vectored | scalared]   [signed] range  [delay3] list_of_net_decl_assignments; 
            //
            //
            // list_of_net_decl_assignments ::= net_decl_assignment { , net_decl_assignment }
            // list_of_net_identifiers      ::= net_identifier [ dimension { dimension }]    { , net_identifier [ dimension { dimension }] }
            // net_decl_assignment          ::= net_identifier = expression
            // dimension                    ::= [ dimension_constant_expression : dimension_constant_expression ]
            // range                        ::= [ msb_constant_expression : lsb_constant_expression ] 

            DriveStrength driveStrength = null;
            ChargeStrengthEnum chargeStrength = ChargeStrengthEnum.none;

            // trireg
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            if(word.Text == "(")
            {
                driveStrength = DriveStrength.ParseCreate(word, nameSpace);
                if(driveStrength == null)
                {
                    switch (word.NextText)
                    {
                        case "small":
                            word.MoveNext();
                            word.Color(CodeDrawStyle.ColorType.Keyword);
                            word.MoveNext();
                            if (word.Text != ")")
                            {
                                word.AddError(") expected");
                                return;
                            }
                            word.MoveNext();
                            chargeStrength = ChargeStrengthEnum.small;
                            break;
                        case "medium":
                            word.MoveNext();
                            word.Color(CodeDrawStyle.ColorType.Keyword);
                            word.MoveNext();
                            if (word.Text != ")")
                            {
                                word.AddError(") expected");
                                return;
                            }
                            word.MoveNext();
                            chargeStrength = ChargeStrengthEnum.medium;
                            break;
                        case "large":
                            word.MoveNext();
                            word.Color(CodeDrawStyle.ColorType.Keyword);
                            word.MoveNext();
                            if (word.Text != ")")
                            {
                                word.AddError(") expected");
                                return;
                            }
                            word.MoveNext();
                            chargeStrength = ChargeStrengthEnum.learge;
                            break;
                        default:
                            break;
                    }
                }
            }

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
            if (!General.IsIdentifier(word.Text))
            {
                word.AddError("illegal net identifier");
                return;
            }
            //[delay3]
            // TODO

            List<Trireg> triregs = new List<Trireg>();
            while (!word.Eof)
            {
                Trireg trireg = new Trireg();
                triregs.Add(trireg);
                trireg.Signed = signed;
                trireg.Range = range;
                trireg.Name = word.Text;
                trireg.DefinedReference = word.GetReference();
                trireg.ChargeStrength = chargeStrength;
                trireg.DriveStrength = driveStrength;
                if (word.Active)
                {
                    if (word.Prototype)
                    {
                        if (nameSpace.Variables.ContainsKey(trireg.Name))
                        {
                            word.RootPointer.AddError(trireg.DefinedReference, "duplicated net name");
                        }
                        else
                        {
                            nameSpace.Variables.Add(trireg.Name, trireg);
                        }
                        word.Color(CodeDrawStyle.ColorType.Net);
                    }
                    else
                    {
                        if (nameSpace.Variables.ContainsKey(trireg.Name) && nameSpace.Variables[trireg.Name] is Net)
                        {
                            trireg = nameSpace.Variables[trireg.Name] as Trireg;
                        }
                        word.Color(CodeDrawStyle.ColorType.Net);
                    }
                }

                word.MoveNext();

                if (word.Text == "=")
                {
                    word.MoveNext();
                    Expressions.Expression initalValue = Expressions.Expression.ParseCreate(word, nameSpace);
                    trireg.AssignedReferences.Add(trireg.DefinedReference);
                }
                else if (word.Text == "[")
                {
                    while (word.Text == "[")
                    {
                        Dimension dimension = Dimension.ParseCreate(word, nameSpace);
                        if (word.Active && word.Prototype)
                        {
                            trireg.dimensions.Add(dimension);
                        }
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
                string comment = word.GetFollowedComment();
                foreach (Trireg trireg in triregs)
                {
                    trireg.Comment = comment;
                }
            }

            return;
        }
    }
}
