using System;
using System.Collections.Generic;
using UnityEngine;
/*
StateMachines just exist as containers for State objects, and only maintain and transition between them.
They do not function on their own.

State's individually can be initialized with valid transitions, which are checked in Transition, and then reacted to accordingly
*/
public class StateMachine
{
    StateNode current;
    Dictionary<Type, StateNode> nodes = new(); /* i.e. Neutral, StateNode(neutral, transitions) */
    HashSet<ITransition> anyTransitions = new();

    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
        {
            Transition(transition.to);
        }

        current.state?.Update();
    }

    /*
    public void FixedUpdate()
    {
        current.state?.FixedUpdate();
    }
    */

    public void SetState(IState state)
    {
        //Debug.Log("setting state to: " + state.ToString());
        current = nodes[state.GetType()];
        current.state?.Enter();
    }

    public void Transition(IState newState)
    {
        if (current.state == newState) return;

        var prevState = current.state;
        var nextState = nodes[newState.GetType()].state;

        //Debug.Log("exiting: " + prevState.ToString());
        prevState?.Exit();
        //Debug.Log("entering: " + nextState.ToString());
        nextState?.Enter();

        current = nodes[newState.GetType()];
    }

    ITransition GetTransition()
    {
        // check anyTransitions first
        foreach (var transition in anyTransitions)
            if (transition.condition.Evaluate())
                return transition;

        // check from-to transitions
        foreach (var transition in current.transitions)
            if (transition.condition.Evaluate())
                return transition;

        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate predicate)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).state, predicate);
    }

    public void AddAnyTransition(IState to, IPredicate predicate)
    {
        anyTransitions.Add(new Transition(GetOrAddNode(to).state, predicate));
    }

    StateNode GetOrAddNode(IState state)
    {
        var node = nodes.GetValueOrDefault(state.GetType());
        if (node == null)
        {
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }

        return node;
    }

    public IState GetCurrentState()
    {
        return current.state;
    }


    class StateNode
    {
        public IState state;
        public HashSet<ITransition> transitions { get; }

        public StateNode(IState stateIn)
        {
            state = stateIn;
            transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState to, IPredicate predicate)
        {
            transitions.Add(new Transition(to, predicate));
        }
    }
}
