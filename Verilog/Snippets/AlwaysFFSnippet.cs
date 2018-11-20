using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;

namespace pluginVerilog.Verilog.Snippets
{
    public class AlwaysFFSnippet : codeEditor.CodeEditor.ToolItem
    {
        public AlwaysFFSnippet() : base("alwaysFF")
        {
        }

        public override void Apply(CodeDocument codeDocument)
        {
            string replaceText =
                "always @(posedge CLK_I or negedge RST_X)\r\n" +
                "begin\r\n" +
                "\tif(~RST_X) begin\r\n" +
                "\t\t\r\n" +
                "\tend else begin\r\n" +
                "\t\t\r\n" +
                "\tend\r\n" +
                "end";
            string carletProceed =
                "always @(posedge CLK_I or negedge RST_X)\r\n" +
                "begin\r\n" +
                "\tif(~RST_X) begin\r\n" +
                "\t\t";

            codeDocument.Replace(codeDocument.CaretIndex, 0, 0, replaceText);
            codeDocument.CaretIndex = codeDocument.CaretIndex + carletProceed.Length;
            codeDocument.SelectionStart = codeDocument.CaretIndex;
            codeDocument.SelectionLast = codeDocument.CaretIndex;
        }
    }
}

