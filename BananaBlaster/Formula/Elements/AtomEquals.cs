using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class AtomEquals : Atom
{
    private Term Term1 { get; }
    private Term Term2 { get; }
    
    public override Element[] Children => [Term1, Term2];
    protected override object[] Identifiers => [Term1, Term2];

    public AtomEquals(Term t1, Term t2)
    {
        if (t1.VectorSize != t2.VectorSize)
            throw new ArgumentException($"Failed to create element: Terms have different vector sizes ({t1.VectorSize} vs. {t2.VectorSize})");
        
        Term1 = t1;
        Term2 = t2;
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[Term1.VectorSize];
        var childReps1 = Term1.GetRepresentatives(context);
        var childReps2 = Term2.GetRepresentatives(context);
        
        for (int i = 0; i < bitConstraints.Length; i++)
            bitConstraints[i] = z3Context.MkEq(childReps1[i], childReps2[i]);
        
        var childConstraint = z3Context.MkAnd(bitConstraints);
        var selfRep = GetRepresentative(context);
        return z3Context.MkEq(selfRep, childConstraint);
    }
    
    public override int GetOperatorCount()
    {
        return 2 * Term1.VectorSize;
    }
}