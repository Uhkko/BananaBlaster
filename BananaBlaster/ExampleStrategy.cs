using BananaBlaster.Formula;

namespace BananaBlaster;

public class ExampleStrategy : SelectionStrategy<object>
{
    private int _currentIteration;
    private List<Element> Atoms { get; } = [];
    private List<Element> Terms { get; } = [];

    protected override void SetUp(Object data)
    {
        _currentIteration = 0;
        Atoms.Clear();
        Terms.Clear();

        foreach (var element in Elements)
        {
            if (element is Atom)
                Atoms.Add(element);
            else if (element is Term)
                Terms.Add(element);
        }
    }

    public override List<Element> Next()
    {
        _currentIteration++;
        return _currentIteration switch
        {
            <= 1 => Atoms,
            2 => Terms,
            _ => []
        };
    }

    public override bool HasNext()
    {
        return _currentIteration <= 1;
    }
}