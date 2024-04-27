using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ToluPL
{
    internal class Function
    {
        public Token Name { get; set; }
        public Token RetValue { get; set; }
        public List<Statement> STArgs;
        public List<Statement> Insidexpr;

        public List<Variable> CV {get; set;}
        public List<Function> CF { get; set; }

        public List<Variable> ArgsVal { get; set; }

        public Function(Token NAME, Token RETVALUE, List<Statement> STARGS, List<Statement> INSIDEXPR, Interpreter interpreter, List<Variable> GlobVar, List<Function> GlobFN) { 
            Name = NAME;
            RetValue = RETVALUE;
            STArgs = STARGS;
            Insidexpr = INSIDEXPR;

            CV = new List<Variable>(GlobVar);
            CF = new List<Function>(GlobFN);

            //Args decoded
            foreach (Statement stat in STArgs)
            {
                interpreter.Expr(stat, CV, CF);
            }
        }

        public string REPR()
        {
            string args = "";
            STArgs.ForEach(t => {args+= t.REPR() + ", "; });
            try
            {
                args = args.Substring(0, args.Length - 2);
            }catch (Exception ex)
            {
                args = "No Args";
            }

            string insexpr = "";
            Insidexpr.ForEach(t => { insexpr += t.REPR() + ", "; });
            try
            {
                insexpr = insexpr.Substring(0, insexpr.Length - 2);
            }
            catch (Exception e)
            {
                insexpr = "No Expr";
            }
            

            return "Assigned new Function:\n"
                + "\t[" + RetValue.TValue + "] " + Name.TValue + "\n"
                + "\t Argumrnts: [" + args + "] \n"
                + "\t Inside Expr: [" + insexpr + "]";
        }
    }
}
