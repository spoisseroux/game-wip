using System.Collections.Generic;
using UnityEngine;


/*
    REFACTOR STUFF:
    - IHittable becomes the single entry point for hitting an object, no more IDamageable
            - public abstract void Hit() --> Hit(HitRecord )

    - IHitboxSource becomes single entry point for applying a hit
            - public CollisionedWith(IHittable hittable) --> OnHitConfirmed(HitRecord record)

    - HitboxContext object made upon Hitbox construction with damage, source, etc.
            - contains DamagePayload (type, amount, etc.), IHitboxSource/CombatOrchestrator, source WeaponDataSO/AbilitSO for metadata
    - HitRecord object made upon confirmed hit for each confirmed hit
            - contains original HitboxContext, target's CombatOrchestrator and/or IHittable, contact point, whatever else needed

*/
public interface IHitboxSource
{
    // maybe good to add a GetGameObject function here too
    void OnHitConfirmed(HitboxRecord record);
    public GameObject GetHitboxSourceGameObject();
}

[System.Serializable]
public class Hitbox
{   
    // source
    public IHitboxSource source { get; private set; } // this gets replaced by Context

    // context
    public HitboxContext hitboxContext;

    // positioning information
    public Box box;
    public Vector3 position;
    public Quaternion orientation;

    // timing
    public float activeTime; // this in frames or engine ticks??? same with timer!! has to be clarified at some point
    public CountdownTimer active;

    // max hit counter allowed for this Hitbox
    private Dictionary<IHittable, int> hitTargets;
    private int hitCount;

    // internal state
    private enum HitboxState {
        Closed,
        Open
    }
    private HitboxState state { get; set; }
    public bool Active { get { return state == HitboxState.Open; } }

    // constructor
    public Hitbox(float time, Vector3 p, Box b, Quaternion q, IHitboxSource s, WeaponDataSO weapon, 
                  DamagePayload payload = null, 
                  int hitsAllowed = 1)
    {
        // parent context
        hitboxContext = new HitboxContext
        {
            source = s,
            damage = payload,
            sourceWeapon = weapon
        };

        // position info 
        position = p;
        box = b;
        orientation = q;

        // timing
        activeTime = time;

        // state
        state = HitboxState.Open;

        // stored hits
        hitTargets = new Dictionary<IHittable, int>();
        hitCount = hitsAllowed;

        // create and start timer
        active = new CountdownTimer(activeTime)
        {
            OnStart = StartCheckingCollision,
            OnStop = StopCheckingCollision
        };
        active.Start();
    }

    public void Tick(float deltaTime)
    {
        active.Tick(deltaTime);
        if (state != HitboxState.Open) { return; }

        // check for overlaps with damageable objects
        Collider[] cols = Physics.OverlapBox(position, new Vector3(box.length / 2, box.width / 2, box.height / 2), orientation);
        for (int i = 0; i < cols.Length; i++)
        {
            // hittables
            if (cols[i].TryGetComponent<IHittable>(out IHittable hitMe))
            {
                HitboxRecord record = new HitboxRecord()
                {
                    context = hitboxContext,
                    target = hitMe,
                    contactDir = Vector3.zero
                };
                // not hit yet
                if (!hitTargets.ContainsKey(hitMe))
                {
                    // construct HitRecord and feed in
                    hitboxContext.source?.OnHitConfirmed(record);
                    hitTargets.Add(hitMe, 1);
                }
                // can the object be hit again
                else if (hitTargets[hitMe] < hitCount)
                {
                    source?.OnHitConfirmed(record);
                }
            }
        }
    }

    public void StartCheckingCollision()
    {
        state = HitboxState.Open;
    }

    public void StopCheckingCollision()
    {
        // cleanup
        source = null;
        state = HitboxState.Closed;
        hitTargets.Clear(); // hmm, object probly gets destroyed afterwards sooo... hmm
    }

    public HitboxGizmo GetGizmoData()
    {
        return new HitboxGizmo(position, box.GizmoXYZ(), orientation);
    }

    public void RepositionHitbox(Vector3 delta)
    {
        position += delta;
    }

    public void RotateHitbox(Quaternion qIn)
    {
        // ???
    }
}

public struct HitboxContext
{
    // damagepayload, edit into status effect appliers at some points
    public DamagePayload damage; // IF THIS CAN CHANGE, WE NEED TO MAKE IT A CLASS!
    // combatorch / ihitboxsource
    public IHitboxSource source;
    // weapondataSO sourceWeapon
    public WeaponDataSO sourceWeapon;
    // abilitySO sourceAbility
}

public struct HitboxRecord
{
    public HitboxContext context;
    public IHittable target;
    public Vector3 contactDir;
}

/*
    Used for Gizmos.matrix = Matrix4x4.TRS()
    as well as Extents data
*/
public struct HitboxGizmo
{
    public Vector3 position;
    public Vector3 extents;
    public Quaternion rotation;

    public HitboxGizmo(Vector3 p, Vector3 e, Quaternion q)
    {
        position = p;
        extents = e;
        rotation = q;
    }
}