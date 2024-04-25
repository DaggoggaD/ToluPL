using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    public class Token
    {
        public string TType;
        public dynamic TValue;
        
        public Token(string ttype, dynamic tvalue) {
            TType = ttype;
            TValue = tvalue;
        }

        public bool Equals(Token other)
        {
            if (this == other) return true;
            return false;
        }

        public bool TypeEquals(string other)
        {
            if (this.TType == other) return true;
            return false;
        }

        public bool ValueEquals(string other)
        {
            if (this.TValue == other) return true;
            return false;
        }

        public void REPR()
        {
            Console.WriteLine(TType + ": " + TValue);
        }
    }
}