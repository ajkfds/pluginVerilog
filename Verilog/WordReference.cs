using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class WordReference
    {
        public WordReference(int index, int length)
        {
            Index = index;
            Length = length;
        }

        //public codeEditor.CodeEditor.CodeDocument Document { get; protected set; }
        //public codeEditor.CodeEditor.ParsedDocument ParsedDocument { get; protected set; }
        public int Index { get; protected set; }
        public int Length { get; protected set; }

        //public void AddError(string message)
        //{
        //    if (ParsedDocument == null) return;
        //    int lineNo = Document.GetLineAt(Index);

        //    ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Error, Index, lineNo, Length, ParsedDocument.ItemID, ParsedDocument.Project));
        //    for (int i = Index; i < Index + Length; i++)
        //    {
        //        Document.SetMarkAt(i, 0);
        //    }
        //}

        //public void AddWarning(string message)
        //{
        //    if (ParsedDocument == null) return;
        //    int lineNo = Document.GetLineAt(Index);

        //    ParsedDocument.Messages.Add(new Verilog.ParsedDocument.Message(message, Verilog.ParsedDocument.Message.MessageType.Warning, Index, lineNo, Length, ParsedDocument.ItemID, ParsedDocument.Project));
        //    for (int i = Index; i < Index + Length; i++)
        //    {
        //        Document.SetMarkAt(i, 1);
        //    }
        //}
    }
}
