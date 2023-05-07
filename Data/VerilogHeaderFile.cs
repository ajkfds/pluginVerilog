﻿using System;
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
        // include
        //public VerilogHeaderFile (string relativePath, IVerilogRelatedFile rootFile, codeEditor.Data.Project project, string id)
        //{
        //    Project = project;
        //    RelativePath = relativePath;
        //    if (relativePath.Contains('\\'))
        //    {
        //        Name = relativePath.Substring(relativePath.LastIndexOf('\\') + 1);
        //    }
        //    else
        //    {
        //        Name = relativePath;
        //    }
        //    this.id = id;
        //    this.RootFile = rootFile;
        //}

        //public IVerilogRelatedFile RootFile  { get; protected set; }

        private string id;
        public override string ID
        {
            get
            {
                return id;
            }
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
                        while (!readFromFile())
                        {
                            System.Threading.Thread.Sleep(10);
                        }
                    }
                    catch(Exception ex)
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

        private bool readFromFile()
        {
            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(Project.GetAbsolutePath(RelativePath)))
                {
                    document = new CodeEditor.CodeDocument(this);
                    string text = sr.ReadToEnd();
                    document.Replace(0, 0, 0, text);
                    document.ClearHistory();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147024864) // used by another process
                {
                    return false;
                }
                else
                {
                    throw ex;
                }
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
                return Project.ProjectProperties[Plugin.StaticID] as ProjectProperty;
            }
        }


        public override ajkControls.CodeTextbox.CodeDrawStyle DrawStyle
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

        public override void AcceptParsedDocument(codeEditor.CodeEditor.ParsedDocument newParsedDocument)
        {
            Data.IVerilogRelatedFile parentFile = Parent as Data.IVerilogRelatedFile;
            if (parentFile == null) return;

            parentFile.AcceptParsedDocument(newParsedDocument);

            Update();
        }

        protected override codeEditor.NavigatePanel.NavigatePanelNode createNode()
        {
            return new NavigatePanel.VerilogHeaderNode(this);
        }

        public override codeEditor.CodeEditor.DocumentParser CreateDocumentParser(codeEditor.CodeEditor.DocumentParser.ParseModeEnum parseMode)
        {
            Data.IVerilogRelatedFile parentFile = Parent as Data.IVerilogRelatedFile;
            if (parentFile == null) return null;
            // do not parse again for background parse. header file is parsed with parent file.
            if (parseMode != DocumentParser.ParseModeEnum.EditParse ) return null;

            // Use Parent File Parser for Edit Parse
            return parentFile.CreateDocumentParser(parseMode);
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

        public override List<codeEditor.CodeEditor.PopupItem> GetPopupItems(ulong version, int index)
        {
            return null;
            ////if (VerilogParsedDocument == null) return null;
            ////if (VerilogParsedDocument.Version != version) return null;

            ////int headIndex, length;
            ////CodeDocument.GetWord(index, out headIndex, out length);
            ////string text = CodeDocument.CreateString(headIndex, length);
            ////if (headIndex != 0 && CodeDocument.GetCharAt(headIndex - 1) == '.')
            ////{
            ////    text = "." + text;
            ////}
            ////List<codeEditor.CodeEditor.PopupItem> popups = VerilogParsedDocument.GetPopupItems(index, text);
            ////if (popups.Count != 0) return popups;

            ////if (RootFile == null || RootFile.VerilogParsedDocument == null) return null;
            ////return RootFile.VerilogParsedDocument.GetPopupItems(index, text);
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
