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
                case Values.T_MOD:
                    RET = new Token(LeftNode.token.TType, LeftNode.token.TValue % RightNode.token.TValue);
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
                if(tok.TType == Values.T_LIST)
                {
                    List<Statement> listStat = new List<Statement>();
                    foreach (Statement stat in tok.TValue)
                    {
                        Node res = Expr(stat, GV, GF);
                        listStat.Add(res);
                    }
                    tok.TValue = listStat;
                    statement.token = tok;
                    return statement;
                }
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

                case nameof(OutStatement):
                    OutStatement ost = (OutStatement)statement;
                    OutI OutF = new OutI(ost.printable,this,GV,GF);
                    return OutF;

                case nameof(IfStatement):
                    IfStatement ifStatement = (IfStatement)statement;
                    IfI ifInterp = new IfI(ifStatement.checkexpr,ifStatement.ifexpr, this,GV,GF);
                    return ifInterp;

                case nameof(WhileStatement):
                    WhileStatement whileStatement = (WhileStatement)statement;
                    WhileI whileInterp = new WhileI(whileStatement.checkexpr,whileStatement.insidexpr,this,GV,GF);
                    return whileInterp;

                case nameof(ChangeValStatement):
                    ChangeValStatement Assign = (ChangeValStatement)statement;
                    ChangeValI AI = new ChangeValI(Assign.Name,Assign.Value,this,GV,GF);
                    return AI;
                case nameof(FnStatement):
                    FnStatement fnStatement = (FnStatement)statement;
                    Function fn = new Function(fnStatement.fnName,fnStatement.returnTok, fnStatement.arguments,fnStatement.insidexpr, this, GV,GF);
                    GF.Add(fn);
                    return fn;

                case nameof(FnCallStatement):
                    FnCallStatement fnCallst = (FnCallStatement)statement;
                    FnCallI FnCall = new FnCallI(fnCallst.fnName,fnCallst.arguments,this,GV,GF);
                    return FnCall.Result;

                case nameof(AccListStatement):
                    AccListStatement accList = (AccListStatement)statement;
                    AccListI AccListI = new AccListI(accList.Name, accList.Indexes, this, GV, GF);
                    return AccListI.VALUE;

                case nameof(ChangeValStatementArr):
                    ChangeValStatementArr changeValStatementArr = (ChangeValStatementArr)statement;
                    ChangeValArrI res = new ChangeValArrI(changeValStatementArr, this, GV, GF);
                    return res;

                case nameof(ReturnStatement):
                    ReturnStatement returnStatement = (ReturnStatement)statement;

                    RetStatI retStatI = new RetStatI(returnStatement.Value, this, GV, GF);

                    return retStatI;
                default:
                    return Values.STEmpty;
            }
        }

        public void Cycle()
        {
            if(!Program.SHUT) Console.Write("----START OF PROGRAM----\n\n");
            while (currstatement!=Values.STEnd)
            {
                dynamic result = Expr(currstatement,GlobalVariables,GlobalFunctions);
                Advance();
            }
            if (!Program.SHUT)
            {
                Console.Write("\n----END OF PROGRAM----\n");
                Console.WriteLine("\n--Global variables debug--\n");
                foreach (Variable var in GlobalVariables)
                {
                    Console.WriteLine(var.REPR());
                }
                Console.WriteLine("\n--Global functions debug--\n");
                foreach (Function fn in GlobalFunctions)
                {
                    Console.WriteLine(fn.REPR());
                }
                Console.WriteLine();
            }
        }
    }
}