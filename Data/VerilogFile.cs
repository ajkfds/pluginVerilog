using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Data
{
    public class VerilogFile : codeEditor.Data.File, codeEditor.Data.ITextFile
    {
        public new static VerilogFile Create(string relativePath, codeEditor.Data.Project project)
        {
            string id = GetID(relativePath, project);
            if (project.IsRegistered(id))
            {
                VerilogFile item = project.GetRegisterdItem(id) as VerilogFile;
                project.RegisterProjectItem(item);
                return item;
            }

            VerilogFile fileItem = new VerilogFile();
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

        public codeEditor.CodeEditor.ParsedDocument ParsedDocument { get; set; }
        public Verilog.ParsedDocument VerilogParsedDocument
        {
            get
            {
                return ParsedDocument as Verilog.ParsedDocument;
            }
        }

        public ProjectProperty ProjectProperty
        {
            get
            {
                if (!Project.ProjectProperties.ContainsKey(Plugin.StaticID)) Project.ProjectProperties.Add(Plugin.StaticID, new ProjectProperty());
                return Project.ProjectProperties[Plugin.StaticID] as ProjectProperty;
            }
        }

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
                            document = new CodeEditor.CodeDocument();
                            string text = sr.ReadToEnd();
                            document.Replace(0, 0, 0, text);
                            document.ParentID = ID;
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

        public override codeEditor.NavigatePanel.NavigatePanelNode CreateNode()
        {
            return new NavigatePanel.VerilogFileNode(ID, Project);
        }

        public virtual codeEditor.CodeEditor.DocumentParser CreateDocumentParser(codeEditor.CodeEditor.CodeDocument document, string id, codeEditor.Data.Project project)
        {
            return new Parser.Parser(document, id, project);
        }

        private Dictionary<string, codeEditor.Data.Item> items = new Dictionary<string, codeEditor.Data.Item>();
        public IReadOnlyDictionary<string, codeEditor.Data.Item> Items
        {
            get { return items; }
        }

        public override void Update()
        {
            if(VerilogParsedDocument == null)
            {
                items.Clear();
                return;
            }

            List<string> ids = new List<string>();

            foreach(string id in VerilogParsedDocument.IncludeFiles.Keys){
                ids.Add(id);
            }

            foreach(Verilog.Module module in VerilogParsedDocument.Modules.Values)
            {
                foreach(Verilog.ModuleItems.ModuleInstantiation moduleInstantiation in module.ModuleInstantiations.Values)
                {
                    string relativeFile = ProjectProperty.GetRelativeFilePathOfModule(moduleInstantiation.ModuleName);
                    if (relativeFile == null) continue;
                    ids.Add(Data.VerilogFile.GetID(relativeFile, Project));
                }
            }

            // update

            // remove unused items
            List<codeEditor.Data.Item> removeItems = new List<codeEditor.Data.Item>();
            foreach(codeEditor.Data.Item item in items.Values)
            {
                if (!ids.Contains(item.ID)) removeItems.Add(item);
            }
            foreach (codeEditor.Data.Item item in removeItems)
            {
                items.Remove(item.ID);
            }

            // add new items
            foreach (string id in ids)
            {
                if (items.ContainsKey(id)) continue;
                if (!Project.IsRegistered(id))
                {
                    System.Diagnostics.Debugger.Break();
                    return;
                }
                codeEditor.Data.Item item = Project.GetRegisterdItem(id);
                items.Add(item.ID, item);
            }
        }
        public virtual void AfterKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        {

        }
        public virtual void AfterKeyDown(System.Windows.Forms.KeyEventArgs e)
        {

        }

        public List<codeEditor.CodeEditor.PopupItem> GetPopupItems(int editId, int index)
        {
            if (VerilogParsedDocument == null) return null;
            if (VerilogParsedDocument.EditID != editId) return null;

            int headIndex, length;
            CodeDocument.GetWord(index, out headIndex, out length);
            string text = CodeDocument.CreateString(headIndex, length);
            return VerilogParsedDocument.GetPopupItems(index,text);
        }

    }
}
