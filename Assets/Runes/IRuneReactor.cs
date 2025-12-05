/*
    General interface for any objects that will be affected by a single rune's activation, whether from afar or nearby

    Need to figure out the data structuring for pushing updates to damage/health of mobs, damage of traps, erratic-ness of terrain, etc.,
    that's based on a rune's number of activations in a given run
*/
#region Interface
// basically, every one of these should store the rune they react to via RegisterRune
using Unity.VisualScripting;
using UnityEngine;

public interface IRuneReactor
{
    public abstract void React(RuneType rune);
    public abstract void RegisterRune(RuneType rune);
}
#endregion

#region Specific Reactors
public class RuneReactor : MonoBehaviour, IRuneReactor
{
    // which type of rune do you react to
    [SerializeField]
    RuneType runeAlignment;

    // rune db
    [SerializeField]
    public RuneDatabaseLocator dbService;

    public void React(RuneType rune)
    {
        // check distance to player, maybe play a shader/sound or something when close?
        return;
    }

    public void RegisterRune(RuneType rune)
    {
        runeAlignment = rune;
    }
}

#endregion