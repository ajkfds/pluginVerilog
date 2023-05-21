﻿using System;
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

            // trim start/end blank of macro text
            string text = macroText;
            text = text.TrimStart(new char[] { ' ', '\t' });
            text = text.TrimEnd(new char[] { ' ', '\t' });

            // seprarate identifier & argument
            
            if (name.Contains("(") && name.EndsWith(")"))
            {
                string argumentsText = name.Substring(name.IndexOf("(")+1);
                argumentsText = argumentsText.Substring(0, argumentsText.Length - 1);

                string[] arguments = argumentsText.Split(',');
                macro.Aurguments = new List<string>();
                foreach(string argument in arguments)
                {
                    macro.Aurguments.Add(argument.Trim());
                }
                text = text.Substring(text.IndexOf(")")+1).Trim();
                name = name.Substring(0,name.IndexOf("("));
            }

            macro.Name = name;
            macro.MacroText = macroText;
            return macro;
        }

        public string Name;
        public List<string> Aurguments = null;
        public string MacroText;

    }
}
