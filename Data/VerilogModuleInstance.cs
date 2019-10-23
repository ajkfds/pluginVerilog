using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;
using codeEditor.Data;

namespace pluginVerilog.Data
{
    public class VerilogModuleInstance : codeEditor.Data.Item, codeEditor.Data.ITextFile, IVerilogRelatedFile
    {
        public static VerilogModuleInstance Create( Verilog.ModuleItems.ModuleInstantiation moduleInstantiation, codeEditor.Data.Project project)
        {
            ProjectProperty projectPropery = project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;
            string relativePath = projectPropery.GetRelativeFilePathOfModule(moduleInstantiation.ModuleName);

            string id = GetID(relativePath,moduleInstantiation.Name,moduleInstantiation.ParameterOverrides,project);
            if (project.IsRegistered(id))
            {
                VerilogModuleInstance item = project.GetRegisterdItem(id) as VerilogModuleInstance;
                project.RegisterProjectItem(item);
                return item;
            }

            VerilogModuleInstance fileItem = new VerilogModuleInstance();
            fileItem.ParameterOverrides = moduleInstantiation.ParameterOverrides;
            fileItem.Project = project;
            fileItem.ID = id;
            fileItem.RelativePath = relativePath;
            fileItem.Name = moduleInstantiation.Name;
            fileItem.ModuleName = moduleInstantiation.ModuleName;
            //if (relativePath.Contains('\\'))
            //{
            //    fileItem.Name = relativePath.Substring(relativePath.LastIndexOf('\\') + 1);
            //}
            //else
            //{
            //    fileItem.Name = relativePath;
            //}
            fileItem.ParseRequested = true;

            project.RegisterProjectItem(fileItem);

//            System.Diagnostics.Debug.Print("create " + id);
            return fileItem;
        }
        public override void DisposeItem()
        {
//            System.Diagnostics.Debug.Print("dispose " + ID);
            if (ParsedDocument != null)
            {
                foreach (var incFile in VerilogParsedDocument.IncludeFiles.Values)
                {
                    incFile.DisposeItem();
                }
            }
            CodeDocument = null;
            if (ParsedDocument != null) ParsedDocument.Dispose();
            base.DisposeItem();
        }
        public bool IsCodeDocumentCashed
        {
            get { if (document == null) return false; else return true; }
        }

        public string ModuleName { set; get; }

        public static string GetID(string relativePath,string instanceName,Dictionary<string,Verilog.Expressions.Expression> parameterOverrides ,codeEditor.Data.Project project)
        {
            if(parameterOverrides.Count == 0) return project.ID + ":ModuleInstance:"+instanceName+":"+ relativePath;
            StringBuilder sb = new StringBuilder();
            sb.Append(project.ID);
            sb.Append(":ModuleInstance:");
            sb.Append(instanceName);
            sb.Append(":");
            sb.Append(relativePath);
            sb.Append(":");
            foreach (var kvp in parameterOverrides)
            {
                sb.Append(kvp.Key);
                sb.Append("=");
                sb.Append(kvp.Value.ConstantValueString());
                sb.Append(",");
            }
            return sb.ToString();
        }

        public static string GetParsedDocumentID(string relativePath, Dictionary<string, Verilog.Expressions.Expression> parameterOverrides, codeEditor.Data.Project project)
        {
            if (parameterOverrides.Count == 0) return project.ID + ":ModuleInstance:" + relativePath;
            StringBuilder sb = new StringBuilder();
            sb.Append(project.ID);
            sb.Append(":ModuleInstance:");
            sb.Append(relativePath);
            sb.Append(":");
            foreach (var kvp in parameterOverrides)
            {
                sb.Append(kvp.Key);
                sb.Append("=");
                sb.Append(kvp.Value.ConstantValueString());
                sb.Append(",");
            }
            return sb.ToString();
        }

        public Dictionary<string, Verilog.Expressions.Expression> ParameterOverrides;

        private volatile bool parseRequested = false;
        public bool ParseRequested { get { return parseRequested; } set { parseRequested = value; } }

        private volatile bool reloadRequested = false;
        public bool ReloadRequested { get { return reloadRequested; } set { reloadRequested = value; } }
        public void Reload()
        {
            if (VerilogParsedDocument != null) VerilogParsedDocument.ReloadIncludeFiles();
            CodeDocument = null;
        }

