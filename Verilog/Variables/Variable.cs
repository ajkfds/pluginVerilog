using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.Verilog.Variables
{
    // Variable -+-> net
    //           +-> reg
    //           +-> real
    //           +-> integer

    public class Variable
    {
        public string Name;
    }

    public class VariablePopup : codeEditor.CodeEditor.PopupItem
    {
        public VariablePopup(Variable variable)
        {
            if(variable is Net)
            {
                color =　CodeDrawStyle.Color(CodeDrawStyle.ColorType.Net);
                icon = new ajkControls.Icon(Properties.Resources.netBox);
                iconColorStyle = ajkControls.Icon.ColorStyle.Red;
            }
            else if(variable is Reg)
            {
                color = CodeDrawStyle.Color(CodeDrawStyle.ColorType.Net);
                icon = new ajkControls.Icon(Properties.Resources.regBox);
                iconColorStyle = ajkControls.Icon.ColorStyle.Red;
            }
            else
            {
                color = Color.Black;
            }
            text = variable.Name;
        }

        private string text;
        private ajkControls.Icon icon = null;
        private ajkControls.Icon.ColorStyle iconColorStyle;
        private Color color;

        public override Size GetSize(Graphics graphics, Font font)
        {
            Size tsize = System.Windows.Forms.TextRenderer.MeasureText(graphics, text, font);
            return new Size(tsize.Width + tsize.Height + (tsize.Height >> 2), tsize.Height);
        }

        public override void Draw(Graphics graphics, int x, int y, Font font, Color backgroundColor)
        {
            Size tsize = System.Windows.Forms.TextRenderer.MeasureText(graphics, text, font);
            if (icon != null) graphics.DrawImage(icon.GetImage(tsize.Height, iconColorStyle), new Point(x, y));
            Color bgColor = backgroundColor;
            System.Windows.Forms.TextRenderer.DrawText(
                graphics,
                text,
                font,
                new Point(x + tsize.Height + (tsize.Height >> 2), y),
                color,
                bgColor,
                System.Windows.Forms.TextFormatFlags.NoPadding
                );
        }

    }

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
            WAnd,
            Wor
        }
        public static Net ParseCreateDeclaration(WordScanner word, NameSpace nameSpace)
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
                    netType = NetTypeEnum.WAnd;
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
                return null;
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
                return null;
            }

            // [range]
            Range range = null;
            if (word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, nameSpace);
                if (word.Eof || range == null)
                {
                    word.AddError("illegal net declaration");
                    return null;
                }
            }
            if (!General.IsSimpleIdentifier(word.Text))
            {
                word.AddError("illegal net identifier");
                return null;
            }
            //[delay3]
            // TODO

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

            while (word.GetCharAt(0) == ',')
            {
                word.MoveNext(); // ,
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal net identifier");
                    break;
                }
                net = new Net();
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
                    word.Color(CodeDrawStyle.ColorType.Net);
                    word.MoveNext();
                }
            }

            if (word.Eof || word.GetCharAt(0) != ';')
            {
                word.AddError("; expected");
            }
            else
            {
                word.MoveNext();
            }

            return net;
        }
    }

    public class Reg : Variable
    {
        protected Reg() { }

        public Range Range { get; protected set; }
        public bool Signed { get; protected set; }

        public Reg(string Name,Range range,bool signed)
        {
            this.Name = Name;
            this.Range = range;
            this.Signed = signed;
        }

        public static Reg ParseCreateDeclaration(WordScanner word,NameSpace nameSpace)
        {
            // reg_declaration::= reg [signed] [range] list_of_variable_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg
            bool signed = false;

            if(word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }
            if(word.Text == "signed")
            {
                word.Color(CodeDrawStyle.ColorType.Keyword);
                word.MoveNext();
                signed = true;
            }
            if (word.Eof)
            {
                word.AddError("illegal reg declaration");
                return null;
            }

            Range range = null;
            if(word.GetCharAt(0) == '[')
            {
                range = Range.ParseCreate(word, nameSpace);
                if (word.Eof || range == null)
                {
                    word.AddError("illegal reg declaration");
                    return null;
                }
            }
            if (!General.IsSimpleIdentifier(word.Text))
            {
                word.AddError("illegal reg identifier");
                return null;
            }
            Reg reg = new Reg();
            reg.Signed = signed;
            reg.Range = range;
            reg.Name = word.Text;
            if (nameSpace.Variables.ContainsKey(reg.Name))
            {
                if(nameSpace.Variables[reg.Name] is Net)
                {
                    nameSpace.Variables.Remove(reg.Name);
                    nameSpace.Variables.Add(reg.Name, reg);
                }
                else
                {
                    word.AddError("duplicated reg name");
                }
            }
            else
            {
                nameSpace.Variables.Add(reg.Name, reg);
            }

            word.Color(CodeDrawStyle.ColorType.Register);
            word.MoveNext();

            while(word.GetCharAt(0) == ',')
            {
                word.MoveNext(); // ,
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal reg identifier");
                    break;
                }
                reg = new Reg();
                reg.Signed = signed;
                reg.Range = range;
                reg.Name = word.Text;
                if (nameSpace.Variables.ContainsKey(reg.Name))
                {
                    if (nameSpace.Variables[reg.Name] is Net)
                    {
                        nameSpace.Variables.Remove(reg.Name);
                        nameSpace.Variables.Add(reg.Name, reg);
                    }
                    else
                    {
                        word.AddError("duplicated reg name");
                    }
                }
                else
                {
                    nameSpace.Variables.Add(reg.Name, reg);
                    word.Color(CodeDrawStyle.ColorType.Register);
                    word.MoveNext();
                }
            }

            if (word.Eof || word.GetCharAt(0) != ';')
            {
                word.AddError("; expected");
            }
            else
            {
                word.MoveNext();
            }

            return reg;
        }

    }
    /*
    A.2.1.3 Type declarations
    event_declaration   ::= event list_of_event_identifiers ;  
    genvar_declaration  ::= genvar list_of_genvar_identifiers ;  
    integer_declaration ::= integer list_of_variable_identifiers ;  
    net_declaration     ::=   net_type                                              [ signed ]          [ delay3 ]  list_of_net_identifiers ;
                            | net_type  [ drive_strength ]                          [ signed ]          [ delay3 ]  list_of_net_decl_assignments ;
                            | net_type  [ vectored | scalared ]                     [ signed ]  range   [ delay3 ]  list_of_net_identifiers ;
                            | net_type  [ drive_strength ] [ vectored | scalared ]  [ signed ]  range   [ delay3 ]  list_of_net_decl_assignments ;
                            | trireg    [ charge_strength ]                         [ signed ]          [ delay3 ]  list_of_net_identifiers ;          
                            | trireg    [ drive_strength ]                          [ signed ]          [ delay3 ]  list_of_net_decl_assignments ;          
                            | trireg    [ charge_strength ] [ vectored | scalared ] [ signed ]  range   [ delay3 ]  list_of_net_identifiers ;
                            | trireg    [ drive_strength ] [ vectored | scalared ]  [ signed ]  range   [ delay3 ]  list_of_net_decl_assignments ;
    real_declaration    ::= real list_of_real_identifiers ;  
    realtime_declaration    ::= realtime list_of_real_identifiers ;  
    reg_declaration     ::= reg [ signed ] [ range ] list_of_variable_identifiers ;  
    time_declaration    ::= time list_of_variable_identifiers ;


    variable_type ::=  variable_identifier [ = constant_expression ] | variable_identifier dimension { dimension } 
    list_of_variable_identifiers ::= variable_type { , variable_type }
    */
}
