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
    public partial class PharseHiarachyForm : Form
    {
        public PharseHiarachyForm(codeEditor.NavigatePanel.NavigatePanelNode rootNode)
        {
            InitializeComponent();
//            Text = projectNode.Project.Name;
            this.rootNode = rootNode;
            this.Icon = ajkControls.Global.Icon;
            this.ShowInTaskbar = false;
            this.BackColor = codeEditor.Controller.GetBackColor();
        }

        private codeEditor.NavigatePanel.NavigatePanelNode rootNode = null;
        private volatile bool close = false;

        private void PharseHiarachyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!close) e.Cancel = true;
            rootNode = null;
        }

        System.Threading.Thread thread = null;
        private void PharseHiarachyForm_Shown(object sender, EventArgs e)
        {
            if (thread != null) return;
            thread = new System.Threading.Thread(() => { worker(); });
            thread.Start();
        }

        private void worker()
        {
            parseHier(rootNode.Item);

            close = true;
            Invoke(new Action(() => { Close(); }));
        }

        private void parseHier(codeEditor.Data.Item item)
        {
            if (item == null) return;
            codeEditor.Data.ITextFile textFile = item as codeEditor.Data.TextFile;
            if (textFile == null) return;
            Invoke(new Action(() => { label.Text = textFile.ID; }));

            codeEditor.CodeEditor.DocumentParser parser = item.CreateDocumentParser(codeEditor.CodeEditor.DocumentParser.ParseModeEnum.BackgroundParse);
            if ( parser != null)
            {
                parser.Parse();
            }
            if (parser.ParsedDocument == null) return;
            textFile.AcceptParsedDocument(parser.ParsedDocument);

            textFile.Update();
            List<codeEditor.Data.Item> items = new List<codeEditor.Data.Item>();
            foreach(codeEditor.Data.Item subItem in textFile.Items.Values)
            {
                items.Add(subItem);
            }
            foreach(codeEditor.Data.Item subitem in items)
            {
                parseHier(subitem);
            }
        }

    }
}
