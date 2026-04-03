using System;
using System.Collections.Generic;
using UnityEngine;

/*
    So. How do both the Movement and Combat components know to start? Where do they run?

    upon RequestAttack() in MovementManager returning an AttackSO (THATS A WHOLE OTHER ISSUE BTW) we know, ok time to go?

    then say, in AttackState() currentAttack.RunMovementRoutine();
              in CombatOrchestrator currentAttack.RunCombatRoutine();

    Seems a bit off. Maybe the AttackOrchestrator idea isn't bad?

    Idk, make a basic attack and go from there
*/

[CreateAssetMenu(fileName = "newAttackDataSO", menuName = "Data/AttackDataSO")]
public class AttackSO : ScriptableObject
{
    /*
        Refactor in progress!
    */
    [SerializeField] public List<CombatPhase> combatPhases;
    [SerializeField] public List<MovementPhase> movementPhases;
    [SerializeField] public DamagePayload damageObject;
    [SerializeField] public string attackName;
    [SerializeField] public int comboIndex;

    public float duration { 
        get
        {
            float combat = 0.0f;
            for (int i = 0; i < combatPhases.Count; i++) {
                combat += combatPhases[i].duration;
            }

            float move = 0.0f;
            for (int i = 0; i < movementPhases.Count; i++) {
                move += movementPhases[i].duration;
            }

            if (combat == move)
                return move;
            
            return -404f;
        }
    }
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