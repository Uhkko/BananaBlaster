using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermMultiplicationOverflow : Term
{
    private Term Term1 { get; }
    private Term Term2 { get; }

    public override Element[] Children => [Term1, Term2];
    protected override object[] Identifiers => [Term1, Term2];

    public override int VectorSize => Term1.VectorSize;
    
    public TermMultiplicationOverflow(Term t1, Term t2)
    {
        if (t1.VectorSize != t2.VectorSize)
            throw new ArgumentException($"Failed to create element: Terms have different vector sizes ({t1.VectorSize} vs. {t2.VectorSize})");
        
        Term1 = t1;
        Term2 = t2;
    }

    private static Term Mul(Term t1, Term t2, int i)
    {
        if (i <= 0)
        {
            return new TermITE(
                new AtomExtraction(t2, i),
                t1,
                new TermRepeatConst(false, t1.VectorSize)
            );
        }
        
        return new TermSumOverflow(
            new TermITE(
                new AtomExtraction(t2, i),
                new TermLShiftConstOverflow(t1, i),
                new TermRepeatConst(false, t1.VectorSize)
            ),
            Mul(t1, t2, i - 1)
        );
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var selfReps = GetRepresentatives(context);
        
        var mulTerm = Mul(Term1, Term2, Term2.VectorSize - 1);
        context.AddElement(mulTerm);
        var mulReps = mulTerm.GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], mulReps[i]);
        
        return z3Context.MkAnd(bitConstraints);
    }
    
    public override int GetOperatorCount()
    {
        return 2 * VectorSize - 1;
    }
}