using System.Collections.Generic;
using UnityEngine;

// other things needed -->
        // generic cutscene generator for doors
        // delta stores in scriptableobjects for affecting/applying data modifications to rune-aligned mobs, platforms, etc.
        // a way to subscribe and receive when a rune reacts

/*
    Reserved for things like Doors which specifically react to a sequence of runes, and would generate an interaction as a result
*/
public interface IChantReactor
{
    public virtual void React(List<RuneType> runes) { return; }
}
