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
            List<string> COND = new List<string>() { Values.T_INT, Values.T_FLOAT, Values.T_ID, Values.T_STRING, Values.T_LIST, Values.T_BOOL};
            if (COND.Contains(tok.TType))
            {
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
            List<string> COND = new List<string>() {Values.T_MULT, Values.T_DIV, Values.T_POW, Values.T_MOD};
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
            AssignStatement ASTATEMENT;
            Advance();
            if (currtoken.TType != Values.T_ID) return Values.N_Error;
            Token Varname = currtoken;
            Advance();
            if (currtoken.TValue == Values.T_SEMICOLON) ASTATEMENT = new AssignStatement(Varname, Values.STEmpty, VARTYPE);
            else
            {
                if (currtoken.TValue != Values.T_EQUAL) return Values.N_Error;
                Advance();
                Statement expr = Expr();
                ASTATEMENT = new AssignStatement(Varname, expr, VARTYPE);
            }
            return ASTATEMENT;
        }

        public Statement CheckRunStatements(string caseName)
        {
            Advance();
            if (currtoken.TValue != Values.T_LPAR) return Values.N_Error;
            Advance();
            Statement checkExpr = Expr();
            if (currtoken.TValue != Values.T_RPAR) return Values.N_Error;
            Advance();
            if (currtoken.TValue != Values.T_CR_LPAR) return Values.N_Error;
            List<Statement> insideStatements = new List<Statement>();
            Advance();
            while (currtoken.TValue != Values.T_CR_RPAR)
            {
                Statement retstatemnt = Expr();
                Advance();
                insideStatements.Add(retstatemnt);
            }
            Statement retStat;
            if(caseName=="if") retStat = new IfStatement(checkExpr, insideStatements);
            else retStat = new WhileStatement(checkExpr, insideStatements);
            return retStat;
        }

        public Statement Funcdecl()
        {
            Advance();
            List<string> COND = new List<string>() {Values.T_ID, Values.T_KW};
            if(!COND.Contains(currtoken.TType)) return Values.N_Error;
            Token ReturnTok = currtoken;

            Advance();
            if(currtoken.TType != Values.T_ID) return Values.N_Error;
            Token NameTok = currtoken;

            Advance();
            if(currtoken.TValue!=Values.T_LPAR) return Values.N_Error;

            Advance();
            List<Statement> Args = new List<Statement>();
            while (currtoken.TValue != Values.T_RPAR)
            {
                Statement retstatemnt = Expr();
                Advance() ;
                Args.Add(retstatemnt);
            }

            Advance();
            if (currtoken.TValue != Values.T_CR_LPAR) return Values.N_Error;

            Advance();
            List<Statement> insideStatements = new List<Statement>();
            while (currtoken.TValue != Values.T_CR_RPAR)
            {
                
                Statement retstatemnt = Expr();
                Advance();
                insideStatements.Add(retstatemnt);
            }

            
            Statement retStat = new FnStatement(ReturnTok, NameTok, Args, insideStatements);
            return retStat;
        }

        public Statement OutPrint()
        {
            Advance();
            if (currtoken.TValue != Values.T_LPAR) return Values.N_Error;
            Advance();
            Statement Args = Expr();
            Advance();
            if (currtoken.TValue!=Values.T_RPAR) return Values.N_Error;
            Advance() ;
            Statement retStat = new OutStatement(Args);
            return retStat;
        }

        public Statement AssignValue(Token AssignedName)
        {
            Advance();
            Statement expr = Expr();
            Statement AV = new ChangeValStatement(AssignedName, expr);
            return AV;
        }

        public Statement ChangeExpr()
        {
            Advance();
            Token Name = currtoken;
            Advance();
            if (currtoken.TValue != Values.T_EQUAL) return Values.STEmpty;
            Advance();
            Statement Assigned = Expr();
            ChangeValStatement res = new ChangeValStatement(Name, Assigned);
            return res;
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
                case "if":
                    return CheckRunStatements("if");
                case "while":
                    return CheckRunStatements("while");
                case "fn":
                    return Funcdecl();
                case "out":
                    return OutPrint();
                case "change":
                    return ChangeExpr();
                default:
                    List<string> COND = new List<string>() { Values.T_PLUS, Values.T_MINUS, Values.T_MULT, Values.T_MOD, Values.T_DIV, Values.T_LESS, Values.T_GREATER, Values.T_LS_EQUAL, Values.T_EQUALS, Values.T_GR_EQUAL, Values.T_EQUAL, Values.T_NOT_EQUALS };
                    Statement left = Term();
                    while (COND.Contains(currtoken.TValue))
                    {

                        Token OP = currtoken;
                        Advance();
                        Statement right = Term();
                        BinaryOP BOP = new BinaryOP(left, OP, right);
                        left = BOP;
                    }
                    return left;
            }
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
