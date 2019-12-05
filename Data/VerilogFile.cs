using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using codeEditor.CodeEditor;
using codeEditor.Data;

namespace pluginVerilog.Data
{
    public class VerilogFile : codeEditor.Data.TextFile, IVerilogRelatedFile
    {
        public new static VerilogFile Create(string relativePath, codeEditor.Data.Project project)
        {
            VerilogFile fileItem = new VerilogFile();
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
            fileItem.ParseRequested = true;

            return fileItem;
        }

        private CodeEditor.CodeDocument document = null;
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
                if (value != null &&  value as CodeEditor.CodeDocument == null) System.Diagnostics.Debugger.Break();
                document = value as CodeEditor.CodeDocument ;
            }
        }

        public override void AcceptParsedDocument(ParsedDocument newParsedDocument)
        {
            ParsedDocument oldParsedDocument = ParsedDocument;
            ParsedDocument = null;
            if (oldParsedDocument != null) oldParsedDocument.Dispose();

            ParsedDocument = newParsedDocument;

            foreach (Verilog.Module module in VerilogParsedDocument.Modules.Values)
            {
                if (!ProjectProperty.IsRegisterableModule(module.Name, this))
                {
                    if (module.NameReference != null) module.NameReference.AddError(CodeDocument, "duplicated module name");
                    continue;
                }

                bool suceed = ProjectProperty.RegisterModule(module.Name, this);
                if (!suceed)
                {
                    System.Diagnostics.Debugger.Break();
                    // add module name error
                }
            }

            ParseRequested = false;
            Update();
        }

        private Dictionary<string, System.WeakReference<ParsedDocument>> instancedParsedDocumentRefs = new Dictionary<string, WeakReference<ParsedDocument>>();
