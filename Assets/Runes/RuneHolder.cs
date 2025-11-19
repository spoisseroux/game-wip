using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// have to figure out where we separate the act of generating a chant from the UI, 
// and then sequentially chanting
public class RuneHolder : MonoBehaviour
{
    [Header("Debugging Purposes")]
    List<RuneDataSO> serializedRunes = new List<RuneDataSO>();
    // available runes
    List<IRune> runes = new List<IRune>();

    // chanting
    public float chantRadius;
    List<RuneType> storedChant;

    #region MonoBehaviour & Live Debug Helpers
    private void Awake()
    {
        SyncRunes();
    }

    private void OnValidate()
    {
        SyncRunes();
    }

    private void SyncRunes()
    {
        runes = serializedRunes.OfType<IRune>().ToList();
    }
    #endregion

    #region Rune Container Public API Methods
    public void BestowRune(RuneType runeEnum)
    {
        // get Rune from database
        IRune rune = null;

        // give user the associated state of the Rune

        // add to list
        runes.Add(rune);
    }

    public void AddChant(List<RuneType> runes)
    {
        return;
    }

    public void ExecuteChant()
    {

        foreach (RuneType rune in storedChant)
        {
            
        }
    }
    #endregion
}
