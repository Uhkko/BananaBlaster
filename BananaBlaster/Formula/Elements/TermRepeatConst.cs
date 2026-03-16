using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermRepeatConst : Term
{
    private bool Value { get; }
    private int Count { get; }

    public override Element[] Children => [];
    protected override object[] Identifiers => [Value, Count];

    public override int VectorSize => Count;

    public TermRepeatConst(bool value, int count)
    {
        if (count < 0)
            throw new ArgumentException("Failed to create element: Count can't be smaller than 0");
        
        Value = value;
        Count = count;
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var selfReps = GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
            bitConstraints[i] = z3Context.MkEq(selfReps[i], z3Context.MkBool(Value));
        
        return z3Context.MkAnd(bitConstraints);
    }
    
    public override int GetOperatorCount()
    {
        return 3 * VectorSize - 1;
    }
}