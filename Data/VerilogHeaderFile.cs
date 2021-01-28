using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using codeEditor.CodeEditor;
using codeEditor.Data;
using pluginVerilog.Verilog;

namespace pluginVerilog.Data
{

    public class VerilogHeaderFile : codeEditor.Data.TextFile, IVerilogRelatedFile
    {
        public new static VerilogHeaderFile Create(string relativePath, codeEditor.Data.Project project)
        {
            //string id = GetID(relativePath, project);

            VerilogHeaderFile fileItem = new VerilogHeaderFile();
            fileItem.Project = project;
           fileItem.RelativePath = relativePath;
            if (relativePath.Contains('\\'))
            {
                fileItem.Name = relativePath.Substring(relativePath.LastIndexOf('\\') + 1);
            }
            else
            {
                fileItem.Name = relativePath;
            }

            return fileItem;
        }

        public override codeEditor.CodeEditor.CodeDocument CodeDocument
        {
            get
            {
                if (document != null && document as CodeEditor.CodeDocument == null) System.Diagnostics.Debugger.Break();
                if (document == null)
                {
                    try
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(Project.GetAbsolutePath(RelativePath)))
                        {
                            document = new CodeEditor.CodeDocument(this);
                            string text = sr.ReadToEnd();
                            document.Replace(0, 0, 0, text);
                            document.ClearHistory();
                        }
                    }
                    catch
                    {
                        document = null;
                    }
                }
                return document;
            }
            protected set
            {
                if (value != null && value as CodeEditor.CodeDocument == null) System.Diagnostics.Debugger.Break();
                document = value as CodeEditor.CodeDocument;
            }
        }

        public override void Dispose()
        {
            if (ParsedDocument != null) ParsedDocument.Dispose();
            base.Dispose();
        }

        public ProjectProperty ProjectProperty
        {
            get
            {
                return Project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;
            }
        }

        public static VerilogHeaderFile CreateInstance(string relativePath, codeEditor.Data.Project project)
        {
            //if (project.IsRegistered(ID))
            //{
            //    VerilogHeaderFile item = project.GetRegisterdItem(ID) as VerilogHeaderFile;
            //    project.RegisterProjectItem(item);
            //    return item;
            //}

            VerilogHeaderFile fileItem = new VerilogHeaderFile();
            fileItem.Project = project;
            fileItem.RelativePath = relativePath;
            if (relativePath.Contains('\\'))
            {
                fileItem.Name = relativePath.Substring(relativePath.LastIndexOf('\\') + 1);
            }
            else
            {
                fileItem.Name = relativePath;
            }

            //project.RegisterProjectItem(fileItem);
            return fileItem;
        }


        public override ajkControls.CodeDrawStyle DrawStyle
        {
            get
            {
                return Global.CodeDrawStyle;
            }
        }

        public Verilog.ParsedDocument VerilogParsedDocument
        {
            get
            {
                return ParsedDocument as Verilog.ParsedDocument;
            }
        }

        public override codeEditor.NavigatePanel.NavigatePanelNode CreateNode()
        {
            return new NavigatePanel.VerilogHeaderNode(this);
        }

        public new virtual codeEditor.CodeEditor.DocumentParser CreateDocumentParser(DocumentParser.ParseModeEnum parseMode)
        {
            return null;
        }

        public new void AfterKeyPressed(KeyPressEventArgs e)
        {
            throw new NotImplementedException();
        }

        public new void AfterKeyDown(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public new void BeforeKeyPressed(KeyPressEventArgs e)
        {
            throw new NotImplementedException();
        }

        public new void BeforeKeyDown(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public new List<PopupItem> GetPopupItems(int EditId, int index)
        {
            throw new NotImplementedException();
        }

        public new List<AutocompleteItem> GetAutoCompleteItems(int index, out string cantidateText)
        {
            throw new NotImplementedException();
        }

        public new List<ToolItem> GetToolItems(int index)
        {
            throw new NotImplementedException();
        }
    }
}
