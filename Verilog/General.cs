﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pluginVerilog
{

    public class General
    {


        // A.9.4 Identifier branches


        // hierarchical_identifier          ::= simple_hierarchical_identifier
        //                                      | escaped_hierarchical_identifier  
        // simple_hierarchical_identifier   ::= simple_hierarchical_branch [ .escaped_identifier ]
        // simple_hierarchical_branch       ::= simple_identifier[[unsigned_number]]
        //                                      [{.simple_identifier[[unsigned_number]]}]
        // escaped_hierarchical_branch      ::= escaped_identifier[[unsigned_number]]
        //                                      [{.escaped_identifier[[unsigned_number]]}] 

        // * The period in escaped_hierarchical_identifier and escaped_hierarchical_branch 
        // shall be preceded by white_space, but shall not be followed by white_space.

        // Identifier /////////////////////////////////////////////////////////

        public static bool IsHierarchicalIdentifier(string value,out string[] hierarchy)
        {
            if (!value.Contains('.')) { hierarchy = new string[]{ value }; return true; }
            string[] hiers = value.Split('.');
            foreach(string identifier in hiers)
            {
                if (!IsIdentifier(identifier)) { hierarchy = null; return false; }
            }

            hierarchy = hiers;
            return true;
        }


        public static bool IsIdentifier(string value)
        {
            // identifier::= simple_identifier | escaped_identifier
            if (value.Length < 1) return false;
            if (value[0] == '\\') return true;  // escaped identifier

            // simple identifier
            int index = 0;
            if (value[index] >= 128) return false;
            if (identifierTable[value[index]] != 1) return false;
            index++;

            while (index <value.Length)
            {
                if (value[index] >= 128) return false;
                if (identifierTable[value[index]] == 0) return false;
                index++;
            }
            return true;
        }

        public static bool IsEscapedIdentifier(string value)
        {
            // escaped_identifier ::= \{Any_ASCII_character_except_white_space} white_space 
            if (value.Length <= 2) return false;
            if (value[0] !='\\') return false;
            return true;
        }

        public static bool IsSimpleIdentifier(string value)
        {
            // simple_identifier::= [a-zA-Z_]{[a-zA-Z0-9_$]}
            if (value.Length < 1) return false;

            int index = 0;
            if (value[index] >= 128) return false;
            if (identifierTable[value[index]] != 1) return false;
            index++;

            while (index < value.Length)
            {
                if (value[index] >= 128) return false;
                if (identifierTable[value[index]] == 0) return false;
                index++;
            }
            return true;
        }

        public static bool IsSpecparamIdentifier(string value)
        {
            // specparam_identifier::= identifier
            return IsSimpleIdentifier(value);
        }

        public static bool IsSystemFunctionIdentifier(string value)
        {
            // system_function_identifier::= $[a-zA-Z0-9_$]{[a-zA-Z0-9_$]}
            if (value.Length <= 2) return false;

            int index = 0;
            if (value[index] != '$') return false;
            index++;

            while (index < value.Length)
            {
                if (value[index] >= 128) return false;
                if (identifierTable[value[index]] == 0) return false;
                index++;
            }
            return true;
        }

        public bool IsSystemTaskIdentifier(string value)
        {
            // system_task_identifier ::= $[a-zA-Z0-9_$]{[a-zA-Z0-9_$]}
            return IsSystemFunctionIdentifier(value);
        }


        // identifier classify table ///////////////////////////////////////////
        // a-zA-Z_  : 1
        // 0-9      : 2
        // $        : 3

        public static byte[] identifierTable = new byte[128] {
            //      0,1,2,3,4,5,6,7,8,9,a,b,c,e,d,f
            // 0*
                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            // 1*
                    0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            // 2*     ! " # $ % & ' ( ) * + , - . /
                    0,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,
            // 3*   0 1 2 3 4 5 6 7 8 9 : ; < = > ?
                    2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,0,
            // 4*   @ A B C D E F G H I J K L M N O
                    0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            // 5*   P Q R S T U V W X Y Z [ \ ] ^ _
                    1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,1,
            // 6*   ` a b c d e f g h i j k l m n o
                    0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            // 7*   p q r s t u v w x y z { | } ~ 
                    1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,
        };

    }
}
