﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog.Verilog.ModuleItems
{
    public class GateInstantiation : Item
    {
        protected GateInstantiation() { }

        // gate_instantiation::=    cmos_switchtype                             [delay3]    cmos_switch_instance        { , cmos_switch_instance }; 
        //                          | enable_gatetype   [drive_strength]        [delay3]    enable_gate_instance        { , enable_gate_instance }; 
        //                          | mos_switchtype                            [delay3]    mos_switch_instance         { , mos_switch_instance }; 
        //                          | n_input_gatetype  [drive_strength]        [delay2]    n_input_gate_instance       {, n_input_gate_instance }; 
        //                          | n_output_gatetype [drive_strength]        [delay2]    n_output_gate_instance      { , n_output_gate_instance }; 
        //                          | pass_en_switchtype                        [delay2]    pass_enable_switch_instance {, pass_enable_switch_instance }; 
        //                          | pass_switchtype                                       pass_switch_instance        { , pass_switch_instance }; 
        //                          | pulldown          [pulldown_strength]                 pull_gate_instance          { , pull_gate_instance }; 
        //                          | pullup            [pullup_strength]                   pull_gate_instance          { , pull_gate_instance }; 

        // enable_gate_instance             ::= [name_of_gate_instance] (output_terminal, input_terminal, enable_terminal) 
        // mos_switch_instance              ::= [name_of_gate_instance] (output_terminal, input_terminal, enable_terminal) 
        // n_input_gate_instance            ::= [name_of_gate_instance] (output_terminal, input_terminal { , input_terminal } ) 
        // n_output_gate_instance           ::= [name_of_gate_instance] (output_terminal { , output_terminal } , input_terminal ) 
        // pass_switch_instance             ::= [name_of_gate_instance] (inout_terminal, inout_terminal) 
        // pass_enable_switch_instance      ::= [name_of_gate_instance] (inout_terminal, inout_terminal, enable_terminal)
        // pull_gate_instance               ::= [name_of_gate_instance] (output_terminal ) 
        // name_of_gate_instance            ::= gate_instance_identifier[range] 

        // pulldown_strength    ::= (strength0, strength1) | (strength1, strength0) | (strength0 ) 
        // pullup_strength      ::= (strength0, strength1) | (strength1, strength0) | (strength1 ) 
        // enable_terminal      ::= expression 
        // inout_terminal       ::= net_lvalue
        // input_terminal       ::= expression ncontrol_terminal ::= expression 
        // output_terminal      ::= net_lvalue pcontrol_terminal ::= expression 

        // cmos_switchtype      ::= cmos | rcmos 
        // enable_gatetype      ::= bufif0 | bufif1 | notif0 | notif1 
        // mos_switchtype       ::= nmos | pmos | rnmos | rpmos 
        // n_input_gatetype     ::= and | nand | or | nor | xor | xnor 
        // n_output_gatetype    ::= buf | not 
        // pass_en_switchtype   ::= tranif0 | tranif1 | rtranif1 | rtranif0 
        // pass_switchtype      ::= tran | rtran

        //        public string ModuleName { get; protected set; }

        //        private List<Verilog.Variables.Port> ports = new List<Variables.Port>();
        //        public IReadOnlyList<Verilog.Variables.Port> Ports { get { return ports; } }

        public static GateInstantiation ParseCreate(WordScanner word, Module module)
        {
            switch (word.Text)
            {
                // cmos_switchtype
                case "cmos":
                case "rcmos":
                    break;
                // enable_gatetype
                case "bufif0":
                case "bufif1":
                case "notif0":
                case "notif1":
                    break;
                // mos_switchtype
                case "nmos":
                case "pmos":
                case "rnmos":
                case "rpmos":
                    break;
                // n_input_gatetype
                case "and":
                case "nand":
                case "or":
                case "nor":
                case "xor":
                case "xnor":
                    NInputGate ngate = NInputGate.ParseCreate(word, module);
                    if(word.Text == ";")
                    {
                        word.MoveNext();
                    }
                    else
                    {
                        word.AddError("; expected");
                    }
                    return ngate;
                // n_output_gatetype
                case "buf":
                case "not":
                    break;
                // pass_en_switchtype
                case "tranif0":
                case "tranif1":
                case "rtranif0":
                case "rtranif1":
                    break;
                // pass_switchtype
                case "tran":
                case "rtran":
                    break;
                case "pullup":
                case "pulldown":
                    PullUpPullDown pull = PullUpPullDown.ParseCreate(word, module);
                    if (word.Text == ";")
                    {
                        word.MoveNext();
                    }
                    else
                    {
                        word.AddError("; expected");
                    }
                    return pull;
                default:
                    break;

            }
            word.AddError("not implemented");
            while(!word.Eof && word.Text != ";")
            {
                word.MoveNext();
            }
            return null;
        }

    }

    public class PullUpPullDown : GateInstantiation
    {
        protected PullUpPullDown() { }
        public bool PullUp = false;

        public static new PullUpPullDown ParseCreate(WordScanner word, Module module)
        {
            PullUpPullDown pull = new PullUpPullDown();
            if (word.Text == "pullup") pull.PullUp = true;
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            DriveStrength.ParseCreate(word, module);

            if(word.Text != "(")
            {
                word.AddError("( expected");
                return null;
            }
            word.MoveNext();
            Expressions.Expression exp = Expressions.Expression.ParseCreate(word, module);
            if (word.Text != ")")
            {
                word.AddError(") expected");
                return null;
            }
            word.MoveNext();

            return pull;
        }

    }

    public class CmosSwitchInstiation : GateInstantiation
    {
        protected CmosSwitchInstiation() { }

        Delay3 delay3;

        // gate_instantiation::=    cmos_switchtype                             [delay3]    cmos_switch_instance        { , cmos_switch_instance }; 
        public static new GateInstantiation ParseCreate(WordScanner word, Module module)
        {
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            CmosSwitchInstiation ret = new CmosSwitchInstiation();
            if (word.Text == "(")
            {
                ret.delay3 = Delay3.ParseCreate(word, module);
                if (ret.delay3 == null) return null;
            }
            // cmos_switch_instance             ::= [name_of_gate_instance] (output_terminal, input_terminal, ncontrol_terminal, pcontrol_terminal) 

            return null;
        }
    }

    public class NInputGate : GateInstantiation
    {
        protected NInputGate() { }

        Delay2 Delay2;
        DriveStrength DriveStrength;

        //  n_input_gatetype  [drive_strength]        [delay2]    n_input_gate_instance       {, n_input_gate_instance }; 
        public static new NInputGate ParseCreate(WordScanner word, Module module)
        {
            word.Color(CodeDrawStyle.ColorType.Keyword);
            word.MoveNext();

            NInputGate ret = new NInputGate();

            ret.DriveStrength = DriveStrength.ParseCreate(word, module);
            ret.Delay2 = Delay2.ParseCreate(word, module);

            while (!word.Eof)
            {
                if (General.IsIdentifier(word.Text))
                {
                    word.Color(CodeDrawStyle.ColorType.Identifier);
                    word.MoveNext();
                }
                if (word.Text != "(") return null;
                word.MoveNext();

                while (!word.Eof)
                {
                    Expressions.Expression expression = Expressions.Expression.ParseCreate(word, module);
                    if(word.Text != ",")
                    {
                        break;
                    }
                    word.MoveNext();
                }
                if (word.Text != ")") return null;
                word.MoveNext();
            }

            // n_input_gate_instance            ::= [name_of_gate_instance] (output_terminal, input_terminal { , input_terminal } ) 

            return ret;
        }
    }

}