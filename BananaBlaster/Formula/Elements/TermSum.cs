using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermSum : Term
{
    public Term Term1 { get; }
    public Term Term2 { get; }

    public override Element[] Children => [Term1, Term2];
    protected override object[] Identifiers => [Term1, Term2];

    public override int VectorSize => Term1.VectorSize + 1;
    
    public TermSum(Term t1, Term t2)
    {
        if (t1.VectorSize != t2.VectorSize)
            throw new ArgumentException($"Failed to create element: Terms have different vector sizes ({t1.VectorSize} vs. {t2.VectorSize})");
        
        Term1 = t1;
        Term2 = t2;
    }

    private static BoolExpr Sum(Context context, BoolExpr[] reps1, BoolExpr[] reps2, BoolExpr[] c, int i)
    {
        return context.MkXor(reps1[i], context.MkXor(reps2[i], c[i]));
    }
    
    private static BoolExpr Cout(Context context, BoolExpr[] reps1, BoolExpr[] reps2, BoolExpr[] c, int i)
    {
        return context.MkOr(
            context.MkAnd(reps1[i], reps2[i]),
            context.MkAnd(
                context.MkXor(reps1[i], reps2[i]),
                c[i]                
            )
        );
    }

    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new List<BoolExpr>();
        var childReps1 = Term1.GetRepresentatives(context);
        var childReps2 = Term2.GetRepresentatives(context);
        var selfReps = GetRepresentatives(context);
        
        var cOuts = new BoolExpr[VectorSize];
        var hash = GetHashCode();
        for (var i = 0; i < VectorSize; i++)
            cOuts[i] = z3Context.MkBoolConst(z3Context.MkSymbol(hash + VectorSize + i));
        
        for (int i = 0; i < VectorSize - 1; i++)
            bitConstraints.Add(z3Context.MkEq(selfReps[i], Sum(z3Context, childReps1, childReps2, cOuts, i)));
        for (int i = 0; i < VectorSize - 1; i++)
            bitConstraints.Add(z3Context.MkEq(cOuts[i + 1], Cout(z3Context, childReps1, childReps2, cOuts, i)));
        
        bitConstraints.Add(z3Context.MkEq(cOuts[0], z3Context.MkBool(false)));
        bitConstraints.Add(z3Context.MkEq(selfReps[VectorSize - 1], cOuts[VectorSize - 1]));
        
        return z3Context.MkAnd(bitConstraints);
    }
    
    public override int GetOperatorCount()
    {
        return 9 * VectorSize - 6;
    }

    public override int GetVariableCount()
    {
        return 2 * VectorSize;
    }
}