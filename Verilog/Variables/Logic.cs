using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ajkControls;
using ajkControls.ColorLabel;

namespace pluginVerilog.Verilog.Variables
{
    public class Logic : Variable
    {
        protected Logic() { }

        public Range Range { get; protected set; }

        public bool Signed { get; protected set; }

        public Logic(string Name, Range range, bool signed)
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
            label.AppendText("logic ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
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
        public static new Logic ParseCreateType(WordScanner word, NameSpace nameSpace)
        {
            // ## Verilog2001
            // reg_declaration::= reg [signed] [range] list_of_variable_identifiers;

            // ## Systemverilog
            //         data_declaration ::= [ const ] [var][lifetime] data_type_or_implicit list_of_variable_decl_assignments;
            //                              | ...

            //         data_type_or_implicit ::=  data_type
            //                                    |...

            // data_type::=     integer_vector_type[signing] { packed_dimension }
            //                  |...
            // integer_vector_type: bit | logic | reg

            if (word.Text != "logic") System.Diagnostics.Debugger.Break();
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // logic

            Logic ret = new Logic();
            ret.Signed = false;



            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }

            if (word.Text == "signed")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                ret.Signed = true;
            }else if(word.Text == "unsigned")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                ret.Signed = false;
            }

            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }

            Range range = null;
            if (word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, nameSpace);
                if (word.Eof || range == null)
                {
                    word.AddError("illegal reg declaration");
                    return null;
                }
            }

            ret.Range = range;
            return ret;
        }
        public static Logic CreateFromType(string name, Logic type)
        {
            Logic ret = new Logic();
            ret.Signed = type.Signed;
            ret.Range = type.Range;
            ret.Name = name;
            return ret;
        }
        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
            Logic type = Logic.ParseCreateType(word, nameSpace);
            if (type == null)
            {
                word.SkipToKeyword(";");
                if (word.Text == ";") word.MoveNext();
                return;
            }

            ParseCreateFromDeclaration(word, nameSpace, type);
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace,Logic type)
        {
            List<Logic> logics = new List<Logic>();
            while (!word.Eof)
            {
                if (!General.IsIdentifier(word.Text))
                {
                    word.AddError("illegal reg identifier");
                    return;
                }

                Logic logic = Logic.CreateFromType(word.Text, type);
                logics.Add(logic);
                WordReference nameRef = word.GetReference();
                logic.DefinedReference = word.GetReference();

                // register valiable
                if (!word.Active)
                {
                    // skip
                }else if (word.Prototype)
                {
                    if (nameSpace.Variables.ContainsKey(logic.Name))
                    {
                        if (nameSpace.Variables[logic.Name] is Net)
                        {
                            nameSpace.Variables.Remove(logic.Name);
                            nameSpace.Variables.Add(logic.Name, logic);
                        }
                        else
                        {
//                            nameRef.AddError("duplicated reg name");
                        }
                    }
                    else
                    {
                        nameSpace.Variables.Add(logic.Name, logic);
                    }
                    word.Color(CodeDrawStyle.ColorType.Register);
                }
                else
                {
                    if(nameSpace.Variables.ContainsKey(logic.Name) && nameSpace.Variables[logic.Name] is Reg)
                    {
                        logic = nameSpace.Variables[logic.Name] as Logic;
                    }
                    word.Color(CodeDrawStyle.ColorType.Register);
                }

                word.MoveNext();

                if (word.Text == "=")
                {
                    word.MoveNext();
                    Expressions.Expression initalValue = Expressions.Expression.ParseCreate(word, nameSpace);
                    logic.AssignedReferences.Add(logic.DefinedReference);
                }
                else if (word.Text == "[")
                {
                    while (word.Text == "[")
                    {
                        Dimension dimension = Dimension.ParseCreate(word, nameSpace);
                        if(word.Active && word.Prototype)
                        {
                            logic.dimensions.Add(dimension);
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
                foreach (Logic logic in logics)
                {
                    logic.Comment = comment;
                }
            }

            return;
        }
    }
}
