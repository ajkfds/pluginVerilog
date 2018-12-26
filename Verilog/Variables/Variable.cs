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
        protected List<Dimension> dimensions = new List<Dimension>();
        public IReadOnlyList<Dimension> Dimensions { get { return dimensions; } }
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

        public static void ParseCreateFromDeclaration(WordScanner word,NameSpace nameSpace)
        {
            // reg_declaration::= reg [signed] [range] list_of_variable_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg
            bool signed = false;

            if(word.Eof)
            {
                word.AddError("illegal reg declaration");
                return;
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
                return;
            }

            Range range = null;
            if(word.GetCharAt(0) == '[')
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
                }

                word.Color(CodeDrawStyle.ColorType.Register);
                word.MoveNext();

                if(word.Text == "=")
                {
                    word.MoveNext();
                    Expressions.Expression initalValue = Expressions.Expression.ParseCreate(word, nameSpace);
                }else if( word.Text == "[")
                {
                    while(word.Text == "[")
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

    public class Integer : Variable
    {
        protected Integer() { }


        public Integer(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
//            integer_declaration::= integer list_of_variable_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal integer identifier");
                    return;
                }
                Integer val = new Integer();
                val.Name = word.Text;
                if (nameSpace.Variables.ContainsKey(val.Name))
                {
                    word.AddError("duplicated integer name");
                }
                else
                {
                    nameSpace.Variables.Add(val.Name, val);
                }

                word.Color(CodeDrawStyle.ColorType.Variable);
                word.MoveNext();

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

    public class Real : Variable
    {
        protected Real() { }


        public Real(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
//            real_declaration::= real list_of_real_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                Real val = new Real();
                val.Name = word.Text;
                if (nameSpace.Variables.ContainsKey(val.Name))
                {
                    if (nameSpace.Variables[val.Name] is Net)
                    {
                        nameSpace.Variables.Remove(val.Name);
                        nameSpace.Variables.Add(val.Name, val);
                    }
                    else
                    {
                        word.AddError("duplicated real name");
                    }
                }
                else
                {
                    nameSpace.Variables.Add(val.Name, val);
                }

                word.Color(CodeDrawStyle.ColorType.Variable);
                word.MoveNext();

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

    public class RealTime : Variable
    {
        protected RealTime() { }


        public RealTime(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
//            realtime_declaration::= realtime list_of_real_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                RealTime val = new RealTime();
                val.Name = word.Text;
                if (nameSpace.Variables.ContainsKey(val.Name))
                {
                    if (nameSpace.Variables[val.Name] is Net)
                    {
                        nameSpace.Variables.Remove(val.Name);
                        nameSpace.Variables.Add(val.Name, val);
                    }
                    else
                    {
                        word.AddError("duplicated real name");
                    }
                }
                else
                {
                    nameSpace.Variables.Add(val.Name, val);
                }

                word.Color(CodeDrawStyle.ColorType.Variable);
                word.MoveNext();

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

    public class Time : Variable
    {
        protected Time() { }


        public Time(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
//            time_declaration::= time list_of_variable_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext(); // reg

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                Time val = new Time();
                val.Name = word.Text;
                if (nameSpace.Variables.ContainsKey(val.Name))
                {
                    if (nameSpace.Variables[val.Name] is Net)
                    {
                        nameSpace.Variables.Remove(val.Name);
                        nameSpace.Variables.Add(val.Name, val);
                    }
                    else
                    {
                        word.AddError("duplicated real name");
                    }
                }
                else
                {
                    nameSpace.Variables.Add(val.Name, val);
                }

                word.Color(CodeDrawStyle.ColorType.Variable);
                word.MoveNext();

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

    public class Event : Variable
    {
        protected Event() { }


        public Event(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
//            event_declaration::= event list_of_event_identifiers ;

        word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                Event val = new Event();
                val.Name = word.Text;
                if (nameSpace.Variables.ContainsKey(val.Name))
                {
                    if (nameSpace.Variables[val.Name] is Net)
                    {
                        nameSpace.Variables.Remove(val.Name);
                        nameSpace.Variables.Add(val.Name, val);
                    }
                    else
                    {
                        word.AddError("duplicated real name");
                    }
                }
                else
                {
                    nameSpace.Variables.Add(val.Name, val);
                }

                word.Color(CodeDrawStyle.ColorType.Variable);
                word.MoveNext();

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

    public class Genvar : Variable
    {
        protected Genvar() { }


        public Genvar(string Name)
        {
            this.Name = Name;
        }

        public static void ParseCreateFromDeclaration(WordScanner word, NameSpace nameSpace)
        {
//            genvar_declaration::= genvar list_of_genvar_identifiers;

            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            while (!word.Eof)
            {
                if (!General.IsSimpleIdentifier(word.Text))
                {
                    word.AddError("illegal real identifier");
                    return;
                }
                Genvar val = new Genvar();
                val.Name = word.Text;
                if (nameSpace.Variables.ContainsKey(val.Name))
                {
                    if (nameSpace.Variables[val.Name] is Net)
                    {
                        nameSpace.Variables.Remove(val.Name);
                        nameSpace.Variables.Add(val.Name, val);
                    }
                    else
                    {
                        word.AddError("duplicated real name");
                    }
                }
                else
                {
                    nameSpace.Variables.Add(val.Name, val);
                }

                word.Color(CodeDrawStyle.ColorType.Variable);
                word.MoveNext();

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
    /*



    variable_type ::=  variable_identifier [ = constant_expression ] | variable_identifier dimension { dimension } 
    list_of_variable_identifiers ::= variable_type { , variable_type }
    */
}
