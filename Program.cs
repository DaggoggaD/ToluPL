using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    internal class Program
    {
        public static List<Token> toks;
        public static bool SHUT = false;

        public static List<string> OpenFile(string filepath)
        {
            string Cpath = "";
            List<string> lines = new List<string>();

            if (filepath.StartsWith("RUN"))
            {
                Cpath = filepath.Substring(4);
                lines = File.ReadAllLines(Cpath).ToList();
                return lines;
            }
            else if (filepath == "")
            {
                Cpath = "C:\\Users\\Andrea\\source\\repos\\RunePL\\RunePL\\ToluFiles\\script.tolu";
                lines = File.ReadAllLines(Cpath).ToList();
                return lines;
            }
            lines.Add(filepath);
            return lines;
        }

        static void INFO(List<string> retVal, List<Statement> statements)
        {
            if (!SHUT)
            {
                Console.WriteLine("LEXER result\n");
                toks.ForEach(x => x.REPR());
                Console.WriteLine("\nPARSER result\n");
                statements.ForEach(statement => { Console.WriteLine(statement.REPR()); });
                Console.WriteLine("\nCOMPILER result\n");
            }
        }

        static void Main(string[] args)
        {
            //Instructions
            Console.WriteLine("Specify script path.");
            Console.WriteLine("Write RUN followed by script path.");
            Console.WriteLine("Write END to exit.");
            Console.Write("Shell> ");

            //Read file lines
            string Filename = Console.ReadLine();
            List<string> retVal = OpenFile(Filename);

            //Lexer init and calc
            Lexer lexer = new Lexer(retVal);
            toks = lexer.tokens;

            //Parser
            Parser parser = new Parser(toks);
            List<Statement> statements = parser.statements;

            //Compiler
            Interpreter interpreter = new Interpreter(statements);

            INFO(retVal, statements);
        }
    }
}
