using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.Snippets
{
    public class AlwaysFFSnippet : codeEditor.CodeEditor.ToolItem
    {
        public AlwaysFFSnippet(codeEditor.CodeEditor.CodeDocument codeDocument) : base("alwaysFF", codeDocument)
        {
            this.codeDocument = codeDocument;
        }
        codeEditor.CodeEditor.CodeDocument codeDocument;

        public override void Apply()
        {
            string replaceText =
                "always @(posedge CLK_I or negedge RST_X)\r\n" +
                "begin\r\n" +
                "\tif(~RST_X) begin\r\n" +
                "\t\t\r\n" +
                "\tend else begin\r\n" +
                "\t\t\r\n" +
                "end";
            codeDocument.Replace(codeDocument.CaretIndex, 0, 0, replaceText);
        }
    }
}
