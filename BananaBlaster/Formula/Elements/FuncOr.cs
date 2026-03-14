using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class FuncOr(Function f1, Function f2) : Function
{
    private Function Func1 { get; } = f1;
    private Function Func2 { get; } = f2;

    public override Element[] Children => [Func1, Func2];
    protected override object[] Identifiers => [Func1, Func2];

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var childSkeleton1 = Func1.CreateConstraint(context);
        var childSkeleton2 = Func2.CreateConstraint(context);
        
        return context.Z3Context.MkOr(childSkeleton1, childSkeleton2);
    }
}