using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace pluginVerilog.Verilog.Snippets
{
    public class ModuleInstanceAutocompleteItem : codeEditor.CodeEditor.AutocompleteItem
    {
        public ModuleInstanceAutocompleteItem(string text, byte colorIndex, Color color, codeEditor.Data.Project project) : base(text, colorIndex, color)
        {
            this.project = project;
        }
        codeEditor.Data.Project project;

        public override void Apply(codeEditor.CodeEditor.CodeDocument codeDocument, System.Windows.Forms.Keys keyCode)
        {
            int prevIndex = codeDocument.CaretIndex;
            if (codeDocument.GetLineStartIndex(codeDocument.GetLineAt(prevIndex)) != prevIndex && prevIndex != 0)
            {
                prevIndex--;
            }
            int headIndex, length;
            codeDocument.GetWord(prevIndex, out headIndex, out length);

            char currentChar = codeDocument.GetCharAt(codeDocument.CaretIndex);
            if (currentChar != '\r' && currentChar != '\n') return;

            ProjectProperty projectProperty = project.ProjectProperties[Plugin.StaticID] as ProjectProperty;
            string relativePath = projectProperty.GetRelativeFilePathOfModule(Text);
            if (relativePath == "") return;

            Data.VerilogFile file = project.GetRegisterdItem(Data.VerilogFile.GetID(relativePath,project)) as Data.VerilogFile;
            Verilog.ParsedDocument parsedDocument = file.ParsedDocument as Verilog.ParsedDocument;
            if (parsedDocument == null) return;

            Verilog.Module module = parsedDocument.Modules[Text];
            if (module == null) return;

            StringBuilder sb = new StringBuilder();

            sb.Append(" (\r\n");
            int i = 0;
            foreach (Verilog.Variables.Port port in module.Ports.Values)
            {
                sb.Append("\t.");
                sb.Append(port.Name);
                sb.Append("\t(  )");
                if (i != module.Ports.Count - 1) sb.Append(",");
                sb.Append("\r\n");
                i++;
            }
            sb.Append(");");

            codeDocument.Replace(headIndex, length, ColorIndex, Text + sb.ToString());
            codeDocument.CaretIndex = headIndex + Text.Length;
            codeDocument.SelectionStart = headIndex + Text.Length;
            codeDocument.SelectionLast = headIndex + Text.Length;
        }
    }
}
