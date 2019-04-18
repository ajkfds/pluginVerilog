using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class WordScanner : IDisposable
    {
        public WordScanner( codeEditor.CodeEditor.CodeDocument document, Verilog.ParsedDocument parsedDocument,bool systemVerilog)
        {
            RootParsedDocument = parsedDocument;
            wordPointer = new WordPointer(document, parsedDocument);
            this.systemVerilog = systemVerilog;
        }

        public static System.Diagnostics.Stopwatch sw0 = new System.Diagnostics.Stopwatch();
        public static System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
        public static System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
        public static System.Diagnostics.Stopwatch sw3 = new System.Diagnostics.Stopwatch();

        public void GetFirst()
        {
            recheckWord();
        }

        public void Dispose()
        {
            wordPointer.Dispose();
        }

        public Verilog.ParsedDocument RootParsedDocument { get; protected set; }


        private int nonGeneratedCount = 0;
        private bool prototype = false;

        public bool Prototype
        {
            set
            {
                prototype = value;
            }
            get
            {
                return prototype;
            }
        }

        private bool systemVerilog;
        public bool SystemVerilog
        {
            get { return systemVerilog; }
        }

        public void StartNonGenenerated()
        {
            nonGeneratedCount++;
        }

        public void EndNonGeneratred()
        {
            if (nonGeneratedCount == 0) return;
            nonGeneratedCount--;
        }

        protected WordPointer wordPointer = null;
        protected List<WordPointer> stock = new List<WordPointer>();

        public WordScanner Clone()
        {
            WordScanner ret = new WordScanner(wordPointer.Document, RootParsedDocument,systemVerilog);
            ret.wordPointer = wordPointer.Clone();
            ret.nonGeneratedCount = nonGeneratedCount;
            ret.prototype = prototype;
            foreach (var wp in stock)
            {
                ret.stock.Add(wp.Clone());
            }
            return ret;
        }

        public WordReference GetReference()
        {
            return new WordReference(wordPointer.Document, wordPointer.ParsedDocument, wordPointer.Index, wordPointer.Length);
        }

        public WordReference GetReference(WordReference fromReference)
        { 
            if(fromReference.Document == wordPointer.Document)
            {
                return new WordReference(wordPointer.Document, wordPointer.ParsedDocument, fromReference.Index, wordPointer.Index + wordPointer.Length - fromReference.Index);
            }
            else
            {
                return new WordReference(wordPointer.Document, wordPointer.ParsedDocument, fromReference.Index, fromReference.Document.Length - fromReference.Index);
            }
        }

        public void Color(CodeDrawStyle.ColorType colorType)
        {
            sw3.Start();
            if (nonGeneratedCount != 0 || prototype) return;
            wordPointer.Color(colorType);
            sw3.Stop();
        }

        public void AddError(string message)
        {
            sw2.Start();
            if (nonGeneratedCount != 0 || prototype) return;
            wordPointer.AddError(message);
            sw2.Stop();
        }

        public void AddWarning(string message)
        {
//            sw2.Start();
            if (nonGeneratedCount != 0 || prototype) return;
            wordPointer.AddWarning(message);
//            sw2.Stop();
        }

        public void AddPrototypeError(string message)
        {
//            sw2.Start();
            if (nonGeneratedCount != 0) return;
            wordPointer.AddError(message);
//            sw2.Stop();
        }

        public void AddPrototypeWarning(string message)
        {
//            sw2.Start();
            if (nonGeneratedCount != 0) return;
            wordPointer.AddWarning(message);
//            sw2.Stop();
        }

        public int RootIndex
        {
            get
            {
                if(stock.Count == 0)
                {
                    return wordPointer.Index;
                }
                else
                {
                    return stock[0].Index;
                }
            }
        }

        public bool Active
        {
            get
            {
                if (nonGeneratedCount != 0) return false;
                return true;
            }
        }

        public void MoveNext()
        {
            sw0.Start();
            if (nonGeneratedCount != 0)
            {
                wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
            }

            while(wordPointer.Eof && stock.Count != 0)
            {
                bool error = false;
                if (wordPointer.ParsedDocument.Messages.Count != 0) error = true;

                if(wordPointer.ParsedDocument == stock.Last().ParsedDocument)
                {
                    error = false;
                }

                wordPointer.Dispose();
                wordPointer = stock.Last();
                stock.Remove(stock.Last());
                if (error) wordPointer.AddError("include errors");
            }

            wordPointer.MoveNext();
            recheckWord();
            sw0.Stop();
        }

        private void recheckWord()
        {
            while (!wordPointer.Eof)
            {
                if (wordPointer.WordType == WordPointer.WordTypeEnum.Comment)
                {
                    wordPointer.MoveNext();
                }
                else if (wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                {
                    parseCompilerDirctive();
                }
                else
                {
                    break;
                }
            }
        }

        public bool Eof
        {
            get
            {
                if(stock.Count == 0) return wordPointer.Eof;
                for(int i= stock.Count - 1; i >= 0; i--)
                {
                    if (!stock[i].Eof) return false;
                }
                return true;
            }
        }

        public string Text
        {
            get
            {
                sw1.Start();
                string ret = wordPointer.Text;
                sw1.Stop();
                return ret;
            }
        }

        public WordPointer.WordTypeEnum WordType
        {
            get
            {
                return wordPointer.WordType;
            }
        }

        public string NextText
        {
            get
            {
                WordPointer temp = wordPointer.Clone();
                wordPointer.MoveNext();
                while (!wordPointer.Eof && wordPointer.WordType == WordPointer.WordTypeEnum.Comment)
                {
                    wordPointer.MoveNext();
                }
                string text = wordPointer.Text;
                wordPointer = temp;
                return text;
            }
        }

        public int Length
        {
            get
            {
                return wordPointer.Length;
            }
        }

        public char GetCharAt(int wordIndex)
        {
            return wordPointer.GetCharAt(wordIndex);
        }

        private enum ifdefEnum
        {
            ifdefActive,
            ifdefInActive,
            ElseActive,
            ElseInActive
        }
        private List<ifdefEnum> ifDefs = new List<ifdefEnum>();

        private void parseCompilerDirctive()
        {
            switch (wordPointer.Text)
            {
                case "`include":
                    parseInclude();
                    break;
                case "`define":
                    parseDefine();
                    break;
                case "`celldefine":
                case "`default_nettype":
                case "`endcelldefine":
                    wordPointer.AddError("not supported");
                    wordPointer.MoveNext();
                    break;
                case "`endif":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    if (ifDefs.Count != 0) ifDefs.Remove(ifDefs.Last());
                    break;
                case "`ifdef":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    wordPointer.Color(CodeDrawStyle.ColorType.Identifier);
                    if (RootParsedDocument.Macros.ContainsKey(wordPointer.Text))
                    {   // true
                        wordPointer.MoveNext();
                        if (wordPointer.Text == "`else")
                        {
                            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                            wordPointer.MoveNext();
                        }
                    }
                    else
                    {   // false
                        wordPointer.MoveNext();
                        skip();
                    }
                    break;
                case "`ifndef":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    wordPointer.Color(CodeDrawStyle.ColorType.Identifier);
                    if (!RootParsedDocument.Macros.ContainsKey(wordPointer.Text))
                    {   // true
                        wordPointer.MoveNext();
                        if (wordPointer.Text == "`else")
                        {
                            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                            wordPointer.MoveNext();
                        }
                    }
                    else
                    {   // false
                        wordPointer.MoveNext();
                        skip();
                    }
                    break;
                case "`else":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    skip();
                    break;
                case "`elsif":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNext();
                    wordPointer.Color(CodeDrawStyle.ColorType.Identifier);
                    if (RootParsedDocument.Macros.ContainsKey(wordPointer.Text))
                    {   // true
                        wordPointer.MoveNext();
                        if (wordPointer.Text == "`else")
                        {
                            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                            wordPointer.MoveNext();
                        }
                    }
                    else
                    {   // false
                        wordPointer.MoveNext();
                        skip();
                    }
                    break;
                case "`line":
                case "`nounconnected_drive":
                case "`resetall":
                case "`unconnected_drive":
                case "`undef":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.AddError("unsupported compiler directive");
                    wordPointer.MoveNext();
                    break;
                case "`timescale":
                    wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                    wordPointer.MoveNextUntilEol();
                    break;
                default: // macro call
                    parseMacro();
                    break;
            }
        }

        private void skip()
        {
            int depth = 0;
            while (!wordPointer.Eof)
            {
                if (wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                {
                    switch (wordPointer.Text)
                    {
                        case "`ifdef":
                            depth++;
                            wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                            wordPointer.MoveNext();
                            break;
                        case "`ifndef":
                            depth++;
                            wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                            wordPointer.MoveNext();
                            break;
                        case "`else":
                            if (depth == 0)
                            {
                                wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                                wordPointer.MoveNext();
                                return;
                            }
                            else
                            {
                                wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                                wordPointer.MoveNext();
                            }
                            break;
                        case "`endif":
                            if (depth == 0)
                            {
                                wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
                                wordPointer.MoveNext();
                                return;
                            }
                            else
                            {
                                depth--;
                                wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                                wordPointer.MoveNext();
                            }
                            break;
                        default:
                            wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                            wordPointer.MoveNext();
                            break;
                    }
                }
                else
                {
                    wordPointer.Color(CodeDrawStyle.ColorType.Inactivated);
                    wordPointer.MoveNext();
                }
            }
        }

        private void parseDefine()
        {
            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
            wordPointer.MoveNext();
            if (!General.IsIdentifier(wordPointer.Text))
            {
                wordPointer.AddError("iilegal identifier");
                return;
            }
            bool error = false;

            wordPointer.Color(CodeDrawStyle.ColorType.Identifier);
            string identifier = wordPointer.Text;
            if (RootParsedDocument.Macros.ContainsKey(identifier))
            {
                if(Active) wordPointer.AddError("dulplicated identifier");
                error = true;
            }

            wordPointer.MoveNextUntilEol();
            string macroText = wordPointer.Text;

            if (!error)
            {
                RootParsedDocument.Macros.Add(identifier, macroText);
            }

            wordPointer.MoveNext();
            recheckWord();
        }

        private void parseInclude()
        {
            wordPointer.Color(CodeDrawStyle.ColorType.Keyword);
            wordPointer.MoveNext();
            if(wordPointer.WordType != WordPointer.WordTypeEnum.String)
            {
                wordPointer.AddError("\" expected");
                return;
            }
            string filePath = wordPointer.Text;
            filePath = filePath.Substring(1, filePath.Length - 2);

            // search in same folder with original verilog file
            string fileID = wordPointer.ParsedDocument.ItemID;
            codeEditor.Data.File file = wordPointer.ParsedDocument.Project.GetRegisterdItem(fileID) as codeEditor.Data.File;

            if(file == null)
            {
                System.Diagnostics.Debugger.Break();
            }
            string sameFolderPath = file.RelativePath;
            if (sameFolderPath.Contains('\\'))
            {
                sameFolderPath = sameFolderPath.Substring(0, sameFolderPath.LastIndexOf('\\'));
                sameFolderPath = sameFolderPath +'\\'+ filePath;

                string id = codeEditor.Data.File.GetID(sameFolderPath, wordPointer.ParsedDocument.Project);
                if (wordPointer.ParsedDocument.Project.IsRegistered(id)){
                    diveIntoIncludeFile(sameFolderPath);
                    return;
                }
            }

            // search same filename in full project
            {
                List<string> ids = wordPointer.ParsedDocument.Project.GetRegisteredIdList();
                string id = ids.FirstOrDefault((x) =>
                {
                    return x.EndsWith(filePath);
                });

                if (id == null)
                {
                    wordPointer.AddError("file not found");
                    wordPointer.MoveNext();
                    return;
                }

                codeEditor.Data.Item item = wordPointer.ParsedDocument.Project.GetRegisterdItem(id);
                codeEditor.Data.File ffile = item as codeEditor.Data.File;
                if (ffile != null)
                {
                    diveIntoIncludeFile(ffile.RelativePath);
                    return;
                }
            }
            wordPointer.AddError("file not found");
            wordPointer.MoveNext();
            recheckWord();
            return;
        }

        private void parseMacro()
        {
            wordPointer.Color(CodeDrawStyle.ColorType.Identifier);
            string macroIdentifier = wordPointer.Text.Substring(1);

            if (!RootParsedDocument.Macros.ContainsKey(macroIdentifier))
            {
                wordPointer.AddError("unsupported macro call");
                wordPointer.MoveNext();
                return;
            }

            string macroText = RootParsedDocument.Macros[macroIdentifier];
//            wordPointer.MoveNext();

            codeEditor.CodeEditor.CodeDocument codeDocument = new codeEditor.CodeEditor.CodeDocument();
            codeDocument.Replace(0, 0, 0, macroText);

            WordPointer newPointer = new WordPointer(codeDocument, wordPointer.ParsedDocument);
            stock.Add(wordPointer);
            wordPointer = newPointer;
            while (!wordPointer.Eof)
            {
                if (wordPointer.WordType == WordPointer.WordTypeEnum.Comment)
                {
                    wordPointer.MoveNext();
                }
                else if (wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                {
                    parseCompilerDirctive();
                    break;
                }
                else
                {
                    break;
                }
            }
            return;
        }

        private void diveIntoIncludeFile(string relativeFilePath)
        {
            string id = wordPointer.ParsedDocument.ItemID+":include:"+relativeFilePath;
            codeEditor.Data.Item item;
            if (wordPointer.ParsedDocument.Project.IsRegistered(id))
            {
                item = wordPointer.ParsedDocument.Project.GetRegisterdItem(id);
            }
            else
            {
                item = Data.VerilogHeaderFile.CreateInstance(relativeFilePath, id, wordPointer.ParsedDocument.Project);
            }
            codeEditor.Data.ITextFile textFile = item as codeEditor.Data.ITextFile;
            if(textFile == null ||! (textFile is Data.VerilogHeaderFile))
            {
                wordPointer.AddError("illegal filetype");
                return;
            }

            Data.VerilogHeaderFile vhFile = textFile as Data.VerilogHeaderFile;
            if(!RootParsedDocument.IncludeFiles.ContainsKey(vhFile.ID))
            {
                RootParsedDocument.IncludeFiles.Add(vhFile.ID,vhFile);
            }
            vhFile.ParsedDocument = new codeEditor.CodeEditor.ParsedDocument(RootParsedDocument.Project, id, -1);

            WordPointer newPointer = new WordPointer(vhFile.CodeDocument, vhFile.ParsedDocument);
            stock.Add(wordPointer);
            wordPointer = newPointer;
            while (!wordPointer.Eof)
            {
                if (wordPointer.WordType == WordPointer.WordTypeEnum.Comment)
                {
                    wordPointer.MoveNext();
                }
                else if (wordPointer.WordType == WordPointer.WordTypeEnum.CompilerDirective)
                {
                    parseCompilerDirctive();
                    break;
                }
                else
                {
                    break;
                }
            }
        }

    }

}