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
        public static Node N_Error = new Node(T_Error);

        //String tokens
        public static string T_PLUS = "PLUS";
        public static string T_MINUS = "MINUS";
        public static string T_MULT = "MULTIPLY";
        public static string T_DIV = "DIV";
        public static string T_POW = "POW";
        public static string T_LPAR = "LPAR";
        public static string T_RPAR = "RPAR";
        public static string T_COMMA = "COMMA";
        public static string T_SEMICOLON = "SEMICOLON";
        public static string T_COLON = "COLON";
        public static string T_EQUAL = "EQUAL";
        public static string T_LESS = "LESS";
        public static string T_GREATER = "GREATER";
        public static string T_EQUALS = "EQUALS";
        public static string T_NOT_EQUALS = "NOTEQUALS";
        public static string T_GR_EQUAL = "GREATEREQUAL";
        public static string T_LS_EQUAL = "LESSEQUAL";
        public static string T_DOT = "DOT";
        public static string T_SQ_LPAR = "LSQPAR";
        public static string T_SQ_RPAR = "RSQPAR";
        public static string T_CR_LPAR = "LCRPAR";
        public static string T_CR_RPAR = "RCRPAR";
        public static string T_NOT = "NOT";
        public static string T_QUOTE = "QUOTE";

        public static string T_INT = "INT";
        public static string T_FLOAT = "FLOAT";
        public static string T_STRING = "STRING";
        public static string T_LIST = "LIST";
        public static string T_BOOL = "BOOL";
        public static string T_KW = "KEYWORD";
        public static string T_ID = "IDENTIFIER";
        public static string T_OP = "OPERATOR";

        //Operations dictionary
        public static Dictionary<string, string> Operators = new Dictionary<string, string>()
        {
            {"+", T_PLUS },
            {"-", T_MINUS },
            {"*", T_MULT },
            {"/", T_DIV },
            {"**", T_POW },
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
            "if","else","while","for","function","out","append","remove","find","return","int","float","list","string","bool"
        };

        //Operations array
        public static List<string> OperationsList = new List<string>()
        { 
            "+", "-", "*", "/", "**", "(", ")", ",", ";", ":", "=", "<", ">", "==", "!=", ">=", "<=", ".", "[", "]", "{", "}", "!","\""
        };
    }
}
