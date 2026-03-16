using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class AtomLess : Atom
{
    private Term Term1 { get; }
    private Term Term2 { get; }
    
    public override Element[] Children => [Term1, Term2];
    protected override object[] Identifiers => [Term1, Term2];

    public AtomLess(Term t1, Term t2)
    {
        if (t1.VectorSize != t2.VectorSize)
            throw new ArgumentException($"Failed to create element: Terms have different vector sizes ({t1.VectorSize} vs. {t2.VectorSize})");
        
        Term1 = t1;
        Term2 = t2;
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var selfRep = GetRepresentative(context);

        var subAtom = new AtomExtraction( // (b_n - a_n)[n]
            new TermSubtraction( // b_n - a_n
                new TermExpansion(Term2, Term2.VectorSize + 1), // b_n
                new TermExpansion(Term1, Term1.VectorSize + 1)  // a_n
            ),
            Term1.VectorSize // n
        );
        context.AddElement(subAtom);
        var subRep = subAtom.GetRepresentative(context);
        
        return z3Context.MkEq(selfRep, subRep);
    }

    public override int GetOperatorCount()
    {
        return 1;
    }
}