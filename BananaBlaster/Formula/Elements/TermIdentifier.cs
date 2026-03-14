using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermIdentifier(string name, int size) : Term
{
    private string Name { get; } = name;
    
    public override Element[] Children => [];
    protected override object[] Identifiers => [Name, VectorSize];

    public override int VectorSize { get; } = size;

    public override BoolExpr CreateConstraint(BBContext context)
    {
        return context.Z3Context.MkBool(true);
    }
}