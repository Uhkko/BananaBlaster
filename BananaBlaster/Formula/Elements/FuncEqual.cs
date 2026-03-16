using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class FuncEqual(Function f1, Function f2) : Function
{
    private Function Func1 { get; } = f1;
    private Function Func2 { get; } = f2;

    public override Element[] Children => [Func1, Func2];
    protected override object[] Identifiers => [Func1, Func2];

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var childSkeleton1 = Func1.CreateConstraint(context);
        var childSkeleton2 = Func2.CreateConstraint(context);
        
        return context.Z3Context.MkEq(childSkeleton1, childSkeleton2);
    }
    
    public override int GetOperatorCount()
    {
        return 1 + Func1.GetOperatorCount() + Func2.GetOperatorCount();
    }
    
    public override int GetVariableCount()
    {
        return Func1.GetVariableCount() + Func2.GetVariableCount();
    }
}