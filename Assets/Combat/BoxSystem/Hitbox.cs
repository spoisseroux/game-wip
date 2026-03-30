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
    void OnHitConfirmed(HitboxRecord record);
}

[System.Serializable]
public class Hitbox
{   
    // source
    public IHitboxSource source { get; private set; } // this gets replaced by Context

    // context
    public HitboxContext context;

    // positioning information
    public Box box;
    public Vector3 position;
    public Quaternion orientation;

    // timing
    public float activeTime; // this in frames or engine ticks??? same with timer!! has to be clarified at some point
    public CountdownTimer active;

    // max hit counter allowed for this Hitbox
    private Dictionary<IHittable, int> hitHittables;
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
        context = new HitboxContext
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
        hitHittables = new Dictionary<IHittable, int>();
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
                // not hit yet
                if (!hitHittables.ContainsKey(hitMe))
                {
                    // construct HitRecord and feed in
                    context.source?.OnHitConfirmed(new HitboxRecord());
                    hitHittables.Add(hitMe, 1);
                }
                // can the object be hit again
                else if (hitHittables[hitMe] < hitCount)
                {
                    source?.OnHitConfirmed(new HitboxRecord());
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
        hitHittables.Clear(); // hmm, object probly gets destroyed afterwards sooo... hmm
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
    public Vector3 contactPoint;
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