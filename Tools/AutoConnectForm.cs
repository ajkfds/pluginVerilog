using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pluginVerilog.Tools
{
    public partial class AutoConnectForm : Form
    {
        public AutoConnectForm()
        {
            InitializeComponent();

            codeEditor.Data.ITextFile textFile = codeEditor.Controller.CodeEditor.GetTextFile();

            /*
            int prevIndex = codeDocument.CaretIndex;
            if (codeDocument.GetLineStartIndex(codeDocument.GetLineAt(prevIndex)) != prevIndex && prevIndex != 0)
            {
                prevIndex--;
            }
            int headIndex, length;
            codeDocument.GetWord(prevIndex, out headIndex, out length);

            char currentChar = codeDocument.GetCharAt(codeDocument.CaretIndex);
            if (currentChar != '\r' && currentChar != '\n') return;

            ProjectProperty projectProperty = project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;

            codeEditor.Data.ITextFile itext = codeEditor.Controller.CodeEditor.GetTextFile();

            if (!(itext is Data.IVerilogRelatedFile)) return;
            var vfile = itext as Data.IVerilogRelatedFile;
            ParsedDocument parsedDocument = vfile.VerilogParsedDocument;
            if (parsedDocument == null) return;
            Module module = parsedDocument.GetModule(vfile.CodeDocument.CaretIndex);
            if (module == null) return;

            Data.VerilogFile instancedFile = projectProperty.GetFileOfModule(Text) as Data.VerilogFile;
            if (instancedFile == null) return;
            Verilog.ParsedDocument instancedParsedDocument = instancedFile.ParsedDocument as Verilog.ParsedDocument;
            if (instancedParsedDocument == null) return;
            Verilog.Module instancedModule = instancedParsedDocument.Modules[Text];
            if (instancedModule == null) return;
            */
        }


        private void AutoConnectForm_Load(object sender, EventArgs e)
        {

        }
    }
}
