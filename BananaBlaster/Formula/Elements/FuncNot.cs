using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class FuncNot(Function func) : Function
{
    private Function Func { get; } = func;

    public override Element[] Children => [Func];
    protected override object[] Identifiers => [Func];

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var childSkeleton = Func.CreateConstraint(context);
        return context.Z3Context.MkNot(childSkeleton);
    }
}