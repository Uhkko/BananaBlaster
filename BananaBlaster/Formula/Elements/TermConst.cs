using System.Collections;
using Microsoft.Z3;

namespace BananaBlaster.Formula.Elements;

public class TermConst(BitArray bits) : Term
{
    private BitArray Bits { get; } = bits;

    public override Element[] Children => [];
    protected override object[] Identifiers => Bits.Cast<object>().ToArray();

    public override int VectorSize => Bits.Length;
    
    public override BoolExpr CreateConstraint(BBContext context)
    {
        var z3Context = context.Z3Context;
        var bitConstraints = new BoolExpr[VectorSize];
        var selfReps = GetRepresentatives(context);
        
        for (int i = 0; i < VectorSize; i++)
        {
            if (Bits[i])
                bitConstraints[i] = selfReps[i];
            else
                bitConstraints[i] = z3Context.MkNot(selfReps[i]);
        }
        
        return z3Context.MkAnd(bitConstraints);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var b in Bits)
            hash.Add(b);
        hash.Add(typeof(TermConst).GetHashCode());

        return Math.Abs(hash.ToHashCode());
    }

    public static TermConst From(long value, int length) {
        var bits = new BitArray(length);

        for(int i = 0; i < length; ++i) {
            bits[i] = ((value >> i) & 1) > 0;
        }

        return new TermConst(bits);
    }
}
