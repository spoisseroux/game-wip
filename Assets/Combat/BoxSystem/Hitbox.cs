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

public enum HitboxState
{
    Closed,
    Open,
    Colliding // iffy
}

public struct HitboxContext
{
    // damagepayload
    int damage;
    // combatorch / ihitboxsource
    IHitboxSource source;
    // weapondataSO sourceWeapon
    WeaponDataSO sourceWeapon;
    // abilitySO sourceAbility
}

public struct HitboxRecord
{
    HitboxContext context;
    IHittable target;
    Vector3 contactPoint;
}

public interface IHitboxSource
{
    void OnHitConfirmed(HitboxRecord record);
}

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


[System.Serializable]
public class Hitbox
{   
    // source
    public IHitboxSource source { get; private set; } // this gets replaced by Context

    // context
    public HitboxContext context;

    // internal state
    public HitboxState state { get; private set; }

    // positioning information
    public Box box;
    public Vector3 position;
    public Quaternion orientation;

    // timing
    public float activeTime; // this in frames or engine ticks??? same with timer!! has to be clarified at some point
    public CountdownTimer active;

    // max hit counter allowed for this Hitbox
    private Dictionary<IHittable, int> hitHittables;
    private Dictionary<IDamageable, int> hitDamageables;
    private int damageCount;

    public Hitbox(float time, Vector3 p, Box b, Quaternion q, IHitboxSource s, int hitsAllowed = 1)
    {
        // parent, replace with HitboxContext
        source = s;

        // position info 
        position = p;
        box = b;
        orientation = q;

        // timing
        activeTime = time;

        // state
        state = HitboxState.Closed;

        // stored hits
        hitHittables = new Dictionary<IHittable, int>();
        hitDamageables = new Dictionary<IDamageable, int>();
        damageCount = hitsAllowed;

        // create the timer
        active = new CountdownTimer(activeTime)
        {
            OnStart = StartCheckingCollision,
            OnStop = StopCheckingCollision
        };
    }

    public void Execute()
    {
        // start --> open collider and begin countdown
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
                if (!hitHittables.ContainsKey(hitMe))
                {
                    // construct HitRecord and feed in
                    source?.OnHitConfirmed(new HitboxRecord());
                    hitHittables.Add(hitMe, 1);
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
        hitDamageables.Clear(); // hmm, object probly gets destroyed afterwards sooo... hmm
    }

    public HitboxGizmo GetGizmoData()
    {
        return new HitboxGizmo(position, box.GizmoXYZ(), orientation);
    }
}
