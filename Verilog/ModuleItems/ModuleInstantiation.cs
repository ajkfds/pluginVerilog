using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.ModuleItems
{
    public class ModuleInstantiation : Item
    {
        protected ModuleInstantiation() { }

        public string ModuleName{ get; protected set; }

        private List<Verilog.Variables.Port> ports = new List<Variables.Port>();
        public IReadOnlyList<Verilog.Variables.Port> Ports { get { return ports; } }

        public static void Parse(WordScanner word,Module module)
        {
            ModuleInstantiation moduleInstantiation = new ModuleInstantiation();
            bool error = false;

            WordScanner moduleIdentifier = word.Clone();
            moduleInstantiation.ModuleName = word.Text;
            word.MoveNext();

            string next = word.NextText;
            if(word.Text != "#" && next != "(" && next != ";" && General.IsIdentifier(word.Text))
            {
                moduleIdentifier.AddError("illegal module item");
                return;
            }

            moduleIdentifier.Color((byte)Style.Color.Identifier);

            if(word.Text == "#")
            {
                word.Color((byte)Style.Color.Keyword);
                word.MoveNext();

                if (word.Text != "(")
                {
                    word.AddError("( expected");
                    return;
                }
                word.MoveNext();

                if (word.Text == ".")
                { // named parameter assignment
                    while (!word.Eof && word.Text == ".")
                    {
                        word.MoveNext();
                        word.Color((byte)Style.Color.Paramater);
                        word.MoveNext();
                        if (word.Text != "(")
                        {
                            word.AddError("( expected");
                        }
                        else
                        {
                            word.MoveNext();
                        }
                        Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module);
                        if (word.Text != ")")
                        {
                            word.AddError(") expected");
                        }
                        else
                        {
                            word.MoveNext();
                        }
                        if (word.Text != ",")
                        {
                            break;
                        }
                        else
                        {
                            word.MoveNext();
                        }
                    }
                }
                else
                { // ordered paramater assignment
                    while (!word.Eof && word.Text != ")")
                    {
                        Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module);
                        if (word.Text != ",")
                        {
                            break;
                        }
                        else
                        {
                            word.MoveNext();
                        }
                    }
                }

                if (word.Text != ")")
                {
                    word.AddError("( expected");
                    return;
                }
                word.MoveNext();
            }

            word.Color((byte)Style.Color.Identifier);
            if (!General.IsIdentifier(word.Text))
            {
                word.AddError("illegal instance name");
                error = true;
            } else if (module.ModuleInstantiations.ContainsKey(word.Text))
            {
                word.AddError("instance name duplicated");
                error = true;
            }
            else
            {
                moduleInstantiation.Name = word.Text;
            }

            word.MoveNext();

            if (word.Text != "(")
            {
                word.AddError("( expected");
                return;
            }
            word.MoveNext();

            if (word.GetCharAt(0) == '.')
            { // named parameter assignment
                while (!word.Eof && word.Text == ".")
                {
                    word.MoveNext();
                    word.Color((byte)Style.Color.Identifier);
                    word.MoveNext();
                    if (word.Text != "(")
                    {
                        word.AddError("( expected");
                    }
                    else
                    {
                        word.MoveNext();
                    }
                    Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module);
                    if (word.Text != ")")
                    {
                        word.AddError(") expected");
                    }
                    else
                    {
                        word.MoveNext();
                    }
                    if (word.Text != ",")
                    {
                        break;
                    }
                    else
                    {
                        word.MoveNext();
                    }
                }
            }
            else
            { // ordered paramater assignment
                while (!word.Eof && word.Text != ")")
                {
                    Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module);
                    if (word.Text != ",")
                    {
                        break;
                    }
                    else
                    {
                        word.MoveNext();
                    }
                }
            }
            if (word.Text != ")")
            {
                word.AddError(") expected");
                return;
            }
            word.MoveNext();
            if (word.Text != ";")
            {
                word.AddError("; expected");
                return;
            }
            word.MoveNext();

            if (!error)
            {
                module.ModuleInstantiations.Add(moduleInstantiation.Name, moduleInstantiation);
            }
        }

        /*
        module_instantiation            ::= module_identifier [ parameter_value_assignment ] module_instance { , module_instance } ;
        parameter_value_assignment      ::= # ( list_of_parameter_assignments )  
        list_of_parameter_assignments   ::= ordered_parameter_assignment { , ordered_parameter_assignment } | named_parameter_assignment { , named_parameter_assignment }  
        ordered_parameter_assignment    ::= expression  named_parameter_assignment ::= .parameter_identifier ( [ expression ] ) 
        module_instance                 ::= name_of_instance ( [ list_of_port_connections ] )
        name_of_instance                ::= module_instance_identifier [ range ]  
        list_of_port_connections        ::= ordered_port_connection { , ordered_port_connection }          | named_port_connection { , named_port_connection }  
        ordered_port_connection         ::= { attribute_instance } [ expression ]
        named_port_connection           ::= { attribute_instance } .port_identifier ( [ expression ] )  
         */
    }
}
