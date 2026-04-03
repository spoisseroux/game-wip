using UnityEngine;

/*  
    The context object which a movement event executes upon
*/
public class CombatContext
{
    public CombatOrchestrator orch;
    public WeaponDataSO weapon;
    public float phaseTime;

    public CombatContext(CombatOrchestrator coIn, WeaponDataSO weaponIn, float timeIn)
    {
        orch = coIn;
        weapon = weaponIn;
        phaseTime = timeIn;
    }

    // spells, abilities, etc.
}

/*
    A single unit of combat execution
*/
[System.Serializable]
public abstract class CombatEvent : ScriptableObject
{
    [SerializeField] public float duration;

    public abstract void Execute(CombatContext context);
    public abstract void Tick(CombatContext context, float delta);
    public abstract void CleanUp(CombatContext context);

    public override string ToString()
    {
        return GetType().ToString();
    }
}

// examples
    // deploy hitbox
    // start attack anim
    // add stat buff
    // idle/pause
    // combo-attack-listen period??
    // spawn vfx
    // play sfx