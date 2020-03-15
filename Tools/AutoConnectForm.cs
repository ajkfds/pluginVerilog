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
            if (textFile == null) return;

            Data.IVerilogRelatedFile verilogRelatedFile = textFile as Data.IVerilogRelatedFile;
            if (verilogRelatedFile == null) return;

            Verilog.ParsedDocument parsedDocument = verilogRelatedFile.VerilogParsedDocument;
            if (parsedDocument == null) return;

            codeEditor.CodeEditor.CodeDocument codeDocument = textFile.CodeDocument;
            if (codeDocument == null) return;

            codeEditor.Data.Project project = textFile.Project;
            if (project == null) return;

            int index = codeDocument.CaretIndex;
            Verilog.Module module = parsedDocument.GetModule(index);


            foreach (var inst in
                module.ModuleInstantiations.Values)
            {
                if (inst.BeginIndex < index && index < inst.LastIndex)
                {
                    setupModule(module, inst, parsedDocument);
                    return;
                }
            }
        }

        private void setupModule(
            Verilog.Module module,
            Verilog.ModuleItems.ModuleInstantiation moduleInstanciation,
            Verilog.ParsedDocument parsedDocument
            )
        {
            Data.VerilogFile source = parsedDocument.ProjectProperty.GetFileOfModule(moduleInstanciation.ModuleName) as Data.VerilogFile;
            if (source == null) return;

            Verilog.ParsedDocument sourceParsedDocument;
            if (moduleInstanciation.ParameterOverrides.Count == 0)
            {
                sourceParsedDocument = source.VerilogParsedDocument;
            }
            else
            {
                sourceParsedDocument = source.GetInstancedParsedDocument(moduleInstanciation.OverrideParameterID) as Verilog.ParsedDocument;
            }
            if (sourceParsedDocument == null) return;
            Verilog.Module sourceModule = sourceParsedDocument.Modules[moduleInstanciation.ModuleName];
            if (sourceModule == null) return;

            Verilog.Module instancedModule = parsedDocument.ProjectProperty.GetModule(moduleInstanciation.ModuleName);

            HeaderLabel header = new HeaderLabel();
            header.AppendText(moduleInstanciation.ModuleName, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Keyword));
            header.AppendText(" ");
            header.AppendText(moduleInstanciation.Name);
            header.AppendText("(");
            colorLabelList.Add(header);

            string sectionName = null;
            foreach (var port in sourceModule.PortsList)
            {
                if (port.SectionName != sectionName)
                {
                    sectionName = port.SectionName;
                    SectionLabel section = new SectionLabel();
                    section.AppendText("// ", Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Comment));
                    section.AppendText(sectionName, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Comment));
                    colorLabelList.Add(section);
                }

                PortLabel portLabel = new PortLabel();
                portLabel.AppendText(".");
                portLabel.AppendText(port.Name, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Identifier));
                portLabel.AppendText(" ( ");
                if (moduleInstanciation.PortConnection.ContainsKey(port.Name))
                {
                    portLabel.AppendLabel(moduleInstanciation.PortConnection[port.Name].GetLabel());
                }
                portLabel.AppendText(" )");
                portLabel.PortName = port.Name;
                colorLabelList.Add(portLabel);
            }

            FooterLabel footer = new FooterLabel();
            footer.AppendText(")");
            colorLabelList.Add(footer);

            checkConnectCantidate(colorLabelList, module, instancedModule);
        }

        private static void checkConnectCantidate(ajkControls.ColorLabelList labelList,Verilog.Module module,Verilog.Module instancedModule)
        {
            foreach(ajkControls.ColorLabel label in labelList)
            {
                if( label is PortLabel )
                {
                    PortLabel portLabel = label as PortLabel;
                    string searchName = portLabel.PortName.ToLower();

                    int matchLength = 0;
                    foreach(var variable in module.Variables.Values)
                    {
                        string checkName = variable.Name.ToLower();
                        
                        if (searchName.Length > matchLength && checkName.Contains(searchName)){ 
                            matchLength = searchName.Length;

                            ajkControls.ColorLabel cantidate = new ajkControls.ColorLabel();
                            cantidate.AppendText(variable.Name, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Variable));
                            portLabel.Cantidate = cantidate;
                        }

                        if(checkName.Length > matchLength && searchName.Contains(checkName))
                        {
                            matchLength = checkName.Length;

                            ajkControls.ColorLabel cantidate = new ajkControls.ColorLabel();
                            cantidate.AppendText(variable.Name, Global.CodeDrawStyle.Color(CodeDrawStyle.ColorType.Variable));
                            portLabel.Cantidate = cantidate;
                        }

                    }



                    if(portLabel.Cantidate != null)
                    {
                        portLabel.AppendText(" -> ");
                        portLabel.AppendLabel(portLabel.Cantidate);
                    }
                }
            }

        }


        private void AutoConnectForm_Load(object sender, EventArgs e)
        {

        }

        private class HeaderLabel : ajkControls.ColorLabel
        {

        }

        private class SectionLabel : ajkControls.ColorLabel
        {

        }

        private class PortLabel : ajkControls.ColorLabel
        {
            public string PortName;
            public ajkControls.ColorLabel Cantidate;
        }

        private class FooterLabel : ajkControls.ColorLabel
        {

        }

        private void colorLabelList_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {

            }else if(e.KeyCode == Keys.Escape)
            {
                Hide();
            }
        }
    }
}
