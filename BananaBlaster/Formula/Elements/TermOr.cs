using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermOr : Term
{
    private Term Term1 { get; }
    private Term Term2 { get; }

    public override Element[] Children => [Term1, Term2];
    protected override object[] Identifiers => [Term1, Term2];

    public override int VectorSize => Term1.VectorSize;
    
    public TermOr(Term t1, Term t2)
    {
        if (t1.VectorSize != t2.VectorSize)
            throw new ArgumentException($"Failed to create element: Terms have different vector sizes ({t1.VectorSize} vs. {t2.VectorSize})");
        
        Term1 = t1;
        Term2 = t2;
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var childReps1 = Term1.GetRepresentatives(context);
        var childReps2 = Term2.GetRepresentatives(context);
        var selfReps = GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], z3Context.MkOr(childReps1[i], childReps2[i]));
        
        return z3Context.MkAnd(bitConstraints);
    }
    
    public override int GetOperatorCount()
    {
        return 3 * VectorSize - 1;
    }
}