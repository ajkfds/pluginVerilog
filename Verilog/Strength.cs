using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    // A.2.2.2 Strengths
    // drive_strength   ::= (strength0, strength1)
    //                      | (strength1, strength0)
    //                      | (strength0, highz1)          
    //                      | (strength1, highz0)          
    //                      | (highz0, strength1)          
    //                      | (highz1, strength0) 
    // strength0        ::= supply0 | strong0 | pull0 | weak0
    // strength1        ::= supply1 | strong1 | pull1 | weak1 
    // charge_strength  ::= (small ) | (medium ) | (large )


    public enum Strengths
    {
        supply0,strong0,pull0,weak0,
        supply1,strong1,pull1,weak1,
        highz0,highz1,
        small,medium,large
    }

    public class DriveStrength
    {
        protected DriveStrength() { }

        public Strengths Strength0;
        public Strengths Strength1;


        public static DriveStrength ParseCreate(WordScanner word, NameSpace nameSpace)
        {
            word.MoveNext(); // (
            DriveStrength ret = new DriveStrength();
            Strengths? strength;

            strength = getStrengths(word, nameSpace);
            if (strength == null)
            {
                word.AddError("illegal strength");
                return null;
            }
            if (
                strength == Strengths.supply0 ||
                strength == Strengths.strong0 ||
                strength == Strengths.pull0 ||
                strength == Strengths.weak0 ||
                strength == Strengths.highz0
                )
            {
                ret.Strength0 = (Strengths)strength;
                if (word.Text != ",")
                {
                    word.AddError("strength1 is required");
                    return null;
                }
                strength = getStrengths(word, nameSpace);
                if (strength == null)
                {
                    word.AddError("illegal strength");
                    return null;
                }
                if (
                    strength == Strengths.supply1 ||
                    strength == Strengths.strong1 ||
                    strength == Strengths.pull1 ||
                    strength == Strengths.weak1 ||
                    strength == Strengths.highz1
                    )
                {
                    ret.Strength1 = (Strengths)strength;
                    if (word.Text != ")")
                    {
                        word.AddError(") required");
                        return null;
                    }
                    word.MoveNext();
                    return ret;
                }
                else
                {
                    word.AddError("illegal strength");
                    return null;
                }
            }
            else if (
               strength == Strengths.supply1 ||
               strength == Strengths.strong1 ||
               strength == Strengths.pull1 ||
               strength == Strengths.weak1 ||
               strength == Strengths.highz1
               )
            {
                ret.Strength1 = (Strengths)strength;
                if (word.Text != ",")
                {
                    word.AddError("strength1 is required");
                    return null;
                }
                strength = getStrengths(word, nameSpace);
                if (strength == null)
                {
                    word.AddError("illegal strength");
                    return null;
                }
                if (
                    strength == Strengths.supply0 ||
                    strength == Strengths.strong0 ||
                    strength == Strengths.pull0 ||
                    strength == Strengths.weak0 ||
                    strength == Strengths.highz0
                    )
                {
                    ret.Strength1 = (Strengths)strength;
                    if (word.Text != ")")
                    {
                        word.AddError(") required");
                        return null;
                    }
                    word.MoveNext();
                    return ret;
                }
                else
                {
                    word.AddError("illegal strength");
                    return null;
                }
            }
            else
            {
                word.AddError("illegal strength");
                return null;
            }
        }

        private static Strengths? getStrengths(WordScanner word, NameSpace nameSpace)
        {
            switch (word.Text)
            {
                case "supply0":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.supply0;
                case "strong0":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.strong0;
                case "pull0":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.pull0;
                case "weak0":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.weak0;
                case "highz0":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.highz0;

                case "supply1":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.supply1;
                case "strong1":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.strong1;
                case "pull1":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.pull1;
                case "weak1":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.weak1;
                case "highz1":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.highz1;

                case "small":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.small;
                case "medium":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.medium;
                case "large":
                    word.Color(CodeDrawStyle.ColorType.Keyword);
                    word.MoveNext();
                    return Strengths.large;
                default:
                    return null;
            }
        }


    }
}
