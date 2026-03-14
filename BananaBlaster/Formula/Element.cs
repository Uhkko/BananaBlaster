using Microsoft.Z3;

namespace BananaBlaster.Formula;

public abstract class Element
{
    public abstract Element[] Children { get; }
    protected abstract object[] Identifiers { get; }
    
    public abstract BoolExpr CreateConstraint(BBContext context);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var identifier in Identifiers)
            hash.Add(identifier);
        hash.Add(GetType());
        return Math.Abs(hash.ToHashCode());
    }
}