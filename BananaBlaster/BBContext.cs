using BananaBlaster.Formula;
using Microsoft.Z3;

namespace BananaBlaster;

public class BBContext
{
    private Function Formula { get; }
    public Context Z3Context { get; }
    private Solver Solver { get; }
    
    private BoolExpr Skeleton { get; }
    private List<BoolExpr> Constraints { get; }

    private Dictionary<Element, bool> Elements { get; }
        
    private BBContext(Function formula)
    {
        Formula = formula;
        Z3Context = new Context();
        Solver = Z3Context.MkSolver();

        Skeleton = Formula.CreateConstraint(this);
        Constraints = [];

        Elements = FindFormulaElements(Formula);
    }
    
    public void AddElement(Element element)
    {
        var allFormulaElements = FindFormulaElements(element);

        foreach (var formulaElement in allFormulaElements.Keys)
        {
            Elements[formulaElement] = false;
            AddConstraint(formulaElement);
        }
    }

    private void AddConstraint(Element element)
    {
        if (!Elements.TryGetValue(element, out var isConstrained) || isConstrained)
            return;
        
        var constraint = element.CreateConstraint(this);
        Constraints.Add(constraint);
        Elements[element] = true;
    }

    private Status Solve()
    {
        Solver.Reset();
        Solver.Add(Skeleton);
        Solver.Add(Constraints);
        return Solver.Check();
    }
    
    public static Status Solve(Function bvFunc)
    {
        var context = new BBContext(bvFunc);

        foreach (var element in context.Elements.Keys)
            context.AddConstraint(element);

        return context.Solve();
    }
    
    public static Status SolveIncremental<T>(Function bvFunc) where T : SelectionStrategy, new()
    {
        var context = new BBContext(bvFunc);
        var strategy = SelectionStrategy.Create<T>(context.Elements.Keys.ToList());

        while (true)
        {
            var result = context.Solve();
            if (result == Status.UNSATISFIABLE || !strategy.HasNext())
                return result;
            
            var next = strategy.Next();
            foreach (var element in next)
                context.AddConstraint(element);
        }
    }
    
    private Dictionary<Element, bool> FindFormulaElements(Element root)
    {
        var elements = new Dictionary<Element, bool>();
        var parentsToCheck = new Stack<Element>();
        parentsToCheck.Push(root);
        
        while (parentsToCheck.Count > 0)
        {
            var parent = parentsToCheck.Pop();
            if (parent is not Function)
                elements.Add(parent, false);
            
            foreach (var child in parent.Children)
            {
                if (elements.ContainsKey(child))
                    throw new Exception("Infinite loop in formula detected");
                
                parentsToCheck.Push(child);
            }
        }
        return elements;
    }
}