using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermRShiftConst : Term
{
    private Term Term { get; }
    private int Amount { get; }
    
    public override Element[] Children => [Term];
    protected override object[] Identifiers => [Term, Amount];

    public override int VectorSize => Term.VectorSize;

    public TermRShiftConst(Term term, int amount)
    {
        Term = term;
        Amount = amount;
    }
    
    private static Term RShift(Term t1, int i)
    {
        if (i <= 0)
            return t1;
        if (i >= t1.VectorSize)
            return new TermRepeatConst(false, t1.VectorSize);            

        return new TermConcat(
            new TermRepeatConst(false, i),
            new TermExtraction(
                t1,
                i, t1.VectorSize - 1
            )
        );
    }
    
    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var selfReps = GetRepresentatives(context);
        
        var shiftTerm = RShift(Term, Amount);
        context.AddElement(shiftTerm);
        var shiftReps = shiftTerm.GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], shiftReps[i]);
        
        return z3Context.MkAnd(bitConstraints);
    }
}