using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class AtomExtraction : Atom
{
    private Term Term { get; }
    private int Index { get; }
    
    public override Element[] Children => [Term];
    protected override object[] Identifiers => [Term, Index];

    public AtomExtraction(Term term, int index)
    {
        if (term.VectorSize <= index)
            throw new ArgumentException("Failed to create element: Index is outside the bounds of the term's vector size");

        Term = term;
        Index = index;
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var childReps = Term.GetRepresentatives(context);
        var selfReps = GetRepresentative(context);
        return context.Z3Context.MkEq(selfReps, childReps[Index]);
    }
    
    public override int GetOperatorCount()
    {
        return 1;
    }
}