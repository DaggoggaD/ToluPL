using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ToluPL
{
    internal class Interpreter
    {
        public List<Variable> GlobalVariables = new List<Variable>();
        public List<Function> GlobalFunctions = new List<Function>();
        public List<Statement> statements;
        public Statement currstatement;
        public int stID = -1;

        public Interpreter(List<Statement> STATEMENTS)
        {
            statements= STATEMENTS;
            statements.Add(Values.STEnd);
            Advance();
            Cycle();
        }

        public void Advance()
        {
            stID += 1;
            if (stID<statements.Count)
            {
                currstatement = statements[stID];
            }
        }

        public Token CalcNodesResult(Node LeftNode, Token op, Node RightNode)
        {
            string STType = LeftNode.token.TType;
            Token RET;
            switch (op.TValue)
            {
                //REMEMBER: CURRENTLY IF THE RESULT IS OF A DIFFERENT TYPE, IT DOESNT CHANGE
                case Values.T_PLUS:
                    RET = new Token(LeftNode.token.TType, LeftNode.token.TValue + RightNode.token.TValue);
                    break;
                case Values.T_MINUS:
                    RET = new Token(LeftNode.token.TType, LeftNode.token.TValue - RightNode.token.TValue);
                    break;
                case Values.T_MULT:
                    RET = new Token(LeftNode.token.TType, LeftNode.token.TValue * RightNode.token.TValue);
                    break;
                case Values.T_DIV:
                    RET = new Token(LeftNode.token.TType, (float)LeftNode.token.TValue / (float)RightNode.token.TValue);
                    break;
                case Values.T_POW:
                    RET = new Token(LeftNode.token.TType, (float)Math.Pow((double)LeftNode.token.TValue, (double)RightNode.token.TValue));
                    break;
                case Values.T_LESS:
                    RET = new Token(Values.T_BOOL, (LeftNode.token.TValue < RightNode.token.TValue));
                    break;
                case Values.T_GREATER:
                    RET = new Token(Values.T_BOOL, (LeftNode.token.TValue > RightNode.token.TValue));
                    break;
                case Values.T_EQUALS:
                    RET = new Token(Values.T_BOOL, (LeftNode.token.TValue == RightNode.token.TValue));
                    break;
                case Values.T_NOT_EQUALS:
                    RET = new Token(Values.T_BOOL, (LeftNode.token.TValue != RightNode.token.TValue));
                    break;
                case Values.T_GR_EQUAL:
                    RET = new Token(Values.T_BOOL, (LeftNode.token.TValue >= RightNode.token.TValue));
                    break;
                case Values.T_LS_EQUAL:
                    RET = new Token(Values.T_BOOL, (LeftNode.token.TValue <= RightNode.token.TValue));
                    break;
                default:
                    RET = Values.Empty;
                    break;
            }
            return RET;
        }

        public Token CalcBinOP(BinaryOP statement, List<Variable> GV, List<Function> GF)
        {
            Statement Left = statement.left;
            Token op = statement.opTOKEN;
            Statement Right = statement.right;


            //Solves left and right id they're another operation
            if (Left.GetType().Name != nameof(Node)) Left = Expr(Left, GV, GF);
            if (Right.GetType().Name != nameof(Node)) Right = Expr(Right, GV, GF);


            //Checks for left and right type
            Node LeftNode = NodeAnalisis((Node)Left,GV,GF);
            Node RightNode = NodeAnalisis((Node)Right,GV,GF);

            return CalcNodesResult(LeftNode,op,RightNode);
        }

        public Node NodeAnalisis(Node statement, List<Variable> GV, List<Function> GF)
        {
            Token tok = statement.token;
            if(tok.TType == Values.T_ID)
            {
                foreach (Variable Var in GV)
                {
                    if (Var.VNAME == tok.TValue) return Var.VALUE;
                }
                return statement;
            }
            else if(tok.TType == Values.T_KW) { return statement; }
            else
            {
                return statement;
            }
        }

        public dynamic Expr(Statement statement, List<Variable> GV, List<Function> GF)
        {
            switch (statement.GetType().Name)
            {
                case nameof(Node):

                    return NodeAnalisis((Node)statement,GV,GF);

                case nameof(AssignStatement):
                    AssignStatement cstatement = (AssignStatement)statement;
                    Variable variable = new Variable(cstatement.VarType, cstatement.Varname.TValue, cstatement.expr, this, GV,GF);
                    GV.Add(variable);
                    return variable;

                case nameof(BinaryOP):
                    Token resTok = CalcBinOP((BinaryOP)statement, GV, GF);
                    Node result = new Node(resTok);
                    return result;

                default:
                    return Values.STEmpty;
            }
        }

        public void Cycle()
        {
            while (currstatement!=Values.STEnd)
            {
                dynamic result = Expr(currstatement,GlobalVariables,GlobalFunctions);
                Advance();
                if(nameof(Variable)!= result.GetType().Name) Console.WriteLine(result.REPR());
            }
            foreach (Variable var in GlobalVariables)
            {
                Console.WriteLine(var.REPR());
            }
        }
    }
}
