using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermLShiftConstOverflow(Term term, int amount) : Term
{
    private Term Term { get; } = term;
    private int Amount { get; } = amount;
    
    public override Element[] Children => [Term];
    protected override object[] Identifiers => [Term, Amount];

    public override int VectorSize => Term.VectorSize;

    private static Term LShift(Term t1, int i)
    {
        if (i <= 0)
            return t1;
        
        return new TermConcat(
            t1,
            new TermRepeatConst(false, i)
        );
    }
    
    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var selfReps = GetRepresentatives(context);
        
        var shiftTerm = LShift(Term, Amount);
        context.AddElement(shiftTerm);
        var shiftReps = shiftTerm.GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], shiftReps[i]);
        
        return z3Context.MkAnd(bitConstraints);
    }
}