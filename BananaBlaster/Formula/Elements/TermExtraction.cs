using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermExtraction : Term
{
    private Term Term { get; }
    private int StartIndex { get; }
    private int EndIndex { get; }

    public override Element[] Children => [Term];
    protected override object[] Identifiers => [Term, StartIndex, EndIndex];

    public override int VectorSize => EndIndex - StartIndex + 1;
    
    public TermExtraction(Term term, int startIndex, int endIndex)
    {
        if (endIndex < startIndex)
            throw new ArgumentException("Failed to create element: End index can't be less than start index");
        if (startIndex < 0)
            throw new ArgumentException("Failed to create element: Start index must be larger than zero");
        if (term.VectorSize <= endIndex)
            throw new ArgumentException("Failed to create element: End index out of the bounds of the term's vector size");
        
        Term = term;
        StartIndex = startIndex;
        EndIndex = endIndex;
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var childReps = Term.GetRepresentatives(context);
        var selfReps = GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], childReps[i + StartIndex]);
        
        return z3Context.MkAnd(bitConstraints);
    }
    
    public override int GetOperatorCount()
    {
        return 2 * VectorSize - 1;
    }
}