        public codeEditor.CodeEditor.ParsedDocument ParsedDocument { get; set; }
        //public codeEditor.CodeEditor.ParsedDocument ParsedDocument {
        //    get
        //    {
        //        string id;
        //        if(ParameterOverrides.Count == 0)
        //        {
        //            id = Data.VerilogFile.GetID(RelativePath, Project);
        //            return (Project.GetRegisterdItem(id) as VerilogFile).ParsedDocument;
        //        }
        //        else
        //        {
        //            id = GetParsedDocumentID(RelativePath, ParameterOverrides, Project);
        //            if (ProjectProperty.IsRegisteredParsedDocument(id))
        //            {
        //                return (Project.GetRegisterdItem(id) as VerilogFile).ParsedDocument;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    set
        //    {
        //        string id;
        //        if (ParameterOverrides.Count == 0)
        //        {
        //            id = Data.VerilogFile.GetID(RelativePath, Project);
        //            (Project.GetRegisterdItem(id) as VerilogFile).ParsedDocument = value;
        //        }
        //        else
        //        {
        //            id = GetParsedDocumentID(RelativePath, ParameterOverrides, Project);
        //            if (ProjectProperty.IsRegisteredParsedDocument(id))
        //            {
        //                (Project.GetRegisterdItem(id) as VerilogFile).ParsedDocument = value;
        //            }
        //            else
        //            {
        //                return;
        //            }
        //        }
        //   }
        //}

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
                return Project.GetProjectProperty(Plugin.StaticID) as ProjectProperty;
            }
        }

        /*
        public codeEditor.CodeEditor.CodeDocument CodeDocument
        {
            get
            {
                string id = Data.VerilogFile.GetID(RelativePath, Project);
                Item item = Project.GetRegisterdItem(id);
                if(item is VerilogFile)
                {
                    return (item as VerilogFile).CodeDocument;
                }
                else
                {
                    return null;
                }
            }
        }
        */
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

        public override codeEditor.NavigatePanel.NavigatePanelNode CreateNode()
        {
            return new NavigatePanel.VerilogModuleInstanceNode(ID, Project);
        }

        public virtual codeEditor.CodeEditor.DocumentParser CreateDocumentParser(codeEditor.CodeEditor.CodeDocument document, string id, codeEditor.Data.Project project,codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode)
        {
            if(ParameterOverrides.Count == 0)
            {
                return new Parser.VerilogParser(document, id, project, parseMode);
            }
            else
            {
                return new Parser.VerilogParser(document,
                    ModuleName,
                    ParameterOverrides,
                    id, project, parseMode);
            }
        }


        public override void Update()
        {
            if (VerilogParsedDocument == null)
            {
                foreach (Item item in items.Values)
                {
                    item.DisposeItem();
                }
                items.Clear();
                return;
            }
            if (ID == "TestRTL:File:SCOPE.v")
            {
                string a = "";
            }

            List<string> currentIds = new List<string>();
            foreach (string id in VerilogParsedDocument.IncludeFiles.Keys)
            {
                currentIds.Add(id);
            }

            Dictionary<string, Item> newItems = new Dictionary<string, Item>();

            // add new item
            foreach (Verilog.Module module in VerilogParsedDocument.Modules.Values)
            {
                foreach (Verilog.ModuleItems.ModuleInstantiation moduleInstantiation in module.ModuleInstantiations.Values)
                {
                    string relativeFile = ProjectProperty.GetRelativeFilePathOfModule(moduleInstantiation.ModuleName);
                    if (relativeFile == null) continue;
                    string id = Data.VerilogModuleInstance.GetID(relativeFile, moduleInstantiation.Name, moduleInstantiation.ParameterOverrides, Project);
                    if (!items.Keys.Contains(id) && !newItems.ContainsKey(id)) // new item
                    {
                        // create & increment project counter
                        Item item = Data.VerilogModuleInstance.Create(moduleInstantiation, Project);
                        newItems.Add(item.ID, item);
                    }
                    currentIds.Add(id);
                }
            }

            List<string> removeIds = new List<string>();
            foreach (codeEditor.Data.Item item in items.Values)
            {
                if (!currentIds.Contains(item.ID)) removeIds.Add(item.ID);
            }

            foreach (string id in removeIds)
            {
                if (Project.IsRegistered(id))
                {
                    Item item = Project.GetRegisterdItem(id);
                    items.Remove(item.ID);
                    item.DisposeItem();
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }
            }

            foreach (Item item in newItems.Values)
            {
                items.Add(item.ID, item);
            }

            /*
            List<string> ids = new List<string>();

            foreach (string id in VerilogParsedDocument.IncludeFiles.Keys)
            {
                ids.Add(id);
            }

            foreach (Verilog.Module module in VerilogParsedDocument.Modules.Values)
            {
                foreach (Verilog.ModuleItems.ModuleInstantiation moduleInstantiation in module.ModuleInstantiations.Values)
                {
                    string relativeFile = ProjectProperty.GetRelativeFilePathOfModule(moduleInstantiation.ModuleName);
                    if (relativeFile == null) continue;
                    string id = Data.VerilogModuleInstance.GetID(relativeFile, moduleInstantiation.Name, moduleInstantiation.ParameterOverrides, Project);
                    Data.VerilogModuleInstance.Create(moduleInstantiation, Project);
                    ids.Add(id);
                }
            }

            // update

            // remove unused items
            List<codeEditor.Data.Item> removeItems = new List<codeEditor.Data.Item>();
            foreach (codeEditor.Data.Item item in items.Values)
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
            */
        }

        public virtual void AfterKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                if (VerilogParsedDocument == null) return;
                switch (e.KeyCode)
                {
                    case System.Windows.Forms.Keys.Return:
                        applyAutoInput();
                        break;
                    case System.Windows.Forms.Keys.Space:
                        break;
                    default:
                        break;
                }
            }

            public virtual void AfterKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
            {
                if (VerilogParsedDocument == null) return;
            }

            public virtual void BeforeKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
            {
            }

            public virtual void BeforeKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
            }

            public List<codeEditor.CodeEditor.PopupItem> GetPopupItems(int editId, int index)
            {
                if (VerilogParsedDocument == null) return null;
                if (VerilogParsedDocument.EditID != editId) return null;

                int headIndex, length;
                CodeDocument.GetWord(index, out headIndex, out length);
                string text = CodeDocument.CreateString(headIndex, length);
                return VerilogParsedDocument.GetPopupItems(index, text);
            }


            public List<codeEditor.CodeEditor.ToolItem> GetToolItems(int index)
            {
                List<codeEditor.CodeEditor.ToolItem> toolItems = new List<codeEditor.CodeEditor.ToolItem>();
                toolItems.Add(new Verilog.Snippets.AlwaysFFSnippet());
                return toolItems;
            }

            public List<codeEditor.CodeEditor.AutocompleteItem> GetAutoCompleteItems(int index, out string cantidateWord)
            {
                cantidateWord = null;

                if (VerilogParsedDocument == null) return null;
                int line = CodeDocument.GetLineAt(index);
                int lineStartIndex = CodeDocument.GetLineStartIndex(line);

                List<codeEditor.CodeEditor.AutocompleteItem> items = VerilogParsedDocument.GetAutoCompleteItems(index, lineStartIndex, line, (CodeEditor.CodeDocument)CodeDocument, out cantidateWord);
                return items;
            }



            private void applyAutoInput()
            {
                int index = CodeDocument.CaretIndex;
                int line = CodeDocument.GetLineAt(index);
                if (line == 0) return;
                int prevLine = line - 1;

                int lineHeadIndex = CodeDocument.GetLineStartIndex(line);
                int prevLineHeadIndex = CodeDocument.GetLineStartIndex(prevLine);

                int prevTabs = 0;
                for (int i = prevLineHeadIndex; i < lineHeadIndex; i++)
                {
                    char ch = CodeDocument.GetCharAt(i);
                    if (ch == '\t')
                    {
                        prevTabs++;
                    }
                    else
                    {
                        break;
                    }
                }
                int indentLength = 0;
                for (int i = lineHeadIndex; i < CodeDocument.Length; i++)
                {
                    char ch = CodeDocument.GetCharAt(i);
                    if (ch == '\t')
                    {
                        indentLength++;
                    }
                    else if (ch == ' ')
                    {
                        indentLength++;
                    }
                    else
                    {
                        break;
                    }
                }


                bool prevBegin = isPrevBegin(lineHeadIndex);
                bool nextEnd = isNextEnd(lineHeadIndex);

                if (prevBegin)
                {
                    if (nextEnd) // caret is sandwiched beteen begin and end
                    {
                        // BEFORE
                        // begin[enter] end

                        // AFTER
                        // begin
                        //     [caret]
                        // end
                        CodeDocument.Replace(lineHeadIndex, indentLength, 0, new String('\t', prevTabs + 1) + "\r\n" + new String('\t', prevTabs));
                        CodeDocument.CaretIndex = CodeDocument.CaretIndex + prevTabs + 1 + 1 - indentLength;
                        return;
                    }
                    else
                    {   // add indent
                        prevTabs++;
                    }
                }

                CodeDocument.Replace(lineHeadIndex, indentLength, 0, new String('\t', prevTabs));
                CodeDocument.CaretIndex = CodeDocument.CaretIndex + prevTabs - indentLength;
            }

            private bool isPrevBegin(int index)
            {
                int prevInex = index;
                if (prevInex > 0) prevInex--;

                if (prevInex > 0 && CodeDocument.GetCharAt(prevInex) == '\n') prevInex--;
                if (prevInex > 0 && CodeDocument.GetCharAt(prevInex) == '\r') prevInex--;

                if (prevInex == 0 || CodeDocument.GetCharAt(prevInex) != 'n') return false;
                prevInex--;
                if (prevInex == 0 || CodeDocument.GetCharAt(prevInex) != 'i') return false;
                prevInex--;
                if (prevInex == 0 || CodeDocument.GetCharAt(prevInex) != 'g') return false;
                prevInex--;
                if (prevInex == 0 || CodeDocument.GetCharAt(prevInex) != 'e') return false;
                prevInex--;
                if (CodeDocument.GetCharAt(prevInex) != 'b') return false;
                return true;
            }

            private bool isNextEnd(int index)
            {
                int prevInex = index;
                if (prevInex < CodeDocument.Length &&
                    (
                        CodeDocument.GetCharAt(prevInex) == ' ' || CodeDocument.GetCharAt(prevInex) == '\t'
                    )
                ) prevInex++;

                if (prevInex >= CodeDocument.Length || CodeDocument.GetCharAt(prevInex) != 'e') return false;
                prevInex++;
                if (prevInex >= CodeDocument.Length || CodeDocument.GetCharAt(prevInex) != 'n') return false;
                prevInex++;
                if (CodeDocument.GetCharAt(prevInex) != 'd') return false;
                return true;
            }

    }
}
