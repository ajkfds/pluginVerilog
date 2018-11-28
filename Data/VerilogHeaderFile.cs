using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;

namespace pluginVerilog.Data
{

    public class VerilogHeaderFile : codeEditor.Data.File, codeEditor.Data.ITextFile
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
                return Style.CodeDrawStyle;
            }
        }

        public override codeEditor.NavigatePanel.NavigatePanelNode CreateNode()
        {
            return new NavigatePanel.VerilogHeaderNode(ID, Project);
        }

        public virtual codeEditor.CodeEditor.DocumentParser CreateDocumentParser(codeEditor.CodeEditor.CodeDocument document, string id, codeEditor.Data.Project project)
        {
            return null;
        }

        public virtual void AfterKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        {

        }
        public virtual void AfterKeyDown(System.Windows.Forms.KeyEventArgs e)
        {

        }
        public virtual void BeforeKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        {
        }

        public virtual void BeforeKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
        }

        public List<codeEditor.CodeEditor.PopupItem> GetPopupItems(int EditId, int index)
        {
            return null;
        }

        public List<AutocompleteItem> GetAutoCompleteItems(int index)
        {
            return null;
        }
        public List<codeEditor.CodeEditor.ToolItem> GetToolItems(int index)
        {
            return null;
        }

    }
}
