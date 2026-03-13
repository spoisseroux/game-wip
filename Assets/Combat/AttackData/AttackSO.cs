using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAttackDataSO", menuName = "Data/AttackDataSO")]
public class AttackSO : ScriptableObject
{
    // duration for the whole attack --> [windup + active + finishing anim]
    public float attackDuration;
    // duration of hitbox
    public float hitboxDuration;
    // damage
    public int damage;
    // hitbox
    public Box hitbox; // could make a list!!!!
    // movespeed modifier
    public float movespeedModifier;
    // anim
    public string animName;
    // combo window, checked against progress
    public float comboWindowStart;
    public float comboWindowEnd;
    // hit number
    public int hitCount;



    /*
        Refactor in progress!
    */
    [SerializeField] List<CombatPhase> combatPhases;
    [SerializeField] List<MovementPhase> movementPhases;

    public float combatDuration;
    public float movementDuration; // maybe we do a foreach and read duration from phases?
}

/*
CHECK WHETHER THIS IS AN OVERENGINEERED MESS...

FSM is a traffic cop --> okay, we're in this state for X seconds or until X, dispatch the relevant movement code at right times
Motor is receiving calls and moving our player/npc
CombatManager --> orchestrates all attack related logic (deploy hitbox, clean up objects, )

Calulcate the total duration across all phases, 
use that as overall piece,
and then use repeated 0 --> duration to cycle through Phases

[CreateAssetMenu]
public class AttackData : ScriptableObject {
    [Header("Identity")]
    public string attackName;
    public AttackType attackType; // Light, Heavy, Special, etc.
    
    [Header("Motor Control")]
    public int basePriority = 5;
    public AxisMask defaultControlledAxes = AxisMask.None;
    
    [Header("Input & Canceling")]
    public bool allowPlayerInput = false;
    [Range(0f, 1f)] public float inputInfluence = 0f;
    public bool canBeCanceledBy; // other attacks, dodge, etc.
    public float cancelWindowStart = 0f; // time when canceling is allowed
    
    [Header("Resources")]
    public float staminaCost = 0f;
    public float manaCost = 0f;
    public float cooldown = 0f;
    
    [Header("Animation")]
    public AnimationClip animationClip;
    public bool useRootMotion = false;
    public AvatarMask animationMask; // which body parts animate
    
    [Header("Combat Data")]
    public float baseDamage = 10f;
    public DamageType damageType;
    public StatusEffect[] statusEffects;
    public bool hasKnockback = false;
    public Vector3 knockbackVector = Vector3.back * 5f;
    
    [Header("Timing")]
    public float totalDuration = 1f; // some attacks calculate this, others set manually
    public float recoveryTime = 0.3f; // time before returning to idle
    
    [Header("VFX & Audio")]
    public GameObject vfxPrefab;
    public AudioClip[] sfxClips;
    
    // Virtual methods for subclasses to override
    public virtual void OnAttackStart(CombatManager combat) { }
    public virtual void OnAttackEnd(CombatManager combat) { }
}
*/

/*
[CreateAssetMenu]
public class VaultSlamAttackData : AttackData {
    // Movement phases
    public PhaseData[] phases = new PhaseData[] {
        new PhaseData { 
            duration = 0.2f, 
            motionType = MotionType.Pause,
            controlledAxes = AxisMask.All 
        },
        new PhaseData { 
            duration = 0.5f, 
            motionType = MotionType.LaunchAngled,
            launchSpeed = 15f,
            launchAngle = 45f,
            controlledAxes = AxisMask.XZ | AxisMask.Y 
        },
        new PhaseData { 
            duration = 0.3f, 
            motionType = MotionType.Hover,
            controlledAxes = AxisMask.Y 
        },
        new PhaseData { 
            duration = 0.4f, 
            motionType = MotionType.DashToGround,
            dashSpeed = 20f,
            controlledAxes = AxisMask.All 
        }
    };
    
    // Combat events (frame-based or time-based, your choice)
    public CombatEvent[] combatEvents = new CombatEvent[] {
        new CombatEvent { 
            triggerTime = 1.0f, // or frame 60 at 60fps
            eventType = CombatEventType.ActivateHitbox,
            hitboxID = "slamAOE" 
        },
        new CombatEvent { 
            triggerTime = 1.4f,
            eventType = CombatEventType.SpawnVFX,
            vfxPrefab = "ImpactCrater" 
        }
    };
}

[System.Serializable]
public class PhaseData {
    public float duration;
    public MotionType motionType;
    public AxisMask controlledAxes;
    // Type-specific params
    public float launchSpeed;
    public float launchAngle;
    public AnimationCurve velocityCurve;
    // etc.
}

*/