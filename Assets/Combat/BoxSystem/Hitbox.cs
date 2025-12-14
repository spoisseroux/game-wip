using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HitboxState
{
    Closed,
    Open,
    Colliding // iffy
}

public interface IHitboxSource
{
    void CollisionedWith(Collider col);
    void CollisionedWith(IDamageable damageMe);
    void CollisionedWith(IHittable hitMe);
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

// Monobehavior for Gizmos?
[Serializable]
public class Hitbox
{
    public IHitboxSource source { get; private set; }
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
    private int hitCount;
    private int damageCount;

    // guid for unique hit
    private string guid = System.Guid.NewGuid().ToString();

    public Hitbox(float time, Vector3 p, Box b, Quaternion q, IHitboxSource s, int hitsAllowed)
    {
        // parent
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

        // guid
        guid = Guid.NewGuid().ToString();

        // create the timer
        active = new CountdownTimer(activeTime);
        active.OnStart = StartCheckingCollision;
        active.OnStop = StopCheckingCollision;
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
            // damage
            if (cols[i].TryGetComponent<IDamageable>(out IDamageable damageMe))
            {
                if (!hitDamageables.ContainsKey(damageMe))
                {
                    source?.CollisionedWith(damageMe);
                    hitDamageables.Add(damageMe, 1);
                }
                else if (hitDamageables[damageMe] < damageCount)
                {
                    source?.CollisionedWith(damageMe);
                    hitDamageables[damageMe]++;
                }
            }

            // hittables
            else if (cols[i].TryGetComponent<IHittable>(out IHittable hitMe))
            {
                if (!hitHittables.ContainsKey(hitMe))
                {
                    source?.CollisionedWith(hitMe);
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
        Debug.Log("hitbox ending");
        // cleanup function
        source = null;
        state = HitboxState.Closed;
        hitDamageables.Clear();
    }

    public HitboxGizmo GetGizmoData()
    {
        return new HitboxGizmo(position, box.GizmoXYZ(), orientation);
    }
}
