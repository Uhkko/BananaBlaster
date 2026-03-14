using BananaBlaster.Formula;

namespace BananaBlaster;

public abstract class SelectionStrategy
{
    protected List<Element> Elements { get; private init; } = [];

    protected abstract void SetUp();
    public abstract List<Element> Next();
    public abstract bool HasNext();

    public static SelectionStrategy Create<T>(List<Element> elements) where T: SelectionStrategy, new()
    {
        var strategy = new T
        {
            Elements = elements
        };
        strategy.SetUp();
        return strategy;
    }
}