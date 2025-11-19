using System.Collections.Generic;
using UnityEngine;

public interface IChantReactor
{
    public virtual void React(List<RuneType> runes) { return; }
    public virtual void React(RuneType rune) { return; }
    

    // other things needed -->
        // generic cutscene generator for doors
        // delta stores in scriptableobjects for affecting/applying data modifications to rune-aligned mobs, platforms, etc.
        // a way to subscribe and receive when a rune reacts
}
