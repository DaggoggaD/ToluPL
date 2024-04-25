using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    public class Statement
    {

        public Statement() { }

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
            return "(" + token.TType + " : " + token.TValue + ")";
        }
    }

    class BinaryOP : Statement
    {
        public Statement left { get; set; }
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

    class IfStatement : Statement
    {
        public Statement checkexpr;
        public List<Statement> ifexpr;

        public IfStatement(Statement CHECKEXPR, List<Statement> IFEXPR)
        {
            checkexpr = CHECKEXPR;
            ifexpr = IFEXPR;
        }

        public override string REPR()
        {
            string reprstr = "";
            ifexpr.ForEach(ie => reprstr += ie.REPR()+", ");
            reprstr = reprstr.Substring(0,reprstr.Length - 2);
            return "(IF: " + "[" + checkexpr.REPR() + "]" + "->" + reprstr + ")";
        }
    }

    class WhileStatement : Statement
    {
        public Statement checkexpr;
        public List<Statement> insidexpr;

        public WhileStatement(Statement CHECKEXPR, List<Statement> INSIDEXPR)
        {
            checkexpr = CHECKEXPR;
            insidexpr = INSIDEXPR;
        }

        public override string REPR()
        {
            string reprstr = "";
            insidexpr.ForEach(ie => reprstr += ie.REPR() + ", ");
            reprstr = reprstr.Substring(0, reprstr.Length - 2);
            return "(WHILE: " + "[" + checkexpr.REPR() + "]" + "->" + reprstr + ")";
        }
    }
    
    class FnStatement : Statement
    {
        public Token returnTok;
        public Token fnName;
        public List<Statement> arguments;
        public List<Statement> insidexpr;

        public FnStatement(Token RETURNTOK, Token FNNAME, List<Statement> ARGUMENTS, List<Statement> INSIDEXPR)
        {
            returnTok = RETURNTOK;
            fnName = FNNAME;
            arguments = ARGUMENTS;
            insidexpr = INSIDEXPR;
        }

        public override string REPR()
        {
            string reprstr = "";
            string args = "";

            arguments.ForEach(ie => args += ie.REPR() + ", ");
            args = args.Substring(0, args.Length - 2);

            insidexpr.ForEach(ie => reprstr += ie.REPR() + ", ");
            reprstr = reprstr.Substring(0, reprstr.Length - 2);
            return "(Function: " + "[" + returnTok.TValue +  "] " + " [" + fnName.TValue + "] " + " [" + args + "]" + " ->" + reprstr;
        }
    }
}
