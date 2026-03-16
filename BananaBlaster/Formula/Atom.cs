using Microsoft.Z3;

namespace BananaBlaster.Formula;

public abstract class Atom : Element
{
    public BoolExpr GetRepresentative(BBContext context)
    {
        var z3Context = context.Z3Context;
        var hash = GetHashCode();
        var symbol = z3Context.MkSymbol(hash);
        return z3Context.MkBoolConst(symbol);
    }
    
    public override int GetVariableCount()
    {
        return 1;
    }
}