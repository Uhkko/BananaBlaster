using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermLShift(Term t1, Term t2) : Term
{
    private Term Term1 { get; } = t1;
    private Term Term2 { get; } = t2;
    
    public override Element[] Children => [Term1, Term2];
    protected override object[] Identifiers => [Term1, Term2];

    public override int VectorSize => Term1.VectorSize;
    
    private static Term LShift(Term t1, Term t2, int i)
    {
        if (i < 0)
            return t1;

        var concat = new TermConcat(
            LShift(t1, t2, i - 1),
            new TermRepeatConst(false, (int)Math.Pow(2, i))
        );
        return new TermITE(
            new AtomExtraction(t2, i),
            new TermExtraction(
                concat,
                0, t1.VectorSize - 1
            ),
            LShift(t1, t2, i - 1)
        );
    }
    
    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var selfReps = GetRepresentatives(context);
        
        var shiftTerm = LShift(Term1, Term2, Term2.VectorSize - 1);
        context.AddElement(shiftTerm);
        var shiftReps = shiftTerm.GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], shiftReps[i]);
        
        return z3Context.MkAnd(bitConstraints);
    }
}