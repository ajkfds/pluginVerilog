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
        public override System.Drawing.Color[] ColorPallet
        {
            get
            {
                return colorPallet;
            }
        }

        private static System.Drawing.Color[] colorPallet = new Color[16]
        {
                System.Drawing.Color.DimGray,                   // default
                System.Drawing.Color.LightGray,                 // inactivated
                System.Drawing.Color.DarkGray,                  // 2
                System.Drawing.Color.Crimson,                   // variable-heavy
                System.Drawing.Color.MediumBlue,                // keyword
                System.Drawing.Color.ForestGreen,               // comment
                System.Drawing.Color.CadetBlue,                 // identifier
                System.Drawing.Color.Orchid,                    // variable-fixed
                System.Drawing.Color.SandyBrown,                // number
                System.Drawing.Color.Salmon,                    // variable-light
                System.Drawing.Color.Green,                     // highlighted comment
                System.Drawing.Color.Pink,                     // 11
                System.Drawing.Color.Black,                     // 12
                System.Drawing.Color.Black,                     // 13
                System.Drawing.Color.Black,                     // 14
                System.Drawing.Color.Black                      // 15
        };

        public static byte ColorIndex(ColorType colorType)
        {
            return (byte)colorType;
        }

        public static System.Drawing.Color Color(ColorType colorType)
        {
            return colorPallet[ColorIndex(colorType)];
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
            Number = 8
        }
    }
}
