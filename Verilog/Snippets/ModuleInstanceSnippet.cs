using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;
using System.Windows.Forms;
using System.Drawing;

namespace pluginVerilog.Verilog.Snippets
{
    public class ModuleInstanceSnippet : codeEditor.Snippets.InteractiveSnippet
    {
        public ModuleInstanceSnippet(string text, codeEditor.Data.Project project) : base(text)
        {
            this.project = project;
        }

        private CodeDocument document;
        codeEditor.Data.Project project;

        public override void Apply(CodeDocument codeDocument)
        {
            document = codeDocument;

            ProjectProperty projectProperty = project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;
            string relativePath = projectProperty.GetRelativeFilePathOfModule(Text);
            if (relativePath == "") return;

            Data.VerilogFile file = project.GetRegisterdItem(Data.VerilogFile.GetID(relativePath, project)) as Data.VerilogFile;
            Verilog.ParsedDocument parsedDocument = file.ParsedDocument as Verilog.ParsedDocument;
            if (parsedDocument == null) return;

            Verilog.Module module = parsedDocument.Modules[Text];
            if (module == null) return;

            string replaceText = getReplaceText(module);


            int index = codeDocument.CaretIndex;

            if(initials.Count == 0)
            {
                codeDocument.Replace(index, 0, 0, replaceText);
                codeEditor.Controller.CodeEditor.AbortInteractiveSnippet();
            }
            else
            {
                for (int i = 0; i < initials.Count; i++)
                {
                    string target = "{" + i.ToString() + "}";
                    if (!replaceText.Contains(target)) break;
                    startIndexs.Add(index + replaceText.IndexOf(target));
                    lastIndexs.Add(index + replaceText.IndexOf(target) + initials[i].Length - 1);
                    replaceText = replaceText.Replace(target, initials[i]);
                }

                codeDocument.Replace(index, 0, 0, replaceText);
                codeDocument.CaretIndex = startIndexs[0];
                codeDocument.SelectionStart = startIndexs[0];
                codeDocument.SelectionLast = lastIndexs[0] + 1;

                codeEditor.Controller.CodeEditor.ClearHighlight();
                for (int i = 0; i < startIndexs.Count; i++)
                {
                    codeEditor.Controller.CodeEditor.AppendHighlight(startIndexs[i], lastIndexs[i]);
                }
            }

            codeEditor.Controller.CodeEditor.RequestReparse();
            base.Apply(codeDocument);
        }

        private string getReplaceText(Module module)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");
            if (module.PortParameterNameList.Count > 0)
            {
                sb.Append("#(\r\n");
                bool first = true;
                foreach (string portName in module.PortParameterNameList)
                {
                    if (!first) sb.Append(",\r\n");
                    sb.Append("\t");
                    sb.Append(".");
                    sb.Append(portName);
                    sb.Append("\t( {");
                    sb.Append(i.ToString());
                    i++;
                    initials.Add(module.Parameters[portName].Expression.Value.ToString());
                    sb.Append("} )");
                    first = false;
                }
                sb.Append("\r\n) ");
            }

            sb.Append("{");
            sb.Append(i.ToString());
            i++;
            initials.Add(module.Name+"_");
            sb.Append("}");

            sb.Append(" (\r\n");
            int j = 0;
            foreach (Verilog.Variables.Port port in module.Ports.Values)
            {
                sb.Append("\t.");
                sb.Append(port.Name);
                sb.Append("\t( {");
                sb.Append(i.ToString());
                i++;
                initials.Add(" ");
                sb.Append("} )");
                if (j != module.Ports.Count - 1) sb.Append(",");
                sb.Append("\r\n");
                j++;
            }
            sb.Append(");");

            return sb.ToString();
        }

        private List<string> initials = new List<string>();

        public override void Aborted()
        {
            codeEditor.Controller.CodeEditor.ClearHighlight();
            document = null;
            base.Aborted();
        }

        private List<int> startIndexs = new List<int>();
        private List<int> lastIndexs = new List<int>();

        public override void BeforeKeyDown(object sender, KeyEventArgs e, codeEditor.CodeEditor.AutoCompleteForm autoCompleteForm)
        {
            if (autoCompleteForm == null || autoCompleteForm.Visible == false)
            {
                if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Escape)
                {
                    bool moved;
                    moveToNextHighlight(out moved);
                    if (!moved) codeEditor.Controller.CodeEditor.AbortInteractiveSnippet();
                    e.Handled = true;
                }
            }
        }
        public override void AfterKeyDown(object sender, KeyEventArgs e, codeEditor.CodeEditor.AutoCompleteForm autoCompleteForm)
        {

        }
        public override void AfterAutoCompleteHandled(object sender, KeyEventArgs e, codeEditor.CodeEditor.AutoCompleteForm autoCompleteForm)
        {
            if (e.Handled) // closed
            {
                int i = codeEditor.Controller.CodeEditor.GetHighlightIndex(document.CaretIndex);
                i++;
                if(i+1 >= initials.Count)
                {
                    codeEditor.Controller.CodeEditor.AbortInteractiveSnippet();
                    return;
                }

                codeEditor.Controller.CodeEditor.SelectHighlight(i);
                codeEditor.Controller.CodeEditor.AbortInteractiveSnippet();
                codeEditor.Controller.CodeEditor.RequestReparse();
            }
        }


        private void moveToNextHighlight(out bool moved)
        {
            moved = false;
            moved = false;
            int i = codeEditor.Controller.CodeEditor.GetHighlightIndex(document.CaretIndex);
            if (i == -1) return;
            i++;
            if (i >= initials.Count) return;

            codeEditor.Controller.CodeEditor.SelectHighlight(i);
            moved = true;
        }
    }
}
