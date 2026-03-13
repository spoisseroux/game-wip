using UnityEngine;

[System.Serializable]
public abstract class CombatEvent
{
    [SerializeField] protected float duration;
    [SerializeField] protected bool instant;

    public abstract void Execute();
    public abstract void Update(float delta);
    public abstract void CleanUp();
}

// examples
    // deploy hitbox
    // start attack anim
    // add stat buff
    // attack-ending vulnerability pause
    // combo-attack-listen period??
    // spawn vfx
    // play sfx

/*
    Maybe we need a generic CombatOrchestrator component?? Sorta like the PlayerCombatManager but just.... better lol
*/

// need to rethink these events and how they are configured... 
// hitboxes are owned by combatorchestrator, but this object is fundamentally about the hitbox
// maybe it just carries data to read and construct a Hitbox

// or maybe it's just an event and the attack provides the correct hitbox??? idk,...
[System.Serializable]
public class DeployHitbox : CombatEvent
{
    [SerializeField] private Box boxInfo;
    [SerializeField] private int hitCount;

    public override void Execute() { }

    public override void Update(float delta) { }

    public override void CleanUp() { }
}

[System.Serializable]
public class PlaySFX : CombatEvent
{
    [SerializeField] AudioClip sfx;

    public override void Execute() { }

    public override void Update(float delta) { }

    public override void CleanUp() { }
}

[System.Serializable]
public class PlayVFX : CombatEvent
{
    [SerializeField] AudioClip vfx;

    public override void Execute() { }

    public override void Update(float delta) { }

    public override void CleanUp() { }
}

public class AddStatusEffect : CombatEvent
{
    // [SerializeField] StatusEffect vfx;
    // [SerializeField] float duration; // duration of status buff for builder
    // [SerializeField] float tickTime; // hmm? default arg that overrides if not 0? how to tell if ticking or idk

    public override void Execute() { }

    public override void Update(float delta) { }

    public override void CleanUp() { }
}

public class ListenForComboInput : CombatEvent
{
    public override void Execute() { }

    public override void Update(float delta) { }

    public override void CleanUp() { }
}