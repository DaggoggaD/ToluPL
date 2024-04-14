using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    class Statement
    {
        public virtual string REPR() {
            return "";
        }
    }

    class Node : Statement
    {
        public Token token;
        public Node(Token TOKEN)
        {
            token = TOKEN;
        }

        public override string REPR()
        {
            if (token.TValue != "NUMBER") return "(" + token.TType + " : " + token.TValue + ")";
            else return "(" + token.TType + " : " + token.TValueNum + ")";
        }
    }

    class BinaryOP : Statement
    {
        public Statement left;
        public Token opTOKEN;
        public Statement right;

        public BinaryOP(Statement LEFT, Token OPT, Statement RIGHT) {
            left = LEFT;
            opTOKEN = OPT;
            right = RIGHT;
        }

        public override string REPR()
        {
            return "(" + left.REPR() + " " + opTOKEN.TValue + " " + right.REPR() +")";
        }
    }

    class AssignStatement : Statement
    {
        public Token Varname;
        public Statement expr;
        public string VarType;

        public AssignStatement(Token VARNAME, Statement EXPR, string VARTYPE)
        {
            Varname = VARNAME;
            expr = EXPR;
            VarType = VARTYPE;
        }

        public override string REPR()
        {
            return "(ASSIGN: " + "[" + expr.REPR() + "]" + "->" + Varname.TType + " : " + Varname.TValue + ")";
        }
    }
}
