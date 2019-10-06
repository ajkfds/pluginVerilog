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

    public class VerilogHeaderFile : codeEditor.Data.File, codeEditor.Data.ITextFile, IVerilogRelatedFile
    {
        public new static VerilogHeaderFile Create(string relativePath, codeEditor.Data.Project project)
        {
            string id = GetID(relativePath, project);
            if (project.IsRegistered(id))
            {
                VerilogHeaderFile item = project.GetRegisterdItem(id) as VerilogHeaderFile;
                project.RegisterProjectItem(item);
                return item;
            }

            VerilogHeaderFile fileItem = new VerilogHeaderFile();
            fileItem.Project = project;
            fileItem.ID = id;
            fileItem.RelativePath = relativePath;
            if (relativePath.Contains('\\'))
            {
                fileItem.Name = relativePath.Substring(relativePath.LastIndexOf('\\') + 1);
            }
            else
            {
                fileItem.Name = relativePath;
            }

            project.RegisterProjectItem(fileItem);
            return fileItem;
        }

        public override void DisposeItem()
        {
            if (ParsedDocument != null) ParsedDocument.Dispose();
            base.DisposeItem();
        }
        public bool IsCodeDocumentCashed
        {
            get { if (document == null) return false; else return true; }
        }


        private volatile bool parseRequested = false;
        public bool ParseRequested { get { return parseRequested; } set { parseRequested = value; } }

        private volatile bool reloadRequested = false;
        public bool ReloadRequested { get { return reloadRequested; } set { reloadRequested = value; } }
        public void Reload()
        {
            CodeDocument = null;
            if (VerilogParsedDocument != null) VerilogParsedDocument.ReloadIncludeFiles();
        }

        public ProjectProperty ProjectProperty
        {
            get
            {
                return Project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;
            }
        }

        public static VerilogHeaderFile CreateInstance(string relativePath, string ID, codeEditor.Data.Project project)
        {
            if (project.IsRegistered(ID))
            {
                VerilogHeaderFile item = project.GetRegisterdItem(ID) as VerilogHeaderFile;
                project.RegisterProjectItem(item);
                return item;
            }

            VerilogHeaderFile fileItem = new VerilogHeaderFile();
            fileItem.Project = project;
            fileItem.ID = ID;
            fileItem.RelativePath = relativePath;
            if (relativePath.Contains('\\'))
            {
                fileItem.Name = relativePath.Substring(relativePath.LastIndexOf('\\') + 1);
            }
            else
            {
                fileItem.Name = relativePath;
            }

            project.RegisterProjectItem(fileItem);
            return fileItem;
        }

        public codeEditor.CodeEditor.ParsedDocument ParsedDocument { get; set; }

        private codeEditor.CodeEditor.CodeDocument document = null;
        public codeEditor.CodeEditor.CodeDocument CodeDocument
        {
            get
            {
                if (document == null)
                {
                    try
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(Project.GetAbsolutePath(RelativePath)))
                        {
                            document = new codeEditor.CodeEditor.CodeDocument();
                            string text = sr.ReadToEnd();
                            document.Replace(0, 0, 0, text);
                            document.ParentID = ID;
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
                document = value;
            }
        }

        public ajkControls.CodeDrawStyle DrawStyle
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
                return null;
            }
        }

        public override codeEditor.NavigatePanel.NavigatePanelNode CreateNode()
        {
            return new NavigatePanel.VerilogHeaderNode(ID, Project);
        }

        public virtual codeEditor.CodeEditor.DocumentParser CreateDocumentParser(codeEditor.CodeEditor.CodeDocument document, string id, codeEditor.Data.Project project, DocumentParser.ParseModeEnum parseMode)
        {
            return null;
        }

        public void AfterKeyPressed(KeyPressEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void AfterKeyDown(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void BeforeKeyPressed(KeyPressEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void BeforeKeyDown(KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public List<PopupItem> GetPopupItems(int EditId, int index)
        {
            throw new NotImplementedException();
        }

        public List<AutocompleteItem> GetAutoCompleteItems(int index, out string cantidateText)
        {
            throw new NotImplementedException();
        }

        public List<ToolItem> GetToolItems(int index)
        {
            throw new NotImplementedException();
        }
    }
}
