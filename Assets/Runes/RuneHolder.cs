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

    // position adjustment
    public Vector3 Yadjustment;

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

        Yadjustment = new Vector3(0.0f, 1.0f, 0.0f);
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
        Gizmos.DrawWireSphere(this.transform.position + Yadjustment, chantRadius);

        Gizmos.color = Color.bisque;
        Gizmos.DrawWireSphere(this.transform.position + Yadjustment, reactRadius);
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
        Collider[] cols = Physics.OverlapSphere(transform.position + Yadjustment, chantRadius);
        foreach (var col in cols) {
            // filter for objects only ABOVE the player, no reaching below the level
            /* 
                halfspherenormal = player.transform.up; or Vector3.up...
                dirToCollider = (col.transform.position - transform.position).normalized;
                if Vector3.Dot(dirToCollider, halfspherenormal) > 0;
                    yeah we want it 



                OKAY LINEAR ALGEBRA LESSON

                SO

                the collider, in world space, is centered at P

                but also, it can be represented as a series of displacements from (0,0,0)
                    - 1) displace to Vector3 player.transform
                    - 2) displace to Collider
                    - 3) Wa-Lah! We've displaced ourselves towards P

                now,,,,, we get the raw displacement vector of 2) by UNDOING the displacement from 1)
                so, if we need 2) but technically have 1) && 3), we just subtract 1 from 3
                then! we have 2) the displacement to the collider, from our origin point...!
                if you want direction, just normalize it!

                NOTE if we reversed this, then it'd be the direction from collider to the player!!!


                numerically... collider is at (9,3,5) for example...
                and our player is at (-3,0,5)... the vector from the origin (0,0,0) is defined by C - P
                (9,3,5) - (-3,0,5) = (12,3,0)... 
                (12,3,0)... that's the displacement needed to get to the collider if we start at our player, given our reference is (0,0,0)
                NORM((12,3,0)) == direction from player to collider! 

                Notice that the vector subtraction (p - c) yields a vector from c to p
            */

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
