using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// have to figure out where we separate the act of generating a chant from the UI, 
// and then sequentially chanting
public class RuneHolder : MonoBehaviour
{
    // available runes
    [SerializeField]
    List<RuneDataSO> runes = new List<RuneDataSO>(4);

    // chanting
    public float chantRadius;
    List<RuneDataSO> storedChant;

    // reaction
    public float reactRadius;

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
    public void BestowRune(RuneDataSO rune)
    {
        // add to list
        if (!runes.Contains(rune))
            runes.Add(rune);
    }
    #endregion

    #region Chanting
    // receive a chant from UI, .....
    public void AddChant(List<RuneDataSO> runes)
    {
        storedChant = runes;
        return;
    }

    // clear chant from data
    public void ClearChant()
    {
        storedChant.Clear();
    }

    // large coroutine function for actually ~doin da chant?~
    public void ExecuteChant()
    {
        // lock player controls, necessary?? could be a post game jam thing

        // compile chant sfx into singular sound 
        List<AudioClip> audio = ChantRunes(storedChant);

        // animation/shaders?

        // activate the runes we have
        ActivateRunes();

        // push chant to any nearby doors, let it react, return confirmation of whether it succeeded
        Collider[] cols = Physics.OverlapSphere(transform.position, chantRadius);
        foreach (var col in cols) {
            // check for chant reactors
            if (col.TryGetComponent<IChantReactor>(out IChantReactor cr)) {
                // package as just enums for chant reader
                cr.React(storedChant.Select(n => n.runeValue).ToList());
            }
        }
    }

    public List<AudioClip> ChantRunes(List<RuneDataSO> runes)
    {
        List<AudioClip> clips = new List<AudioClip>();
        foreach (RuneDataSO rune in runes)
        {
            clips.Add(rune.soundFX);
        }
        return clips;
    }
    #endregion

    #region Activating Runes
    public void ActivateRunes()
    {
        // get all concerned columns
        Collider[] cols = Physics.OverlapSphere(transform.position, reactRadius);
        // check for rune reactors
        foreach (RuneDataSO rune in storedChant) {
            foreach (var col in cols) {
                if (col.TryGetComponent<IRuneReactor>(out IRuneReactor rr)) {
                    rr.React(rune.runeValue);
                }
            }
        }
    }

    public void ApplyRune(RuneDataSO rune)
    {
        foreach (IStatusEffect se in rune.effects)
        {
            
        }
        return;
    }
    #endregion
}
