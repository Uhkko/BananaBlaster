using BananaBlaster.Formula;

namespace BananaBlaster;

public abstract class SelectionStrategy<O>
{
    protected List<Element> Elements { get; private init; } = [];

    protected abstract void SetUp(O data);
    public abstract List<Element> Next();
    public abstract bool HasNext();

    public static SelectionStrategy<O> Create<T>(List<Element> elements, O data) where T: SelectionStrategy<O>, new()
    {
        var strategy = new T
        {
            Elements = elements
        };
        strategy.SetUp(data);
        return strategy;
    }
}