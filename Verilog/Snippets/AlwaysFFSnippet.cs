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
            string indent = (codeDocument as CodeEditor.CodeDocument).GetIndentString(codeDocument.CaretIndex);

            string replaceText =
                "always @(posedge CLK_I or negedge RST_X)\r\n" +
                indent+"begin\r\n" +
                indent + "\tif(~RST_X) begin\r\n" +
                indent + "\t\t\r\n" +
                indent + "\tend else begin\r\n" +
                indent + "\t\t\r\n" +
                indent + "\tend\r\n" +
                indent + "end";
            string carletProceed =
                "always @(posedge CLK_I or negedge RST_X)\r\n" +
                indent + "begin\r\n" +
                indent + "\tif(~RST_X) begin\r\n" +
                indent + "\t\t";

            codeDocument.Replace(codeDocument.CaretIndex, 0, 0, replaceText);
            codeDocument.CaretIndex = codeDocument.CaretIndex + carletProceed.Length;
            codeDocument.SelectionStart = codeDocument.CaretIndex;
            codeDocument.SelectionLast = codeDocument.CaretIndex;
        }
    }
}

