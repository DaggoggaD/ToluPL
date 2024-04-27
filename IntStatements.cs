using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    public class IntStatements
    {
    }

    internal class OutI : IntStatements
    {
        public Statement Printable;
        public dynamic ToPrint;
        private List<Variable> CV;
        private List<Function> CF;

        public OutI(Statement PRINTABLE, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            Printable = PRINTABLE;

            CV = new List<Variable>(GlobVar);
            CF = new List<Function>(GlobFN);

            dynamic RES = interpreter.Expr(Printable, CV, CF);
            if (nameof(Node)== RES.GetType().Name)
            {
                Node NRES = (Node) RES;
                Token tok = NRES.token;
                ToPrint += Convert.ToString(tok.TValue);
            }
            Console.Write(ToPrint.Replace("\\n",Environment.NewLine));
        }

        public string REPR()
        {
            return "OUT: " + ToPrint;
        }
    }

    internal class IfI : IntStatements
    {
        public Statement CheckExpr;
        public List<Statement> IfStats;
        private List<Variable> CV;
        private List<Function> CF;

        public IfI(Statement CHECKEXPR, List<Statement> IFSTATS, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            CheckExpr = CHECKEXPR;
            IfStats = IFSTATS;

            CV = new List<Variable>(GlobVar);
            CF = new List<Function>(GlobFN);

            Node RES = interpreter.Expr(CheckExpr, CV, CF);
            if (RES.token.TValue==true)
            {
                foreach (Statement stat in IfStats)
                {
                    dynamic InRes = interpreter.Expr(stat, CV, CF);
                }
            }
        }

        public string REPR()
        {
            return "If: " + CheckExpr.REPR() + " -> RUN";
        }
    }

    internal class WhileI : IntStatements
    {
        public Statement CheckExpr;
        public List<Statement> WhileStats;
        private List<Variable> CV;
        private List<Function> CF;

        public WhileI(Statement CHECKEXPR, List<Statement> WHILESTATS, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            CheckExpr = CHECKEXPR;
            WhileStats = WHILESTATS;

            CV = new List<Variable>(GlobVar);
            CF = new List<Function>(GlobFN);

            Node RES = interpreter.Expr(CheckExpr, CV, CF);
            while (RES.token.TValue == true)
            {
                foreach (Statement stat in WhileStats)
                {
                    dynamic InRes = interpreter.Expr(stat, CV, CF);
                }
                RES = interpreter.Expr(CheckExpr, CV, CF);
            }
        }

        public string REPR()
        {
            return "If: " + CheckExpr.REPR() + " -> RUN";
        }
    }

    internal class ChangeValI : IntStatements
    {
        public string VNAME;
        public dynamic VALUE;

        public ChangeValI(Token tok, Statement value, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            VNAME = tok.TValue;
            VALUE = interpreter.Expr(value, GlobVar, GlobFN);
            foreach (Variable Var in GlobVar)
            {
                if(Var.VNAME == VNAME)
                {
                    Var.VALUE = VALUE; 
                    break;
                }
            }
        }

        public string REPR()
        {
            return "Change " + VNAME + " -> " + VALUE.REPR();
        }
    }

    internal class FnCallI : IntStatements
    {
        public string VNAME;
        public List<Statement> ARGS;
        private List<Variable> CV;
        private List<Function> CF;

        public FnCallI(Token tok, List<Statement> CARGS, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            VNAME = tok.TValue;
            ARGS = CARGS;

            //Finds the function between global (or upwards) sections of code
            Function SelectedFN = findFunc(GlobFN);
            CV = SelectedFN.CV;
            CF = SelectedFN.CF;

            List<Node> InV = new List<Node>();
            foreach (Statement stat in ARGS)
            {
                InV.Add(interpreter.Expr(stat, GlobVar, GlobFN));
            }

            int argsCount = ARGS.Count;
            int CVLEN = CV.Count;

            //assigns arguments to placeholder variables
            for(int index = -argsCount; index < 0; index++)
            {
                CV[CVLEN + index].VALUE = InV[argsCount + index];
            }

            dynamic result = RunFunc(SelectedFN,interpreter, CV,CF);
        }

        public Function findFunc(List<Function> GlobF)
        {
            foreach (Function Fn in GlobF)
            {
                if (Fn.Name.TValue == VNAME)
                {
                    return Fn;
                }
            }
            Console.WriteLine("Function name not yet defined");
            return null;
        }

        public dynamic RunFunc(Function FN, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            dynamic RES = null;
            foreach (Statement stat in FN.Insidexpr)
            {
                RES = interpreter.Expr(stat, GlobVar, GlobFN);
            }
            return RES;
        }

        public string REPR()
        {
            return "Function run ";
        }
    }
}
