using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ajkControls;
using ajkControls.ColorLabel;

namespace pluginVerilog.Verilog.Variables
{
    public class Bit : Variable
    {
        protected Bit() { }

        public Range Range { get; protected set; }

        public bool Signed { get; protected set; }

        public Bit(string Name, Range range, bool signed)
        {
            this.Name = Name;
            this.Range = range;
            this.Signed = signed;
        }

        public override void AppendLabel(ColorLabel label)
        {
            AppendTypeLabel(label);
            label.AppendText(Name, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Register));

            foreach (Dimension dimension in Dimensions)
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
            label.AppendText("bit ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            label.AppendText(" ");
            if (Signed)
            {
                label.AppendText("signed ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            }

            if (Range != null)
            {
                label.AppendLabel(Range.GetLabel());
                label.AppendText(" ");
            }
        }

        public static Bit ParseCreateType(WordScanner word, NameSpace nameSpace)
        {
            if (word.Text != "bit") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // bit


            Bit type = new Bit();
            type.Signed = false;

            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }
            if (word.Text == "signed")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                type.Signed = true;
            }
            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }

            type.Range = null;
            if (word.GetCharAt(0) == '[')
            {
                type.Range = Range.ParseCreate(word, nameSpace);
                if (word.Eof || type.Range == null)
                {
                    word.AddError("illegal reg declaration");
                    return null;
                }
            }
            return type;
        }

        public static Bit CreateFromType(string name, Bit type)
        {
            Bit bit = new Bit();
            bit.Signed = type.Signed;
            bit.Range = type.Range;
            bit.Name = name;
            return bit;
        }
        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            // ## Systemverilog
            //         data_declaration ::= [ const ] [var][lifetime] data_type_or_implicit list_of_variable_decl_assignments;
            //                              | ...

            //         data_type_or_implicit ::=  data_type
            //                                    |...

            // data_type::=     integer_vector_type[signing] { packed_dimension }
            //                  |...
            // integer_vector_type: bit | logic | reg

            Bit type = Bit.ParseCreateType(word, nameSpace);
            if (type == null)
            {
                word.SkipToKeyword(";");
                if (word.Text == ";") word.MoveNext();
                return;
            }

            if (!word.SystemVerilog) word.AddError("systemverilog description");

            List<Bit> bits = new List<Bit>();
            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal reg identifier");
                    return;
                }

                Bit bit = Bit.CreateFromType(word.Text, type);
                bits.Add(bit);
                WordReference nameRef = word.GetReference();
                bit.DefinedReference = word.GetReference();

                // register valiable
                if (!word.Active)
                {
                    // skip
                }
                else if (word.Prototype)
                {
                    if (nameSpace.Variables.ContainsKey(bit.Name))
                    {
                        if (nameSpace.Variables[bit.Name] is Net)
                        {
                            nameSpace.Variables.Remove(bit.Name);
                            nameSpace.Variables.Add(bit.Name, bit);
                        }
                        else
                        {
                            //                            nameRef.AddError("duplicated reg name");
                        }
                    }
                    else
                    {
                        nameSpace.Variables.Add(bit.Name, bit);
                    }
                    word.Color(CodeDrawStyle.ColorType.Register);
                }
                else
                {
                    if (nameSpace.Variables.ContainsKey(bit.Name) && nameSpace.Variables[bit.Name] is Reg)
                    {
                        bit = nameSpace.Variables[bit.Name] as Bit;
                    }
                    word.Color(CodeDrawStyle.ColorType.Register);
                }

                word.MoveNext();

                if (word.Text == "=")
                {
                    word.MoveNext();
                    Expressions.Expression initalValue = Expressions.Expression.ParseCreate(word, nameSpace);
                    bit.AssignedReferences.Add(bit.DefinedReference);
                }
                else if (word.Text == "[")
                {
                    while (word.Text == "[")
                    {
                        Dimension dimension = Dimension.ParseCreate(word, nameSpace);
                        if (word.Active && word.Prototype)
                        {
                            bit.dimensions.Add(dimension);
                        }
                    }
                }

                if (word.GetCharAt(0) != ',') break;
                word.MoveNext();
            }

            if (word.Eof || word.GetCharAt(0) != ';')
            {
                word.AddError("; expected");
            }
            else
            {
                word.MoveNext();
                string comment = word.GetFollowedComment();
                foreach (Bit logic in bits)
                {
                    logic.Comment = comment;
                }
            }

            return;
        }
    }
}
