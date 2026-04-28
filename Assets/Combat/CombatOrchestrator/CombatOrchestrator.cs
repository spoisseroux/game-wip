using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

/*
    OK, next steps:
        1. Null handling less awkward. Passing around all these nulled scriptable objects is going to crash the game inevitably
           There needs to be a NoneType object equivalent, have the game fail a bit more gracefully
        
        2. Using ScriptableObjects as stored objects for data retrieval and passing around instances of them feels a bit off
           Might be a good idea to confirm whether that's the best way to make use of storing/reading/deciding data flows

        3. AttackRunner object would be good to have. I can see it being a useful component if made unit-agnostic 
        
        4. CombatOrchestrator being unit-agnostic as well would be nice. Or split apart into only necessary components 
           Ideal world any unit performing combat just has one, but there's probably some nuance here. 
                What if it's a turret or a neutral/undamageable entity?
                What if we want an object that just overlays a hitbox for infinity/until destroyed?

        5. More robust architecture, requirements checking, and phase delineation in AttackSO
                1.  VFX routines
                    SFX routines
                    Trajectory routines for hitboxes
                    More complicated things... yey!
               
                2.  It's so easy to get the phase lengths wrong manually
                    It's also a bit tedious to conceptualize, define concrete building blocks, and compose phases
        

*/
public class CombatOrchestrator : MonoBehaviour, IHittable, IHitboxSource
{
    // health component eventually
    [SerializeField] float health;

    // weapon object.... what actually is this ??
    [SerializeField] WeaponHolder equippedWeapon;

    // current attack
    AttackSO currentAttack;
    List<CombatPhase> phases;
    List<CombatPhase> emptyPhaseList = new List<CombatPhase>();
    public int currPhase;
    // AttackOrchestrator attackOrch; ???
    // SpellOrchestrator spellOrch; ???

    // combat context should just be fixed to this unit
    CombatContext context; // updates on weapon swaps, etc.

    // active hitboxes
    [SerializeField] List<Hitbox> activeHitboxes = new List<Hitbox>();

    #region MonoBehaviour
    private void Awake() {}
    private void Start() { phases = emptyPhaseList; }

    private void Update()
    {
        // run attack
        UpdateAttackRunner();

        // tick active hitboxes & remove dead
        List<int> inactive = TickActiveHitboxes();
        CleanDeadHitboxes(inactive);
    }
    #endregion

    #region Gizmos
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        for (int i = 0; i < activeHitboxes.Count; i++)
        {
            HitboxGizmo drawData = activeHitboxes[i].GetGizmoData();
            Gizmos.matrix = Matrix4x4.TRS(drawData.position, drawData.rotation, Vector3.one);
            Gizmos.DrawCube(Vector3.zero, drawData.extents);
        }
    }
    #endregion

    #region Attack Runner
    public void InitializeAttackRunner(List<CombatPhase> combatPhases)
    {
        currPhase = 0;
        phases = combatPhases;
        Debug.Log("Running: " + currentAttack.name + " with phase count of " + combatPhases.Count);
        context = new CombatContext(this, equippedWeapon.weaponData, 0.0f);
        // yeah next part here
        phases[currPhase].Begin(context);
    }

    public void UpdateAttackRunner()
    {
        // safety check
        if (currentAttack == null)
            return; 

        // safety check
        if (currPhase >= phases.Count)
        {
            Debug.Log("Clearing: " + currentAttack.name);
            ClearAttackRunner();
            return;
        }
        
        // tick phase
        context.phaseTime += Time.deltaTime;
        phases[currPhase].Tick(context, Time.deltaTime);

        // check if move to next phase
        if (context.phaseTime >= phases[currPhase].duration)
        {
            phases[currPhase].End(context);
            currPhase++;

            if (currPhase < phases.Count)
                phases[currPhase].Begin(context);
        }
    }

    public void ClearAttackRunner()
    {
        currentAttack = null;
        phases = emptyPhaseList;
        currPhase = 0;
    }
    #endregion

    #region Weapon Helpers
    public WeaponDataSO GetEquippedWeapon()
    {
        return equippedWeapon.weaponData;
    }

    public void SetEquippedWeapon(WeaponDataSO weapon)
    {
        equippedWeapon.LoadWeaponFromSave(weapon);
    }
    #endregion

    #region Hittable Interface
    public void Hit(HitboxRecord hitboxRecord)
    {
        health -= hitboxRecord.context.damage.baseDamage;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
    #endregion

    #region Hitbox Ownership
    public void RegisterHitbox(Box box, int hits, float duration)
    {
        /*
            Okay, enhancement ideas
            1. find a dynamic position to launch hitboxes relative to the weapon AND the attack
            2. rotations, uhhhh yeah. parenting is an issue we have yet to solve
            3. what if we want sphere or whatever other shape?? that's a bigger, multi-piece one though
        */
        Hitbox hitbox = new Hitbox(duration, 
                                   transform.position + transform.forward, 
                                   box, transform.rotation, this, 
                                   equippedWeapon.weaponData, currentAttack.damageObject, hits);
        activeHitboxes.Add(hitbox);
        Debug.Log("Hitbox added at: " + (transform.position + transform.forward) + " with duration " + duration 
                  + ". ActiveHitboxes list is now of size: " + activeHitboxes.Count + ". Time added: " + Time.time);
    }
    
    private List<int> TickActiveHitboxes()
    {
        List<int> inactive = new List<int>();
        for (int i = 0; i < activeHitboxes.Count; i++)
        {
            activeHitboxes[i].Tick(Time.deltaTime);
            if (!activeHitboxes[i].Active) {
                Debug.Log("Flagged hitbox at: " + activeHitboxes[i].GetGizmoData().position + " for removal");
                inactive.Add(i);
            }
        }

        return inactive;
    }

    private void CleanDeadHitboxes(List<int> inactiveIndices)
    {
        for (int index = inactiveIndices.Count - 1; index >= 0; index--)
        {
            int removePosition = inactiveIndices[index];
            activeHitboxes.RemoveAt(removePosition);
        }
    } 
    #endregion

    #region HitboxSource Interface
    public void OnHitConfirmed(HitboxRecord hitMe)
    {
        // EVERY IHittable implements Hit in their own way, SaveGem, CombatOrchestrator, Wall, etc.

        // if the hit target is just ourselves, don't apply damage
        if (hitMe.target?.GetGameObject() == GetHitboxSourceGameObject())
            return;

        Debug.Log("Hit target: " + hitMe.target);
        hitMe.target?.Hit(hitMe);
    }

    // something about this feels awkward
        // like, 
        // get the IHittable target
        // from the HitboxRecord
        // then tell it to get Hit
        // by this HitboxRecord
        // feel like it should just be, tell the target to be hit by the hitbox record
        // or idk maybe there's a simpler way to design this

        // maybe could just be its own separate monobehaviour or object 
        // receives hitbox records and performs the "hit" procedure

    
    public GameObject GetHitboxSourceGameObject()
    {
        return this.gameObject;
    }
    #endregion

    #region Attack Request
    public AttackSO AttemptAttack()
    {
        // build context for decision
        CombatPhase phase = null;
        if (phases.Count != 0 && currPhase < phases.Count)
            phase = phases[currPhase];

        // try attack
        currentAttack = equippedWeapon.AttemptAttack(currentAttack, phase);
        if (currentAttack != null)
            InitializeAttackRunner(currentAttack.combatPhases);

        return currentAttack;
    }
    #endregion

    #region Health
    public void SetHealth(float healthIn)
    {
        health = healthIn;
    }

    public float GetHealth()
    {
        return health;
    }
    #endregion
}