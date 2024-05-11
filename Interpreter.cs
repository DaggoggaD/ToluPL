using System;
using System.Collections.Generic;

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
            switch (op.TValue)
            {
                //REMEMBER: CURRENTLY IF THE RESULT IS OF A DIFFERENT TYPE, IT DOESNT CHANGE
                case Values.T_PLUS:
                    return new Token(LeftNode.token.TType, LeftNode.token.TValue + RightNode.token.TValue);
                case Values.T_MINUS:
                    return new Token(LeftNode.token.TType, LeftNode.token.TValue - RightNode.token.TValue);
                case Values.T_MULT:
                    return new Token(LeftNode.token.TType, LeftNode.token.TValue * RightNode.token.TValue);
                case Values.T_DIV:
                    return new Token(LeftNode.token.TType, (float)LeftNode.token.TValue / (float)RightNode.token.TValue);
                case Values.T_POW:
                    return new Token(LeftNode.token.TType, (float)Math.Pow((double)LeftNode.token.TValue, (double)RightNode.token.TValue));
                case Values.T_MOD:
                    return new Token(LeftNode.token.TType, LeftNode.token.TValue % RightNode.token.TValue);
                case Values.T_LESS:
                    return new Token(Values.T_BOOL, (LeftNode.token.TValue < RightNode.token.TValue));
                case Values.T_GREATER:
                    return new Token(Values.T_BOOL, (LeftNode.token.TValue > RightNode.token.TValue));
                case Values.T_EQUALS:
                    return new Token(Values.T_BOOL, (LeftNode.token.TValue == RightNode.token.TValue));
                case Values.T_NOT_EQUALS:
                    return new Token(Values.T_BOOL, (LeftNode.token.TValue != RightNode.token.TValue));
                case Values.T_GR_EQUAL:
                    return new Token(Values.T_BOOL, (LeftNode.token.TValue >= RightNode.token.TValue));
                case Values.T_LS_EQUAL:
                    return new Token(Values.T_BOOL, (LeftNode.token.TValue <= RightNode.token.TValue));
                default:
                    return Values.Empty;
            }
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
                    return NodeAnalisis((Node)statement, GV, GF);

                case nameof(AssignStatement):
                    AssignStatement cstatement = (AssignStatement)statement;
                    Variable variable = new Variable(cstatement.VarType, cstatement.Varname.TValue, cstatement.expr, this, GV, GF);
                    foreach(Variable var in GV)
                    {
                        if(var.VNAME == variable.VNAME)
                        {
                            throw new Exception($"Variable name already declared!{var.REPR()}");
                        }
                    }
                    GV.Add(variable);
                    return variable;

                case nameof(BinaryOP):
                    Token resTok = CalcBinOP((BinaryOP)statement, GV, GF);
                    Node result = new Node(resTok);
                    return result;

                case nameof(OutStatement):
                    OutStatement ost = (OutStatement)statement;
                    OutI OutF = new OutI(ost.printable, this, GV, GF);
                    return OutF;

                case nameof(IfStatement):
                    IfStatement ifStatement = (IfStatement)statement;
                    IfI ifInterp = new IfI(ifStatement.checkexpr, ifStatement.ifexpr, this, GV, GF);
                    return ifInterp;

                case nameof(WhileStatement):
                    WhileStatement whileStatement = (WhileStatement)statement;
                    WhileI whileInterp = new WhileI(whileStatement.checkexpr, whileStatement.insidexpr, this, GV, GF);
                    return whileInterp;

                case nameof(ChangeValStatement):
                    ChangeValStatement Assign = (ChangeValStatement)statement;
                    ChangeValI AI = new ChangeValI(Assign.Name, Assign.Value, this, GV, GF);
                    return AI;
                case nameof(FnStatement):
                    FnStatement fnStatement = (FnStatement)statement;
                    Function fn = new Function(fnStatement.fnName, fnStatement.returnTok, fnStatement.arguments, fnStatement.insidexpr, this, GV, GF);
                    GF.Add(fn);
                    return fn;

                case nameof(FnCallStatement):
                    FnCallStatement fnCallst = (FnCallStatement)statement;
                    FnCallI FnCall = new FnCallI(fnCallst.fnName, fnCallst.arguments, this, GV, GF);
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

                case nameof(AppendStatement):
                    AppendStatement appendStatement = (AppendStatement)statement;
                    AppStatI appStatI = new AppStatI(appendStatement.Name, appendStatement.Value, this,GV,GF);
                    
                    return appStatI;
                default:
                    return Values.STEmpty;
            }
        }

        public void Cycle()
        {
            while (currstatement!=Values.STEnd)
            {
                Expr(currstatement,GlobalVariables,GlobalFunctions);
                Advance();
            }
        }
    }
}