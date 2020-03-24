using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class WordReference : IDisposable
    {
        public WordReference(int index, int length, codeEditor.CodeEditor.ParsedDocument parsedDocument)
        {
            Index = index;
            Length = length;
            ParsedDocument = parsedDocument;

        }
        public void Dispose()
        {
            ParsedDocument = null;
        }

        public WordReference CreateReferenceFrom(WordReference fromReference)
        {
            if (fromReference.ParsedDocument != ParsedDocument) return this;
            return new WordReference(fromReference.Index, Index + Length - fromReference.Index, ParsedDocument);
        }
        public int Index { get; protected set; }
        public int Length { get; protected set; }

        private System.WeakReference<codeEditor.CodeEditor.ParsedDocument> parsedDocumentRef;
        public codeEditor.CodeEditor.ParsedDocument ParsedDocument {
            get
            {
                codeEditor.CodeEditor.ParsedDocument ret;
                if (!parsedDocumentRef.TryGetTarget(out ret)) return null;
                return ret;
            }
            protected set
            {
                parsedDocumentRef = new WeakReference<codeEditor.CodeEditor.ParsedDocument>(value);
            }
        }
        public void AddError(codeEditor.CodeEditor.CodeDocument document, string message)
        {
            if (ParsedDocument == null) return;

            if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).ErrorCount < 100)
            {
                int lineNo = document.GetLineAt(Index);
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(document.TextFile as Data.IVerilogRelatedFile, message, Verilog.ParsedDocument.Message.MessageType.Warning, Index, lineNo, Length, ParsedDocument.Project));
            }
            else if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).ErrorCount == 100)
            {
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(document.TextFile as Data.IVerilogRelatedFile, ">100 errors", Verilog.ParsedDocument.Message.MessageType.Error, 0, 0, 0, ParsedDocument.Project)); ;
            }

            {
                for (int i = Index; i < Index + Length; i++)
                {
                    document.SetMarkAt(i, 0);
                }
            }
            if (ParsedDocument is Verilog.ParsedDocument) (ParsedDocument as Verilog.ParsedDocument).ErrorCount++;
        }
        public void AddWarning(codeEditor.CodeEditor.CodeDocument document, string message)
        {
            if (ParsedDocument == null) return;

            if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount < 100)
            {
                int lineNo = document.GetLineAt(Index);
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(document.TextFile as Data.IVerilogRelatedFile, message, Verilog.ParsedDocument.Message.MessageType.Warning, Index, lineNo, Length, ParsedDocument.Project));
            }
            else if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount == 100)
            {
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(document.TextFile as Data.IVerilogRelatedFile, ">100 warnings", Verilog.ParsedDocument.Message.MessageType.Warning, 0, 0, 0, ParsedDocument.Project));
            }

            for (int i = Index; i < Index + Length; i++)
            {
                document.SetMarkAt(i, 1);
            }
            if (ParsedDocument is Verilog.ParsedDocument) (ParsedDocument as Verilog.ParsedDocument).WarningCount++;
        }
        public void AddNotice(codeEditor.CodeEditor.CodeDocument document, string message)
        {
            if (ParsedDocument == null) return;

            if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount < 100)
            {
                int lineNo = document.GetLineAt(Index);
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(document.TextFile as Data.IVerilogRelatedFile, message, Verilog.ParsedDocument.Message.MessageType.Notice, Index, lineNo, Length, ParsedDocument.Project));
            }
            else if (ParsedDocument is Verilog.ParsedDocument && (ParsedDocument as Verilog.ParsedDocument).WarningCount == 100)
            {
                ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(document.TextFile as Data.IVerilogRelatedFile, ">100 notices", Verilog.ParsedDocument.Message.MessageType.Notice, 0, 0, 0, ParsedDocument.Project));
            }

            for (int i = Index; i < Index + Length; i++)
            {
                document.SetMarkAt(i, 2);
            }
            if (ParsedDocument is Verilog.ParsedDocument) (ParsedDocument as Verilog.ParsedDocument).WarningCount++;
        }

    }
}
