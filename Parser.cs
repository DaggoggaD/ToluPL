using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace ToluPL
{
    internal class Parser
    {
        public List<Token> tokens;
        public Token currtoken;
        public int tokID = -1;
        public List<Statement> statements;

        public Parser(List<Token> TOKENS) {
            tokens = TOKENS;
            Advance();
            statements = Cycle();
            statements.ForEach(statement => {Console.WriteLine(statement.REPR());});
        }

        public void Advance()
        {
            tokID += 1;
            if (tokID < tokens.Count) {
                currtoken = tokens[tokID];
            }
        }

        public Statement Factor()
        {
            Token tok = currtoken;
            List<string> COND = new List<string>() { Values.T_INT, Values.T_FLOAT, Values.T_ID, Values.T_STRING, Values.T_LIST};
            if (COND.Contains(tok.TType)) {
                Advance();
                Node node = new Node(tok);
                return node;
            }
            else if (currtoken.TValue == Values.T_LPAR)
            {
                Advance();
                Statement expr = Expr();
                if (currtoken.TValue == Values.T_RPAR)
                {
                    Advance();
                    return expr;
                }
                Node error = new Node(Values.T_Error);
                return error;
            }
            else
            {
                Console.WriteLine("ERROR: wrong factor line");
                return null;
            }
        }

        public Statement Term()
        {
            Statement left = Factor();
            List<string> COND = new List<string>() {Values.T_MULT, Values.T_DIV};
            while (COND.Contains(currtoken.TValue))
            {
                Token OPtoken = currtoken;
                Advance();
                Statement right = Factor();
                BinaryOP BOP = new BinaryOP(left, OPtoken, right);
                left = BOP;
            }
            return left;
        }

        public Statement AssignFunc(string VARTYPE) {
            Advance();
            if (currtoken.TType != Values.T_ID) return Values.N_Error;
            Token Varname = currtoken;
            Advance();
            if (currtoken.TValue != Values.T_EQUAL) return Values.N_Error;
            Advance();
            currtoken.REPR();
            Statement expr = Expr();
            AssignStatement ASTATEMENT = new AssignStatement(Varname, expr, VARTYPE);
            return ASTATEMENT;
        }

        public Statement Expr() {

            switch (currtoken.TValue)
            {
                case "int":
                    return AssignFunc("int");
                case "float":
                    return AssignFunc("float");
                case "string":
                    return AssignFunc("string");
                case "list":
                    return AssignFunc("list");
                case "bool":
                    return AssignFunc("bool");
                default:
                    break;
            }

            List<string> COND = new List<string>() {Values.T_PLUS, Values.T_MINUS, Values.T_MULT, Values.T_DIV, Values.T_LESS, Values.T_GREATER, Values.T_LS_EQUAL, Values.T_GR_EQUAL, Values.T_EQUAL, Values.T_NOT_EQUALS};
            Statement left = Term();
            while (COND.Contains(currtoken.TValue))
            {
                
                Token OP = currtoken;
                Advance();
                Statement right = Term();
                BinaryOP BOP = new BinaryOP(left,OP,right);
                left = BOP;
            }
            return left;
        }

        public List<Statement> Cycle()
        {
            List<Statement> result = new List<Statement>();
            while (tokID < tokens.Count)
            {
                Statement res = Expr();
                result.Add(res);
                Advance();
            }
            return result;
        }
    }
}
