using UnityEngine;

/*  
    The context object which a movement event executes upon
*/
public struct CombatContext
{
    public CombatOrchestrator combatOrchestrator;
    public WeaponDataSO weapon;
    public float phaseTime;
}

/*
    A single unit of combat execution
*/
[System.Serializable]
public abstract class CombatEvent : ScriptableObject
{
    [SerializeField] protected float duration;
    [SerializeField] protected bool instant;

    public abstract void Execute(CombatContext context);
    public abstract void Update(CombatContext context, float delta);
    public abstract void CleanUp(CombatContext context);
}

// examples
    // deploy hitbox
    // start attack anim
    // add stat buff
    // attack-ending vulnerability pause
    // combo-attack-listen period??
    // spawn vfx
    // play sfx

[CreateAssetMenu(fileName = "NewDeployHitbox", menuName = "CombatEvent/DeployHitbox", order = 1)]
public class DeployHitbox : CombatEvent
{
    [SerializeField] private Box boxInfo;
    [SerializeField] private int hitCount;
    [SerializeField] private float hitboxDuration;

    public override void Execute(CombatContext context) { }

    // use this to update hitbox position i guess?
    public override void Update(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { }
}

[CreateAssetMenu(fileName = "NewPlaySFX", menuName = "CombatEvent/PlaySFX", order = 1)]
public class PlaySFX : CombatEvent
{
    [SerializeField] AudioClip sfxClip;

    public override void Execute(CombatContext context) { }

    public override void Update(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { }
}

[CreateAssetMenu(fileName = "NewPlayVFX", menuName = "CombatEvent/PlayVFX", order = 1)]
public class PlayVFX : CombatEvent
{
    [SerializeField] GameObject vfxPrefab;

    public override void Execute(CombatContext context) { }

    public override void Update(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { }
}

[CreateAssetMenu(fileName = "NewComboInput", menuName = "CombatEvent/ComboInput", order = 1)]
public class ListenForComboInput : CombatEvent
{
    public override void Execute(CombatContext context) { }

    public override void Update(CombatContext context, float delta) { }

    public override void CleanUp(CombatContext context) { }
}