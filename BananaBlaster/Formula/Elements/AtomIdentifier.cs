using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class AtomIdentifier(string name) : Atom
{
    private string Name { get; } = name;
    
    public override Element[] Children => [];
    protected override object[] Identifiers => [Name];

    public override BoolExpr CreateConstraint(BBContext context)
    {
        context.Diagnostics.AtomIdentifiers.TryAdd(Name, GetRepresentative(context));

        return context.Z3Context.MkBool(true);
    }
    
    public override int GetOperatorCount()
    {
        return 0;
    }
}