using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ToluPL;

namespace RunePL
{
    internal class Compiler
    {
        public List<Statement> statements;
        public Statement currstatement;
        public int statID;

        public Compiler(List<Statement> STATEMENTS) {
            statements = STATEMENTS;
            Advance();
        }

        public void Advance() {
            statID += 1;
            if (statID < statements.Count)
            {
                currstatement = statements[statID];
            }
        }


    }
}
