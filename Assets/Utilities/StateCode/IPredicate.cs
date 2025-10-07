using System;

/*
Predicates just evaluate a kind of condition and respond True or False
*/
public interface IPredicate
{
    bool Evaluate();
}

// Evaluates a condition with a simple function
public class FuncPredicate : IPredicate
{
    readonly Func<bool> eval;

    public FuncPredicate(Func<bool> f)
    {
        this.eval = f;
    }

    public bool Evaluate() => eval.Invoke();
}
