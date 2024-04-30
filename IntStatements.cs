using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                if (tok.TType!=Values.T_LIST) ToPrint += Convert.ToString(tok.TValue);
                else
                {
                    List<Statement> arr = tok.TValue;
                    string currRepr = "";
                    currRepr += "[";
                    foreach (Statement s in arr)
                    {
                        Node x = (Node)s;
                        currRepr += ", " + x.token.TValue;
                    }
                    currRepr += "]";
                    currRepr = currRepr.Remove(1,2);
                    ToPrint+= currRepr;
                }
                
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
        public Statement Result;
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

            dynamic CalcRes = RunFunc(SelectedFN,interpreter, CV,CF);
            if (CalcRes.GetType().Name == nameof(RetStatI))
            {
                if(SelectedFN.RetValue.TValue.ToUpper() == CalcRes.Value.token.TType) Result = CalcRes.Value;
            }
            else Result = null;
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
                if (RES.GetType().Name == nameof(RetStatI)) break;
            }
            return RES;
        }

        public string REPR()
        {
            return "Function run, result: " + Result.REPR();
        }
    }

    internal class AccListI : IntStatements
    {
        public Token VNAME;
        public List<Statement> Indexes;
        public Statement VALUE;


        public AccListI(Token tok, List<Statement> INDEXES, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            VNAME = tok;
            Indexes = INDEXES;
            List<Node> Expressed_indexes = new List<Node>();
            Indexes.ForEach(x => Expressed_indexes.Add(interpreter.Expr(x, GlobVar, GlobFN)));

            Variable FoundArr = null;
            foreach (Variable Var in GlobVar)
            {
                if (Var.VNAME == VNAME.TValue)
                {
                    FoundArr = Var;
                    break;
                }
            }

            Token val = FoundArr.VALUE.token;

            List<Statement> ARRITEMS = val.TValue;
            int curr_index = (int)Expressed_indexes[0].token.TValue;
            Node res = (Node)ARRITEMS[curr_index];

            for (int i = 1; i < Expressed_indexes.Count; i++)
            {
                curr_index = (int)Expressed_indexes[i].token.TValue;
                res = res.token.TValue[curr_index];
            }
            VALUE = res;
        }

        public string REPR()
        {
            string res = "";
            Indexes.ForEach(x => res += ", " + x.REPR());
            res = res.Remove(0, 2);
            return "Access list at index: [" + res + "]" + " -> " + VALUE.REPR();
        }
    }

    internal class ChangeValArrI : IntStatements
    {
        public Token AccessName;
        public Node VALUE;

        public ChangeValArrI(ChangeValStatementArr ACCNAME, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            AccessName = ACCNAME.ArrAccName.Name;
            List<Statement> Indexes = ACCNAME.ArrAccName.Indexes;
            List<Node> Expressed_indexes = new List<Node>();
            Indexes.ForEach(x => Expressed_indexes.Add(interpreter.Expr(x, GlobVar, GlobFN)));
            VALUE = interpreter.Expr(ACCNAME.AssignedVal, GlobVar, GlobFN);

            Variable FoundArr = null;
            foreach (Variable Var in GlobVar)
            {
                if (Var.VNAME == AccessName.TValue)
                {
                    FoundArr = Var;
                    break;
                }
            }

            Token val = FoundArr.VALUE.token;

            List<Statement> ARRITEMS = val.TValue;
            int curr_index = (int)Expressed_indexes[0].token.TValue;
            Node res = (Node)ARRITEMS[curr_index];

            for (int i = 1; i < Expressed_indexes.Count; i++)
            {
                curr_index = (int)Expressed_indexes[i].token.TValue;
                res = res.token.TValue[curr_index];
            }
            res.token = VALUE.token;

        }

        public string REPR()
        {
            return "Change Arr Value:" + AccessName.TValue + " -> " + VALUE.REPR();
        }
    }

    internal class RetStatI : IntStatements
    {
        public Statement Value;

        public RetStatI(Statement VALUE, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            Value = interpreter.Expr(VALUE, GlobVar, GlobFN);
        }

        public string REPR()
        {
            return "Returned value: " + Value.REPR();
        }
    }
}