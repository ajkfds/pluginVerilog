using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog
{
    public class Macro
    {
        //        text_macro_definition::= ‘define text_macro_name macro_text 
        // text_macro_name ::= text_macro_identifier[(list_of_formal_arguments)] 
        // list_of_formal_arguments ::= formal_argument_identifier { ,  formal_argument_identifier }
        //        text_macro_identifier::= (From Annex A - A.9.3) simple_identifier
        protected Macro() { }

        public static Macro Create(string name,string macroText)
        {
            Macro macro = new Macro();
            macro.Name = name;

            string text = macroText;
            text = text.TrimStart(new char[] { ' ', '\t' });
            text = text.TrimEnd(new char[] { ' ', '\t' });
            if (text.StartsWith("(") && text.Contains(")"))
            {
                string argumentsText = text.Substring(1, text.IndexOf(")")-1);
                string[] arguments = argumentsText.Split(',');
                macro.Aurguments = new List<string>();
                foreach(string argument in arguments)
                {
                    macro.Aurguments.Add(argument.Trim());
                }
                text = text.Substring(text.IndexOf(")")+1).Trim();
            }

            macro.MacroText = text;
            return macro;
        }

        public string Name;
        public List<string> Aurguments = null;
        public string MacroText;

    }
}
