using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    internal class Token
    {
        public string TType;
        public string TValue;
        public float? TValueNum;
        
        public Token(string ttype, string tvalue, float? tvaluenum = null) {
            TType = ttype;
            TValue = tvalue;
            TValueNum = tvaluenum;
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
            if (TValue!="NUMBER") Console.WriteLine(TType + ": " + TValue);
            else Console.WriteLine(TType + ": " + TValueNum);
        }
    }
}