//        private List<System.WeakReference<Data.VerilogModuleInstance>> verilogModuleInstanceRefs = new List<WeakReference<VerilogModuleInstance>>();


        //public void RegisterModuleInstance(Data.VerilogModuleInstance verilogModuleInstance)
        //{
        //    // clean weak reference

        //    List<System.WeakReference<Data.VerilogModuleInstance>> removes = new List<System.WeakReference<Data.VerilogModuleInstance>>();
        //    Data.VerilogModuleInstance ret;
        //    lock (verilogModuleInstanceRefs)
        //    {
        //        foreach (System.WeakReference<Data.VerilogModuleInstance> r in verilogModuleInstanceRefs)
        //        {
        //            if (!r.TryGetTarget(out ret)) removes.Add(r);
        //        }
        //        foreach (System.WeakReference<Data.VerilogModuleInstance> key in removes)
        //        {
        //            verilogModuleInstanceRefs.Remove(key);
        //        }
        //    }

        //    // 



        //}

        public ParsedDocument GetInstancedParsedDocument(string parameterId)
        {
            cleanWeakRef();

            ParsedDocument ret;
            if (instancedParsedDocumentRefs.ContainsKey(parameterId))
            {
                if (instancedParsedDocumentRefs[parameterId].TryGetTarget(out ret))
                {
                    return ret;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void RegisterInstanceParsedDocument(string parameterId, ParsedDocument parsedDocument)
        {
            cleanWeakRef();
            if (instancedParsedDocumentRefs.ContainsKey(parameterId))
            {
                instancedParsedDocumentRefs[parameterId] = new WeakReference<ParsedDocument>(parsedDocument);
            }
            else
            {
                instancedParsedDocumentRefs.Add(parameterId,new WeakReference<ParsedDocument>(parsedDocument));
            }
        }

        private void cleanWeakRef()
        {
            List<string> removeKeys = new List<string>();
            ParsedDocument ret;
            lock (instancedParsedDocumentRefs)
            {
                foreach(var r in instancedParsedDocumentRefs)
                {
                    if (!r.Value.TryGetTarget(out ret)) removeKeys.Add(r.Key);
                }
                foreach(string key in removeKeys)
                {
                    instancedParsedDocumentRefs.Remove(key);
                }
            }
        }

        private List<System.WeakReference<Data.VerilogModuleInstance>> moduleInstanceRefs
            = new List<WeakReference<VerilogModuleInstance>>();

        public void RegisterModuleInstance(VerilogModuleInstance verilogModuleInstance)
        {
            moduleInstanceRefs.Add(new WeakReference<VerilogModuleInstance>(verilogModuleInstance));
        }

        public void RemoveModuleInstance(VerilogModuleInstance verilogModuleInstance)
        {
            for(int i = 0; i< moduleInstanceRefs.Count; i++)
            {
                VerilogModuleInstance ret;
                if (!moduleInstanceRefs[i].TryGetTarget(out ret)) continue;
                if (ret == verilogModuleInstance) moduleInstanceRefs.Remove(moduleInstanceRefs[i]);
            }
        }


        public override void Dispose()
        {
            if(ParsedDocument != null)
            {
                foreach(var incFile in VerilogParsedDocument.IncludeFiles.Values)
                {
                    incFile.Dispose();
                }
            }
            moduleInstanceRefs.Clear();
            base.Dispose();
        }


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

        public override ajkControls.CodeDrawStyle DrawStyle
        {
            get
            {
                return Global.CodeDrawStyle;
            }
        }

        public override codeEditor.NavigatePanel.NavigatePanelNode CreateNode()
        {
            NavigatePanel.VerilogFileNode node = new NavigatePanel.VerilogFileNode(this);
            nodeRef = new WeakReference<codeEditor.NavigatePanel.NavigatePanelNode>(node);
            return node;
        }

        public override void Update()
        {
            if(VerilogParsedDocument == null)
            {
                // dispose all
                foreach (Item item in items.Values) item.Dispose();
                items.Clear();
                return;
            }

            List<Item> currentItems = new List<Item>();
            Dictionary<string, Item> newItems = new Dictionary<string, Item>();

            foreach (Item item in VerilogParsedDocument.IncludeFiles.Values)
            {
                currentItems.Add(item);
            }

            foreach (Verilog.Module module in VerilogParsedDocument.Modules.Values)
            {
                foreach (Verilog.ModuleItems.ModuleInstantiation moduleInstantiation in module.ModuleInstantiations.Values)
                {
                    if (items.Keys.Contains(moduleInstantiation.Name))
                    {
                        currentItems.Add(items[moduleInstantiation.Name]);
                    }
                    else
                    {
                        Item item = Data.VerilogModuleInstance.Create(moduleInstantiation, Project);
                        if (item != null & !newItems.ContainsKey(moduleInstantiation.Name))
                        {
                            item.Parent = this;
                            newItems.Add(moduleInstantiation.Name, item);
                        }
                    }
                }
            }

            List<Item> removeItems = new List<Item>();
            foreach (codeEditor.Data.Item item in items.Values)
            {
                if (!currentItems.Contains(item)) removeItems.Add(item);
            }

            foreach (Item item in removeItems)
            {
                    items.Remove(item.Name);
                    item.Dispose();
            }

            foreach (Item item in newItems.Values)
            {
                item.Parent = this;
                items.Add(item.Name, item);
//                Project.AddReparseTarget(item);
            }
        }

        public override void AfterKeyDown(System.Windows.Forms.KeyEventArgs e)
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

        public override void AfterKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (VerilogParsedDocument == null) return;
        }

        public override void BeforeKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        {
        }

        public override void BeforeKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
        }

        public override List<codeEditor.CodeEditor.PopupItem> GetPopupItems(int editId, int index)
        {
            if (VerilogParsedDocument == null) return null;
            if (VerilogParsedDocument.EditID != editId) return null;

            int headIndex, length;
            CodeDocument.GetWord(index, out headIndex, out length);
            string text = CodeDocument.CreateString(headIndex, length);
            if(headIndex != 0 && CodeDocument.GetCharAt(headIndex-1) == '.')
            {
                text = "." + text;
            }
            return VerilogParsedDocument.GetPopupItems(index,text);
        }


        public override List<codeEditor.CodeEditor.ToolItem> GetToolItems(int index)
        {
            List<codeEditor.CodeEditor.ToolItem> toolItems = new List<codeEditor.CodeEditor.ToolItem>();
            toolItems.Add(new Verilog.Snippets.AlwaysFFSnippet());
            toolItems.Add(new Verilog.Snippets.ConnectionCheckSnippet());
            toolItems.Add(new Verilog.Snippets.AutoFormatSnippet());
            return toolItems;
        }

        public override List<codeEditor.CodeEditor.AutocompleteItem> GetAutoCompleteItems(int index,out string cantidateWord)
        {
            cantidateWord = null;

            if (VerilogParsedDocument == null) return null;
            int line = CodeDocument.GetLineAt(index);
            int lineStartIndex = CodeDocument.GetLineStartIndex(line);

            List<codeEditor.CodeEditor.AutocompleteItem> items = VerilogParsedDocument.GetAutoCompleteItems(index,lineStartIndex,line,(CodeEditor.CodeDocument)CodeDocument,out cantidateWord);
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


            bool prevBegin =  isPrevBegin(lineHeadIndex);
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
                    CodeDocument.Replace(lineHeadIndex, indentLength, 0, new String('\t', prevTabs+1)+"\r\n"+new String('\t', prevTabs));
                    CodeDocument.CaretIndex = CodeDocument.CaretIndex + prevTabs+1+1 - indentLength;
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

        public override DocumentParser CreateDocumentParser(DocumentParser.ParseModeEnum parseMode)
        {
            return new Parser.VerilogParser(this, parseMode);
        }
    }
}
