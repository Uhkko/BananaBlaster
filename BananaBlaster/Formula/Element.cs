using Microsoft.Z3;

namespace BananaBlaster.Formula;

public abstract class Element
{
    public abstract Element[] Children { get; }
    
    public abstract BoolExpr CreateConstraint(BBContext context);
}