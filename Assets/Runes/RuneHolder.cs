using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// have to figure out where we separate the act of generating a chant from the UI, 
// and then sequentially chanting
public class RuneHolder : MonoBehaviour
{
    [SerializeField]
    List<RuneDataSO> debugRunes = new List<RuneDataSO>(4);

    // available runes
    [SerializeField]
    List<RuneType> runes = new List<RuneType>(4);

    // chanting
    public float chantRadius;
    List<RuneType> storedChant;

    // rune db
    [SerializeField]
    public RuneDatabaseLocator dbService;

    #region MonoBehaviour
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
        /*
        foreach (RuneDataSO r in debugRunes)
        {
            runes.Add(r.runeValue);
        }
        */
    }
    #endregion

    #region Rune Container Additions
    // receive a rune from Altar
    public void BestowRune(RuneType runeEnum)
    {
        // add to list
        if (!runes.Contains(runeEnum))
            runes.Add(runeEnum);

        // get Rune from database
        IRune rune = (IRune)dbService.database.GetRune(runeEnum);

        // give user the associated state of the Rune
    }
    #endregion

    #region Chanting
    // receive a chant from UI, .....
    public void AddChant(List<RuneType> runes)
    {
        storedChant = runes;
        return;
    }

    // clear chant from data
    public void ClearChant(List<RuneType> runes)
    {
        storedChant.Clear();
    }

    // large coroutine function for actually ~doin da chant?~
    public void ExecuteChant()
    {
        // activate each rune in backend
        foreach (RuneType runeEnum in storedChant)
        {
            // map from runedatabaseSO
            IRune rune = (IRune)dbService.database.GetRune(runeEnum);
            rune.Activate();
        }
    }
    #endregion
}
