using System.Diagnostics;
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

    public BBDiagnostics Diagnostics { get; }
        
    private BBContext(Function formula)
    {
        Formula = formula;
        Z3Context = new Context();
        Solver = Z3Context.MkSolver();

        Skeleton = Formula.CreateConstraint(this);
        Constraints = [];

        Elements = FindFormulaElements(Formula);

        Diagnostics = new BBDiagnostics();
    }
    
    public void AddElement(Element element)
    {
        var allFormulaElements = FindFormulaElements(element);

        foreach (var formulaElement in allFormulaElements.Keys)
        {
            if (!Elements.TryAdd(formulaElement, false))
                continue;
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

    private SolverResult Solve()
    {
        Solver.Reset();
        Solver.Add(Skeleton);
        Solver.Add(Constraints);

        return new SolverResult(Solver, this.Diagnostics);
    }
    
    public static SolverResult Solve(Function bvFunc)
    {
        var context = new BBContext(bvFunc);

        foreach (var element in context.Elements.Keys.ToList())
            context.AddConstraint(element);

        return context.Solve();
    }
    
    public static SolverResult SolveIncremental<T>(Function bvFunc) where T : SelectionStrategy, new()
    {
        var context = new BBContext(bvFunc);
        var strategy = SelectionStrategy.Create<T>(context.Elements.Keys.ToList());

        while (true)
        {
            var result = context.Solve();
            if (result.Status == Status.UNSATISFIABLE || !strategy.HasNext())
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
                elements.TryAdd(parent, false);
            
            foreach (var child in parent.Children)
                parentsToCheck.Push(child);
        }
        return elements;
    }
}

public class BBDiagnostics {
    public Dictionary<string, BoolExpr> AtomIdentifiers { get; } = [];
    public Dictionary<string, BoolExpr[]> TermIdentifiers { get; } = [];

    public Dictionary<string, bool> GetAtomValues(Model model)
    {
        Dictionary<string, bool> result = [];

        foreach(var elem in AtomIdentifiers)
        {
            var interpretation = model.ConstInterp(elem.Value);
            if(!interpretation.IsBool) throw new UnreachableException("The interpretation is always a boolean.");

            result.Add(elem.Key, interpretation.BoolValue > 0); // TODO: There is also = 0 (undecided)
        }

        return result;
    }

    public Dictionary<string, int> GetTermValues(Model model)
    {
        Dictionary<string, int> result = [];

        foreach(var elem in TermIdentifiers)
        {
            int value = 0;
            for(int i = 0; i < elem.Value.Length; ++i)
            {
                var interpretation = model.ConstInterp(elem.Value[i]);
                if(!interpretation.IsBool) throw new UnreachableException("The interpretation is always a boolean.");

                value |= Convert.ToInt32(interpretation.BoolValue > 0) << i;
            }

            result.Add(elem.Key, value);
        }

        return result;
    }
}

public readonly struct SolverResult(Solver solver, BBDiagnostics diagnostics) {
    public Status Status { get; } = solver.Check();

    private Model? Model { get; } = solver.Model;

    public readonly string SMTLibCode {
        get {
            return solver.ToString();
        }
    }

    public Dictionary<string, bool> GetAtomValues()
    {
        return diagnostics.GetAtomValues(Model ?? throw new ArgumentException("This function cannot be called if there is no model."));
    }

    public Dictionary<string, int> GetTermValues()
    {
        return diagnostics.GetTermValues(Model ?? throw new ArgumentException("This function cannot be called if there is no model."));
    }
}
