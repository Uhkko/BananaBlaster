using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class AtomConst(bool value) : Atom
{
    private bool Value { get; } = value;
    
    public override Element[] Children => [];
    protected override object[] Identifiers => [Value];

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var selfRep = GetRepresentative(context);

        return Value ? selfRep : z3Context.MkNot(selfRep);
    }

    public override int GetOperatorCount()
    {
        return 1;
    }
}