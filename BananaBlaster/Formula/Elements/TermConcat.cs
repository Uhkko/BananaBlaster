using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermConcat(Term term1, Term term2) : Term
{
    private Term Term1 { get; } = term1;
    private Term Term2 { get; } = term2;

    public override Element[] Children => [Term1, Term2];
    protected override object[] Identifiers => [Term1, Term2];

    public override int VectorSize => Term1.VectorSize + Term2.VectorSize;

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var childReps1 = Term1.GetRepresentatives(context);
        var childReps2 = Term2.GetRepresentatives(context);
        var selfReps = GetRepresentatives(context);
        
        for (int i = 0; i < Term2.VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], childReps2[i]);
        for (int i = 0; i < Term1.VectorSize; i++)
            bitConstraints[i + Term2.VectorSize] = z3Context.MkEq(selfReps[i + Term2.VectorSize], childReps1[i]);
        
        return z3Context.MkAnd(bitConstraints);
    }
    
    public override int GetOperatorCount()
    {
        return 2 * VectorSize - 1;
    }
}