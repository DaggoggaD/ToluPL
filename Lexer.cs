using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ToluPL
{
    internal class Lexer
    {
        public List<string> lines;
        public List<Token> tokens;
        public Lexer(List<string> Elines) { 
            lines = Elines;
            lines = Remove_tabs();
            tokens = Cycle_lines();
        }

        private List<string> Remove_tabs()
        {
            List<string> endlines = new List<string>();
            lines.ForEach(cline => {
                const string reduceMultiSpace = @"[ ]{2,}";
                var line = Regex.Replace(cline.Replace("\t", ""), reduceMultiSpace, "");
                endlines.Add(line);
            });
            return endlines;
        }
        
        private Token Find_token(string str) 
        {
            int couldint = 0;
            float couldfloat = 0;
            if (Values.KeywordsList.Contains(str)) {
                Token token = new Token(Values.T_KW, str);
                return token;
            }
            else if (Int32.TryParse(str, out couldint))
            {
                Token token = new Token(Values.T_INT, "NUMBER", couldint);
                return token;
            }
            else if (float.TryParse(str, out couldfloat))
            {
                Token token = new Token(Values.T_FLOAT, "NUMBER", couldfloat);
                return token;
            }
            else
            {
                Token token = new Token(Values.T_ID, str);
                return token;
            }
        }

        private List<Token> Analyze_tokens(string line) 
        {
            List<Token> tokens = new List<Token>();
            int index = 0;
            string currstr = "";
            while (index < line.Length)
            {
                string currchar = line[index].ToString();
                string nextchar;
                if (index + 1 < line.Length) nextchar = line[index + 1].ToString();
                else nextchar = "|";
                currstr += currchar;
                //Operators checker
                if (Values.OperationsList.Contains(currstr) && currstr[0].ToString() != "\"" && currstr!=" ")
                {
                    //One-char operators
                    if (Values.OperationsList.Contains(currstr+nextchar))
                    {
                        Token token = new Token(Values.T_OP, Values.Operators[currstr+nextchar]);
                        if (currstr != " ") tokens.Add(token);
                        index++;
                    }
                    else //multiple chars operators
                    {
                        Token token = new Token(Values.T_OP, Values.Operators[currstr]);
                        if (currstr != " ") tokens.Add(token);
                    }
                    currstr = "";
                    index++;
                }
                //Space separation
                else if((nextchar==" " || nextchar == "|") && currstr[0].ToString() != "\"") 
                {
                    Token token = Find_token(currstr);
                    if (currstr != " ") tokens.Add(token);
                    index +=2;
                    currstr = "";
                }
                //Operator skip
                else if (Values.OperationsList.Contains(nextchar) && currstr[0].ToString() != "\"")
                {
                    if(Int32.TryParse(currstr, out int temp) && nextchar.ToString()==".")
                    {
                        index++;
                        if (index + 1 < line.Length) nextchar = line[index + 1].ToString();
                        while (Int32.TryParse(nextchar, out int temp2))
                        {
                            if (index + 1 < line.Length) nextchar = line[index + 1].ToString();
                            currstr += line[index];
                            index++;
                        }
                        float res = 0;
                        float.TryParse(currstr, out res);
                        Token token = new Token(Values.T_FLOAT, "NUMBER", res);
                        tokens.Add(token);
                        currstr = "";
                    }
                    else
                    {
                        Token token = Find_token(currstr);
                        if(currstr!=" ") tokens.Add(token);
                        index++;
                        currstr = "";
                    }
                }
                else if (currstr[0].ToString()=="\"" && currchar.ToString() == "\"" && currstr.Length!=1)
                {
                    Token token = new Token(Values.T_STRING,currstr);
                    if (currstr != " ") tokens.Add(token);
                    index++;
                    currstr = "";
                }
                else
                {
                    index++;
                }
            }
            return tokens;
        }

        private List<Token> Cycle_lines()
        {
            List<Token> INTERNAL_tokens = new List<Token>();
            for (int i = 0; i<lines.Count; i++)
            {
                string currline = lines[i];
                List<Token> retTokens = Analyze_tokens(currline);
                retTokens.ForEach(x => INTERNAL_tokens.Add(x));
            }
            return INTERNAL_tokens;
        }
    }
}
