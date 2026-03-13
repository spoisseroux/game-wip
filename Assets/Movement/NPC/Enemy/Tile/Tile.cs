using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// tile state
public abstract class TileState : IState
{
    protected readonly NavMeshAgent agent;
    protected readonly Animator animator;

    public TileState(NavMeshAgent ag, Animator a)
    {
        agent = ag;
        animator = a;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    public override string ToString()
    {
        return GetType().ToString();
    }
}

// concrete states
public class TileIdleState : TileState
{
    private string enterAnim;
    private string exitAnim;

    public TileIdleState(NavMeshAgent ag, Animator a) : base(ag, a)
    {
        
    }

    public override void Enter()
    {
        Debug.Log("Tile in idle state");
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }
}

public class TileAlertState : TileState
{
    public TileAlertState(NavMeshAgent ag, Animator a) : base(ag, a)
    {
        
    }

    public override void Enter()
    {
        // play animation
        Debug.Log("Tile in alert state");

    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        
    }
}

public class TileAttackState : TileState
{
    private string enterAnim;

    private float duration;
    CountdownTimer timer;

    public TileAttackState(NavMeshAgent ag, Animator a, float dur) : base(ag, a)
    {
        duration = dur;
        timer = new CountdownTimer(duration);
    }

    public override void Enter()
    {
        // animator
        Debug.Log("Tile in attack state");
        timer.Start();
    }

    public override void Update()
    {
        timer.Tick(Time.deltaTime);
    }

    public override void Exit()
    {
        timer.Reset(duration);
    }

    public float Progress => timer.progress;
    public bool Complete => timer.progress <= 0;
}

// monobehaviour
public class Tile : MonoBehaviour, IRuneReactor, IDamageable, IHitboxSource
{
    // components
    [SerializeField] Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] RuneDataSO rune;
    [SerializeField] Material runeReactionTexture;
    [SerializeField] private Transform player;

    // death event
    public delegate void TileDied();
    public event TileDied OnTileDeath;

    // alert
    [SerializeField] private float alertRange;
    // attack
    [SerializeField] private float attackRange;
    [SerializeField] private float attackDuration;
    // health
    [SerializeField] private int health;
    // damage
    [SerializeField] private int damage;
    [SerializeField] private int runeDamageMultiplier;

    // hitbox
    [SerializeField] Box hitboxPos;
    List<Hitbox> activeHitboxes = new List<Hitbox>();
    List<Hitbox> inactive = new List<Hitbox>();

    // fsm and states
    StateMachine fsm;
    private TileIdleState idleState;
    private TileAlertState alertState;
    private TileAttackState attackState;

    #region MonoBehaviour
    void Awake()
    {
        // health
        health = 20;

        // get player
        player = FindFirstObjectByType<PlayerManager>().transform.Find("Eyes").transform;

        // fsm
        fsm = new StateMachine();

        // states
        idleState = new TileIdleState(agent, animator);
        alertState = new TileAlertState(agent, animator);
        attackState = new TileAttackState(agent, animator, attackDuration);

        // idle transitions
        At(idleState, alertState, new FuncPredicate(() => CanReactToPlayer()));

        // alert transitions
        At(alertState, idleState, new FuncPredicate(() => !CanReactToPlayer()));
        At(alertState, attackState, new FuncPredicate(() => CanAttackPlayer()));

        // attack transitions
        At(attackState, idleState, new FuncPredicate(() => attackState.Complete && !CanReactToPlayer()));
        At(attackState, alertState, new FuncPredicate(() => attackState.Complete));

        fsm.SetState(idleState);
    }

    // Update is called once per frame
    void Update()
    {
        // look at the scary player guy!
        if (fsm.GetCurrentState() == alertState || fsm.GetCurrentState() == attackState) {
            transform.LookAt(player);

            if (fsm.GetCurrentState() == attackState)
            {
                if (attackState.Progress >= 0.97f)
                {
                    Attack();
                }
            }
        }

        // tick hitboxes
        foreach (var hbox in activeHitboxes)
        {
            hbox.Tick(Time.deltaTime);
        }

        // remove inactive ones
        if (activeHitboxes.Count > 0) {
            for (int i = activeHitboxes.Count; i >= 0; i--)
            {
                if (activeHitboxes[i].state == HitboxState.Closed)
                {
                    activeHitboxes.RemoveAt(i);
                }
            }
        }

        // tick fsm
        fsm.Update();
    }
    #endregion

    #region IRuneReactor Interface
    public void React(RuneType rune)
    {
        return;
    }

    public void RegisterRune(RuneType rune)
    {
        return;
    }
    #endregion

    #region IDamageable
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            OnTileDeath?.Invoke();
        return;
    }
    #endregion

    #region IHitboxSource
    public void OnHitConfirmed(HitboxRecord hit) {}
    
    #endregion

    #region FSM
    void At(IState from, IState to, IPredicate condition) => fsm.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => fsm.AddAnyTransition(to, condition);
    #endregion

    #region Detection
    public bool CanReactToPlayer()
    {
        return Vector3.Distance(this.transform.position, player.position) <= alertRange;
    }

    public bool CanAttackPlayer()
    {
        return Vector3.Distance(this.transform.position, player.position) <= attackRange;
    }
    #endregion

    #region Attack
    public IEnumerator AttackRoutine()
    {
        // wait a lil
        yield return new WaitForSeconds(0.3f);
        // create the hitbox and add it
        Hitbox box = CreateHitbox(this.transform.position + this.transform.forward * 0.3f, this.transform.rotation);
        activeHitboxes.Add(box);
    }

    public void Attack()
    {
        StartCoroutine(AttackRoutine());
    }

    // create hitbox for current attack from parent's position & rotation
    private Hitbox CreateHitbox(Vector3 spawnPos, Quaternion spawnRotation)
    {
        return new Hitbox(0.5f,
                          spawnPos,
                          hitboxPos,
                          spawnRotation,
                          this, 
                          1);
    }
    #endregion
}
