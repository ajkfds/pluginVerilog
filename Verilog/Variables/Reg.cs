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

            List<Reg> regs = new List<Reg>();
            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal reg identifier");
                    return;
                }

                Reg reg = new Reg();
                regs.Add(reg);
                WordReference nameRef = word.GetReference();
                reg.Signed = signed;
                reg.Range = range;
                reg.Name = word.Text;

                // register valiable
                if (!word.Active)
                {
                    // skip
                }else if (word.Prototype)
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
//                            nameRef.AddError("duplicated reg name");
                        }
                    }
                    else
                    {
                        nameSpace.Variables.Add(reg.Name, reg);
                    }
                    word.Color(CodeDrawStyle.ColorType.Register);
                }
                else
                {
                    if(nameSpace.Variables.ContainsKey(reg.Name) && nameSpace.Variables[reg.Name] is Reg)
                    {
                        reg = nameSpace.Variables[reg.Name] as Reg;
                    }
                    word.Color(CodeDrawStyle.ColorType.Register);
                }

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
                        if(word.Active && word.Prototype)
                        {
                            reg.dimensions.Add(dimension);
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
                foreach (Reg reg in regs)
                {
                    reg.Comment = comment;
                }
            }

            return;
        }
    }
}
