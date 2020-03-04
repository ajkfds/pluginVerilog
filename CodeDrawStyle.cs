using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog
{
    public class CodeDrawStyle : ajkControls.CodeDrawStyle
    {
        public CodeDrawStyle()
        {
            colors = new Color[16]
            {
                    System.Drawing.Color.FromArgb(212,212,212),     // Normal
                    System.Drawing.Color.FromArgb(150,150,150),     // inactivated
                    System.Drawing.Color.DarkGray,                  // 2
                    System.Drawing.Color.FromArgb(212,0,0),         // Resister
                    System.Drawing.Color.FromArgb(86,156,214),      // keyword
                    System.Drawing.Color.FromArgb(106,153,85),      // Comment
                    System.Drawing.Color.FromArgb(78,201,176),      // identifier
                    System.Drawing.Color.FromArgb(255,94,194),      // Parameter
                    System.Drawing.Color.FromArgb(206,145,120),     // number
                    System.Drawing.Color.FromArgb(255,80,80),       // Net
                    System.Drawing.Color.FromArgb(63,86,52),        // highlighted comment
                    System.Drawing.Color.Pink,                      // Variable
                    System.Drawing.Color.Black,                     // 12
                    System.Drawing.Color.Black,                     // 13
                    System.Drawing.Color.Black,                     // 14
                    System.Drawing.Color.Black                      // 15
            };
        }

        public static byte ColorIndex(ColorType colorType)
        {
            return (byte)colorType;
        }

        public Color Color(ColorType index)
        {
            return colors[(int)index];
        }

        public enum ColorType : byte
        {
            Normal = 0,
            Comment = 5,
            Register = 3,
            Net = 9,
            Variable = 11,
            Paramater = 7,
            Keyword = 4,
            Identifier = 6,
            Number = 8,
            Inactivated = 1,
            HighLightedComment = 10
        }

        public override Color[] MarkColor
        {
            get
            {
                return new System.Drawing.Color[8]
                    {
                        System.Drawing.Color.Red,    // 0 error
                        System.Drawing.Color.Orange, // 1 warning
                        System.Drawing.Color.FromArgb(106,153,85), // 2 notice
                        System.Drawing.Color.FromArgb(86,156,214), // 3 hint
                        System.Drawing.Color.Red, // 4
                        System.Drawing.Color.Red, // 5
                        System.Drawing.Color.Red, // 6
                        System.Drawing.Color.FromArgb(52,58,64)  // 7
                    };
            }
        }

        public override ajkControls.CodeTextbox.MarkStyleEnum[] MarkStyle
        {
            get
            {
                return new ajkControls.CodeTextbox.MarkStyleEnum[8]
                    {
                        ajkControls.CodeTextbox.MarkStyleEnum.wave,    // 0
                        ajkControls.CodeTextbox.MarkStyleEnum.wave,    // 1
                        ajkControls.CodeTextbox.MarkStyleEnum.underLine,
                        ajkControls.CodeTextbox.MarkStyleEnum.wave,
                        ajkControls.CodeTextbox.MarkStyleEnum.underLine,
                        ajkControls.CodeTextbox.MarkStyleEnum.underLine,
                        ajkControls.CodeTextbox.MarkStyleEnum.underLine,
                        ajkControls.CodeTextbox.MarkStyleEnum.fill
                    };
            }
        }

    }
}
