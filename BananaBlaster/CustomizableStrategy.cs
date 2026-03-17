using BananaBlaster.Formula;
using BananaBlaster.Formula.Elements;

namespace BananaBlaster;

public sealed class CustomizableStrategy : SelectionStrategy<List<Type>>
{
    private int _currentIteration;
    private readonly List<List<Element>> Buckets = [];
    private List<Type> Types = [];

    protected override void SetUp(List<Type> types)
    {
        _currentIteration = 0;

        Types = types;

        foreach(var type in types)
        {
            Console.Write(type);
            Console.Write(',');
        }
        Console.WriteLine();

        Buckets.Clear();
        foreach(var _ in Types) Buckets.Add([]);

        foreach (var element in Elements)
        {
            for(int i = 0; i < Types.Count; ++i)
            {
                if(!Types[i].IsInstanceOfType(element)) {
                    continue;
                }

                Buckets[i].Add(element);
            }
        }
    }

    public override bool HasNext()
    {
        return _currentIteration < Buckets.Count;
    }

    public override List<Element> Next()
    {
        return Buckets[_currentIteration++];
    }

    public static List<Type> ParseStragegy(string strategy)
    {
        List<Type> types = [];

        foreach(var item in strategy.Split(','))
        {
            types.Add(
                item.ToLower() switch {
                    // Formula
                    "atom" => typeof(Atom),
                    "element" => typeof(Element),
                    "function" => typeof(Function),
                    "term" => typeof(Term),
                    // Elements
                    // Atom
                    "atomconst" => typeof(AtomConst),
                    "atomequals" => typeof(AtomEquals),
                    "atomextraction" => typeof(AtomExtraction),
                    "atomgreater" => typeof(AtomGreater),
                    "atomidentifier" => typeof(AtomIdentifier),
                    "atomless" => typeof(AtomLess),
                    // Func
                    "funcand" => typeof(FuncAnd),
                    "funcatom" => typeof(FuncAtom),
                    "funcequal" => typeof(FuncEqual),
                    "funcimplication" => typeof(FuncImplication),
                    "funcnot" => typeof(FuncNot),
                    "funcor" => typeof(FuncOr),
                    // Term
                    "termand" => typeof(TermAnd),
                    "termconcat" => typeof(TermConcat),
                    "termconst" => typeof(TermConst),
                    "termexpansion" => typeof(TermExpansion),
                    "termextraction" => typeof(TermExtraction),
                    "termidentifier" => typeof(TermIdentifier),
                    "termite" => typeof(TermITE),
                    "termlshift" => typeof(TermLShift),
                    "termlshiftconst" => typeof(TermLShiftConst),
                    "termmultiplication" => typeof(TermMultiplication),
                    "termmultiplicationoverflow" => typeof(TermMultiplicationOverflow),
                    "termnot" => typeof(TermNot),
                    "termor" => typeof(TermOr),
                    "termrepeat" => typeof(TermRepeat),
                    "termrepeatconst" => typeof(TermRepeatConst),
                    "termrshift" => typeof(TermRShift),
                    "termrshiftconst" => typeof(TermRShiftConst),
                    "termsubtraction" => typeof(TermSubtraction),
                    "termsum" => typeof(TermSum),
                    "termsumoverflow" => typeof(TermSumOverflow),

                    _ => throw new Exception($"Could not parse element {item}"),
                }
            );
        }

        return types;
    }
}
