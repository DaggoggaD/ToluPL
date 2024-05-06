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

        static void INFO(List<string> retVal, List<Statement> statements, Interpreter interpreter)
        {
            if (!SHUT)
            {
                Console.WriteLine("\nLEXER result");
                toks.ForEach(x => x.REPR());
                Console.WriteLine("\nPARSER result\n");
                statements.ForEach(statement => { Console.WriteLine(statement.REPR()); });
                Console.WriteLine("INTERPRETER DEBUG");
                Console.WriteLine("\n--Global variables debug--\n");
                foreach (Variable var in interpreter.GlobalVariables)
                {
                    Console.WriteLine(var.REPR());
                }
                Console.WriteLine("\n--Global functions debug--\n");
                foreach (Function fn in interpreter.GlobalFunctions)
                {
                    Console.WriteLine(fn.REPR());
                }
                Console.WriteLine();                
            }
        }

        static void Main(string[] args)
        {
            //Instructions
            Console.WriteLine("Specify script path.");
            Console.WriteLine("Write RUN followed by script path.");
            Console.WriteLine("Write END to exit.");
            
            while (true)
            {
                //Read file lines
                var watch = new System.Diagnostics.Stopwatch();
                Console.Write("Shell> ");
                string Filename = Console.ReadLine();
                List<string> retVal = OpenFile(Filename);
                //Lexer init and calc
                Lexer lexer = new Lexer(retVal);
                toks = lexer.tokens;

                //Parser
                Parser parser = new Parser(toks);
                List<Statement> statements = parser.statements;

                //Interpreter

                if (!SHUT) Console.Write("----START OF PROGRAM----\n\n");
                watch.Start();
                Interpreter interpreter = new Interpreter(statements);
                watch.Stop();
                Console.Write("\n----END OF PROGRAM----\n");
                INFO(retVal, statements, interpreter);
                Console.WriteLine($"\n[Execution Time: {watch.ElapsedMilliseconds} ms]");
            }
        }
    }
}