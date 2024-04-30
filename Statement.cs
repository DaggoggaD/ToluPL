using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            try
            {
                args = args.Substring(0, args.Length - 2);
            }
            catch (Exception e){ args = "No Args"; }

            insidexpr.ForEach(ie => reprstr += ie.REPR() + ", ");
            try
            {
                reprstr = reprstr.Substring(0, reprstr.Length - 2);
            }
            catch{ reprstr = "No Expr"; }
            return "(Function: " + "[" + returnTok.TValue +  "] " + " [" + fnName.TValue + "] " + " [" + args + "]" + " ->" + reprstr;
        }
    }

    class FnCallStatement : Statement
    {
        public Token fnName;
        public List<Statement> arguments;

        public FnCallStatement(Token FNNAME, List<Statement> ARGUMENTS)
        {
            fnName = FNNAME;
            arguments = ARGUMENTS;
        }

        public override string REPR()
        {
            string args = "";
            arguments.ForEach(ie => args += ie.REPR() + ", ");
            try
            {
                args = args.Substring(0, args.Length - 2);
            }
            catch (Exception e) { args = "No Args"; }
            
            return $"(Function Call: {fnName}, Args:[{args}])";
        }
    }

    class OutStatement : Statement
    {
        public Statement printable;

        public OutStatement(Statement PRINTABLE)
        {
            printable = PRINTABLE;
        }

        public override string REPR()
        {
            return "(Out(" + printable.REPR() + "))";
        }
    }

    class ChangeValStatement : Statement
    {
        public Token Name;
        public Statement Value;

        public ChangeValStatement(Token NAME, Statement VALUE)
        {
            Name = NAME;
            Value = VALUE;
        }

        public override string REPR()
        {
            return "(Assign: " + Value.REPR() + " -> " + "(" + Name.TType +" : " + Name.TValue +")" + ")";
        }
    }

    class ChangeValStatementArr : Statement
    {
        public AccListStatement ArrAccName;
        public Statement AssignedVal;

        public ChangeValStatementArr(AccListStatement ARRNAME, Statement VALUE)
        {
            ArrAccName = ARRNAME;
            AssignedVal = VALUE;
        }

        public override string REPR()
        {
            string indexes = "";
            ArrAccName.Indexes.ForEach(x=> indexes += x.REPR());
            
            return "(Assign: " + "(" + AssignedVal.REPR() + ")" + " -> " + ArrAccName.Name.TValue + "[" + indexes + "]" + ")";
        }
    }

    class AccListStatement : Statement
    {
        public Token Name;
        public List<Statement> Indexes;

        public AccListStatement(Token NAME, List<Statement>INDEXES)
        {
            Name = NAME;
            Indexes = INDEXES;

        }

        public override string REPR()
        {
            string res = "";
            Indexes.ForEach(s=> res += ", " + s.REPR());
            res = res.Remove(0,2);
            return $"(Access list: {Name}[{res}])";
        }
    }

    class ReturnStatement : Statement
    {
        public Statement Value;
        public ReturnStatement(Statement VALUE)
        {
            Value = VALUE;
        }

        public override string REPR()
        {
            return $"(Return: {Value.REPR()})";
        }
    }
}
