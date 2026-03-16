using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermNot(Term term) : Term
{
    private Term Term { get; } = term;

    public override Element[] Children => [Term];
    protected override object[] Identifiers => [Term];

    public override int VectorSize => Term.VectorSize;

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var childReps = Term.GetRepresentatives(context);
        var selfReps = GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], z3Context.MkNot(childReps[i]));
        
        return z3Context.MkAnd(bitConstraints);
    }
    
    public override int GetOperatorCount()
    {
        return 3 * VectorSize - 1;
    }
}