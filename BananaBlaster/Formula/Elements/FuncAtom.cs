using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class FuncAtom(Atom atom) : Function
{
    private Atom Atom { get; } = atom;
    
    public override Element[] Children => [Atom];
    protected override object[] Identifiers => [Atom];

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var atomRep = Atom.GetRepresentative(context);
        return atomRep;
    }
}
