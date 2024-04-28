using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    static class Values
    {
        public static Token T_Error = new Token("ERROR", "ERROR");
        public static Token Empty = new Token("NONE", "NONE");
        public static Node N_Error = new Node(T_Error);
        public static Statement STEmpty = new Statement();
        public static Statement STEnd = new Statement();
        public static string DONE = "DONE";

        //String tokens
        public const string T_PLUS = "PLUS";
        public const string T_MINUS = "MINUS";
        public const string T_MULT = "MULTIPLY";
        public const string T_DIV = "DIV";
        public const string T_POW = "POW";
        public const string T_MOD = "MOD";
        public const string T_LPAR = "LPAR";
        public const string T_RPAR = "RPAR";
        public const string T_COMMA = "COMMA";
        public const string T_SEMICOLON = "SEMICOLON";
        public const string T_COLON = "COLON";
        public const string T_EQUAL = "EQUAL";
        public const string T_LESS = "LESS";
        public const string T_GREATER = "GREATER";
        public const string T_EQUALS = "EQUALS";
        public const string T_NOT_EQUALS = "NOTEQUALS";
        public const string T_GR_EQUAL = "GREATEREQUAL";
        public const string T_LS_EQUAL = "LESSEQUAL";
        public const string T_DOT = "DOT";
        public const string T_SQ_LPAR = "LSQPAR";
        public const string T_SQ_RPAR = "RSQPAR";
        public const string T_CR_LPAR = "LCRPAR";
        public const string T_CR_RPAR = "RCRPAR";
        public const string T_NOT = "NOT";
        public const string T_QUOTE = "QUOTE";

        public const string T_INT = "INT";
        public const string T_FLOAT = "FLOAT";
        public const string T_DOUBLE = "DOUBLE";
        public const string T_LONG = "LONG";
        public const string T_STRING = "STRING";
        public const string T_LIST = "LIST";
        public const string T_BOOL = "BOOL";
        public const string T_KW = "KEYWORD";
        public const string T_ID = "IDENTIFIER";
        public const string T_OP = "OPERATOR";

        //Operations dictionary
        public static Dictionary<string, string> Operators = new Dictionary<string, string>()
        {
            {"+", T_PLUS },
            {"-", T_MINUS },
            {"*", T_MULT },
            {"/", T_DIV },
            {"**", T_POW },
            {"%", T_MOD },
            {"(", T_LPAR },
            {")", T_RPAR },
            {",", T_COMMA },
            {";",T_SEMICOLON },
            {":",T_COLON },
            {"=",T_EQUAL },
            {"<",T_LESS },
            {">",T_GREATER },
            {"==",T_EQUALS},
            {"!=",T_NOT_EQUALS},
            {">=",T_GR_EQUAL },
            {"<=",T_LS_EQUAL },
            {".",T_DOT },
            {"[",T_SQ_LPAR },
            {"]",T_SQ_RPAR },
            {"{",T_CR_LPAR },
            {"}",T_CR_RPAR },
            {"!",T_NOT },
            {"\"",T_QUOTE }
        };

        public static List<string> KeywordsList = new List<string>()
        {
             //          //            //   //                                      //    //      //        //     //       //      //
            "if","else","while","for","fn","out","append","remove","find","return","int","float", "double","list","string","bool", "long"
             //          //            //   //                                      //    //      //        //     //       //      //
        };

        //Operations array
        public static List<string> OperationsList = new List<string>()
        { 
            "%","+", "-", "*", "/", "**", "(", ")", ",", ";", ":", "=", "<", ">", "==", "!=", ">=", "<=", ".", "[", "]", "{", "}", "!","\""
        };
    }
}
