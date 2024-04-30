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
        public Token nextoken;
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
            if(tokID+1 < tokens.Count)
            {
                nextoken = tokens[tokID+1];
            }
        }

        public Statement Factor()
        {
            Token tok = currtoken;
            List<string> COND = new List<string>() { Values.T_INT, Values.T_LONG, Values.T_FLOAT, Values.T_DOUBLE, Values.T_ID, Values.T_STRING, Values.T_LIST, Values.T_BOOL};
            if (COND.Contains(tok.TType))
            {
                if (nextoken.TValue != Values.T_SQ_LPAR)
                {
                    Advance();
                    Node node = new Node(tok);
                    return node;
                }
                Statement LSnode = Expr();
                Advance();
                return LSnode;
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
            Token StTok = new Token(Values.T_INT, -999);
            Node StNode = new Node(StTok);
            if (currtoken.TValue == Values.T_SEMICOLON || currtoken.TValue == Values.T_SEMICOLON) ASTATEMENT = new AssignStatement(Varname, StNode, VARTYPE);
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
            Token Name = currtoken;
            Advance();
            if (Convert.ToString(currtoken.TValue) != Values.T_EQUAL) return Values.STEmpty;
            Advance();
            Statement Assigned = Expr();
            ChangeValStatement res = new ChangeValStatement(Name, Assigned);
            return res;
        }

        public Statement CreateBOP(Statement left = null)
        {
            //REMOVED T_EQUAL, IF IT DOESNT WORK CHECK AGAIN (DONE FORE ARR CHANGE VALUE)
            List<string> COND = new List<string>() { Values.T_PLUS, Values.T_MINUS, Values.T_MULT, Values.T_MOD, Values.T_DIV, Values.T_LESS, Values.T_GREATER, Values.T_LS_EQUAL, Values.T_EQUALS, Values.T_GR_EQUAL, Values.T_NOT_EQUALS };
            if(left == null) left = Term();
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

        public Statement FnCallExpr()
        {
            Token Name = currtoken;
            Advance();
            if(currtoken.TValue!=Values.T_LPAR) return Values.STEmpty;
            Advance();
            List<Statement> Args = new List<Statement>();
            while (Convert.ToString(currtoken.TValue) != Values.T_RPAR)
            {
                Statement retstatemnt = Expr();
                Advance();
                Args.Add(retstatemnt);
            }
            if (currtoken.TValue != Values.T_RPAR) return Values.STEmpty;

            Advance();
            FnCallStatement fncall = new FnCallStatement(Name, Args);
            return fncall;
        }

        public Node GenerateArr()
        {
            Token tok = new Token(Values.T_LIST, null);
            Node retNode = new Node(tok);
            List<Statement> listStat = new List<Statement>();
            Advance();
            while (Convert.ToString(currtoken.TValue) != Values.T_SQ_RPAR)
            {
                Statement cs = Expr();
                Advance();
                listStat.Add(cs);
            }
            Advance();
            retNode.token.TValue = listStat;
            return retNode;
        }

        public Statement AccLsItemExpr()
        {
            Token VarName = currtoken;
            Advance();
            if (currtoken.TValue != Values.T_SQ_LPAR) return Values.STEmpty;
            Advance();
            List<Statement> indexes = new List<Statement>();
            while (Convert.ToString(currtoken.TValue) != Values.T_SQ_RPAR)
            {
                Statement currnode = Expr();
                indexes.Add(currnode);
                Advance();
            }
            AccListStatement als = new AccListStatement(VarName, indexes);
            return als;
        }

        public Statement HandleDefault()
        {
            if (currtoken.TType == Values.T_ID)
            {
                switch (nextoken.TValue)
                {
                    case Values.T_EQUAL:
                        return ChangeExpr();
                    case Values.T_LPAR:
                        return FnCallExpr();
                    case Values.T_SQ_LPAR:
                        Statement ListAccess = AccLsItemExpr();
                        Advance();
                        ListAccess = CreateBOP(ListAccess);
                        if (Convert.ToString(currtoken.TValue) != Values.T_EQUAL) return ListAccess;
                        Advance();
                        Statement toAssign = Expr();
                        ChangeValStatementArr cvs = new ChangeValStatementArr((AccListStatement)ListAccess,toAssign);
                        return cvs;
                    default:
                        return CreateBOP();
                }
            }
            else if (currtoken.TType == Values.T_OP)
            {
                switch (currtoken.TValue)
                {
                    case Values.T_SQ_LPAR:
                        return GenerateArr();
                    default:
                        break;
                }
            }
            return CreateBOP();            
        }

        public Statement AssignRunStatement(string VARTYPE)
        {
            Advance();
            Statement Assigned = Expr();
            ReturnStatement retst = new ReturnStatement(Assigned);
            return retst;
        }

        public Statement Expr() {
            switch (currtoken.TValue)
            {
                case "int":
                    return AssignFunc("int");
                case "float":
                    return AssignFunc("float");
                case "double":
                    return AssignFunc("double");
                case "long":
                    return AssignFunc("long");
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
                case "return":
                    return AssignRunStatement("return");
                case "fn":
                    return Funcdecl();
                case "out":
                    return OutPrint();
                default:
                    return HandleDefault();
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
