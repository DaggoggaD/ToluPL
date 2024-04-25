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

        public Variable(string OVTYPE, string OVNAME, Statement OASSIGNEDVAL, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN)
        {
            VTYPE = OVTYPE;
            VNAME = OVNAME;
            AssignedVal = OASSIGNEDVAL;
            VALUE = interpreter.Expr(AssignedVal, GlobVar, GlobFN);
        }

        public string REPR()
        {
            return "Assigned new variable:\n"
                + "\t" + VTYPE + " \n"
                + "\t" + VNAME + " \n"
                + "\t" + VALUE.REPR();
        }
    }

    class Function
    {

    }
}
