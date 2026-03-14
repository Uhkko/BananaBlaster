using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermExpansion : Term
{
    private Term Term { get; }
    private int NewSize { get; }

    public override Element[] Children => [Term];
    protected override object[] Identifiers => [Term, NewSize];

    public override int VectorSize => NewSize;

    public TermExpansion(Term term, int newSize)
    {
        if (term.VectorSize > newSize)
            throw new ArgumentException("Failed to create element: New size can't be smaller than original size");
        
        Term = term;
        NewSize = newSize;
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var childReps = Term.GetRepresentatives(context);
        var selfReps = GetRepresentatives(context);
        
        for (int i = 0; i < Term.VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], childReps[i]);
        for (int i = Term.VectorSize; i < NewSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], z3Context.MkBool(false));
        
        return z3Context.MkAnd(bitConstraints);
    }
}