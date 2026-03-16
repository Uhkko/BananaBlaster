using Microsoft.Z3;

namespace BananaBlaster.Formula;

public abstract class Term : Element
{
    public abstract int VectorSize { get; }
    
    public BoolExpr[] GetRepresentatives(BBContext context)
    {
        var z3Context = context.Z3Context;
        var representatives = new BoolExpr[VectorSize];
        var hash = GetHashCode();
        
        for (int i = 0; i < representatives.Length; i++)
        {
            var symbol = z3Context.MkSymbol(hash + i);
            representatives[i] = z3Context.MkBoolConst(symbol);
        }
        
        return representatives;
    }
    
    public override int GetVariableCount()
    {
        return VectorSize;
    }
}