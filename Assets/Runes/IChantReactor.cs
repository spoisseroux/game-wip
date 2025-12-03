using System.Collections.Generic;
using UnityEngine;

/*
    Reserved for things like Doors which specifically react to a sequence of runes, and would generate an interaction as a result
*/
public interface IChantReactor
{
    public virtual void React(List<RuneType> runes) { return; }
}
