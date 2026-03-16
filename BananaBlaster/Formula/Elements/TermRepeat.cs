using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermRepeat : Term
{
    private Term Term { get; }
    private int Index { get; }
    private int Count { get; }

    public override Element[] Children => [Term];
    protected override object[] Identifiers => [Term, Index, Count];

    public override int VectorSize => Count;

    public TermRepeat(Term term, int index, int count)
    {
        if (count < 0)
            throw new ArgumentException("Failed to create element: Count can't be smaller than 0");
        if (term.VectorSize <= index)
            throw new ArgumentException("Failed to create element: Index is outside the bounds of the term's vector size");
        
        Term = term;
        Index = index;
        Count = count;
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var childReps = Term.GetRepresentatives(context);
        var selfReps = GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], childReps[Index]);
        
        return z3Context.MkAnd(bitConstraints);
    }
    
    public override int GetOperatorCount()
    {
        return 2 * VectorSize - 1;
    }
}