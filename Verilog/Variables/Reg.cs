using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Variables
{
    public class Reg : Variable
    {
        protected Reg() { }

        public Range Range { get; protected set; }

        public bool Signed { get; protected set; }

        public Reg(string Name, Range range, bool signed)
        {
            this.Name = Name;
            this.Range = range;
            this.Signed = signed;
        }

        public override ajkControls.ColorLabel GetLabel()
        {
            ajkControls.ColorLabel label = new ajkControls.ColorLabel();
            label.AppendText("reg ", CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
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

            label.AppendText(Name, CodeDrawStyle.Color(CodeDrawStyle.ColorType.Register));
            label.AppendText("\r\n");
            return label;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            // reg_declaration::= reg [signed] [range] list_of_variable_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg
            bool signed = false;

            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
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
                word.AddError("illegal reg declaration");
                return;
            }

            Range range = null;
            if (word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, nameSpace);
                if (word.Eof || range == null)
                {
                    word.AddError("illegal reg declaration");
                    return;
                }
            }

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal reg identifier");
                    return;
                }

                Reg reg = new Reg();
                WordReference nameRef = word.GetReference();
                reg.Signed = signed;
                reg.Range = range;
                reg.Name = word.Text;

                // register valiable
                if (word.Active)
                {
                    if (nameSpace.Variables.ContainsKey(reg.Name))
                    {
                        if (nameSpace.Variables[reg.Name] is Net)
                        {
                            nameSpace.Variables.Remove(reg.Name);
                            nameSpace.Variables.Add(reg.Name, reg);
                        }
                        else
                        {
                            nameRef.AddError("duplicated reg name");
                        }
                    }
                    else
                    {
                        nameSpace.Variables.Add(reg.Name, reg);
                    }
                }

                word.Color(CodeDrawStyle.ColorType.Register);
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
                        reg.dimensions.Add(dimension);
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
            }

            return;
        }
    }
}
