using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    internal static class ParseUtils
    {

        public static Node TryParseNode(Node node, string StrConv)
        {
            bool done;
            switch (StrConv)
            {
                case Values.T_INT:
                    int INTres;
                    done = Int32.TryParse(node.token.TValue.ToString(), out INTres);
                    if (done) return new Node(new Token(Values.T_INT, INTres));
                    else throw new Exception($"Invalid type conversion{node.token.TType}->{Values.T_INT}");
                case Values.T_LONG:
                    long LONGres;
                    done = long.TryParse(node.token.TValue.ToString(), out LONGres);
                    if (done) return new Node(new Token(Values.T_LONG, LONGres));
                    else throw new Exception($"Invalid type conversion{node.token.TType}->{Values.T_LONG}");
                case Values.T_FLOAT:
                    float FLOATres;
                    done = float.TryParse(node.token.TValue.ToString(), out FLOATres);
                    if (done) return new Node(new Token(Values.T_FLOAT, FLOATres));
                    else throw new Exception($"Invalid type conversion{node.token.TType}->{Values.T_FLOAT}");
                case Values.T_DOUBLE:
                    double DOUBLEres;
                    done = double.TryParse(node.token.TValue.ToString(), out DOUBLEres);
                    if (done) return new Node(new Token(Values.T_FLOAT, DOUBLEres));
                    else throw new Exception($"Invalid type conversion{node.token.TType}->{Values.T_DOUBLE}");
                case Values.T_STRING:
                    if (node.token.TType.ToString() == Values.T_STRING) return node;
                    throw new Exception($"Invalid type conversion{node.token.TType}->{Values.T_DOUBLE}");
                case Values.T_LIST:
                    if (node.token.TType.ToString() == Values.T_LIST) return node;
                    throw new Exception($"Invalid type conversion{node.token.TType}->{Values.T_DOUBLE}");
                case Values.T_BOOL:
                    if (node.token.TType.ToString() == Values.T_BOOL) return node;
                    throw new Exception($"Invalid type conversion{node.token.TType}->{Values.T_DOUBLE}");
                default:
                    throw new Exception("Invalid type passed");
            }
        }
    }
}