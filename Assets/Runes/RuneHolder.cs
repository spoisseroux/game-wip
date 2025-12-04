using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// have to figure out where we separate the act of generating a chant from the UI, 
// and then sequentially chanting
public class RuneHolder : MonoBehaviour
{
    // available runes
    [SerializeField]
    List<RuneDataSO> runes = new List<RuneDataSO>(4);

    // chanting
    public float chantRadius;

    [SerializeField]
    List<RuneDataSO> storedChant;

    // reaction
    public float reactRadius;

    // status effect factory
    StatusEffectFactory seFactory;

    // rune db
    [SerializeField]
    public RuneDatabaseLocator dbService;

    #region MonoBehaviour
    private void Awake()
    {
        seFactory = new StatusEffectFactory();
    }

    private void Update()
    {
        // m key for quick testing
        var keyboard = Keyboard.current;
        if (keyboard.mKey.wasPressedThisFrame)
        {
            Debug.Log("heyyy checking...");
            ExecuteChant(runes);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(this.transform.position, chantRadius);

        Gizmos.color = Color.bisque;
        Gizmos.DrawWireSphere(this.transform.position, reactRadius);
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
    public void ClearChant()
    {
        storedChant.Clear();
    }

    // large coroutine function for actually ~doin da chant?~
    public void ExecuteChant(List<RuneDataSO> runes)
    {
        storedChant = runes;
        // lock player controls, necessary?? could be a post game jam thing

        // compile chant sfx into singular sound 
        // List<AudioClip> audio = ChantRunes(storedChant);

        // animation/shaders?

        // activate the runes we have
        // ActivateRunes();

        // push chant to any nearby doors, let it react, return confirmation of whether it succeeded
        Collider[] cols = Physics.OverlapSphere(transform.position, chantRadius);
        foreach (var col in cols) {
            // check for chant reactors
            if (col.TryGetComponent<IChantReactor>(out IChantReactor cr)) {
                // package as just enums for chant reader
                cr.React(storedChant.Select(n => n.runeValue).ToList());
            }
        }

        // clear chant
        ClearChant();
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
            rune.activationCount++;
            ApplyRune(rune);

            foreach (var col in cols) {
                if (col.TryGetComponent<IRuneReactor>(out IRuneReactor rr)) {
                    rr.React(rune.runeValue);
                }
            }
        }
    }

    public void ApplyRune(RuneDataSO rune)
    {
        /*
        foreach (IStatusEffect se in rune.effects)
        {
            se.ApplyStatus(this.gameObject);
        }
        return;
        */
    }
    #endregion
}
