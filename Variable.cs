using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    internal class Variable
    {
        public string VTYPE { get; set; }
        public string VNAME { get; set; }
        public Node VALUE { get; set; }
        public Statement AssignedVal;
        private List<Variable> CV;
        private List<Function> CF;


        public Variable(string OVTYPE, string OVNAME, Statement OASSIGNEDVAL, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            VTYPE = OVTYPE;
            VNAME = OVNAME;
            AssignedVal = OASSIGNEDVAL;
            CV = new List<Variable>(GlobVar);
            CF = new List<Function>(GlobFN);

            VALUE = interpreter.Expr(AssignedVal, CV, CF);
            TranslateVar();
        }

        public void TranslateVar()
        {
            if (VTYPE == "float")
            {
                VALUE.token.TValue = (float)VALUE.token.TValue;
                VALUE.token.TType = Values.T_FLOAT;
            }
            if (VTYPE == "int")
            {
                VALUE.token.TValue = (int)VALUE.token.TValue;
                VALUE.token.TType = Values.T_INT;
            }
        }

        public string REPR()
        {
            return "Assigned new variable:\n"
                + "\t" + VTYPE + " \n"
                + "\t" + VNAME + " \n"
                + "\t" + VALUE.REPR();
        }
    }
}
