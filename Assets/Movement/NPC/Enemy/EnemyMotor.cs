using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMotor : MonoBehaviour
{
    // needed components
    [SerializeField] NavMeshAgent agent;
    [SerializeField] AnimationController animationController;
    [SerializeField] Transform player;

    // fsm & states
    [SerializeField] StateMachine fsm;
    private IdleState idleState;
    private RoamState roamState;
    private AlertState alertState;
    private ChaseState chaseState;
    private EnemyAttackState attackState;

    // detection 
    float detectionRange = 5f;
    float chaseRange = 10f;

    // attack
    float attackRange = 1f;

    #region MonoBehaviour
    void Awake()
    {
        // fsm & states
        fsm = new StateMachine();
        idleState = new IdleState(this, animationController);
        roamState = new RoamState(this, animationController, agent, 20f);
        alertState = new AlertState(this, animationController);
        chaseState = new ChaseState(this, animationController, agent, player);
        attackState = new EnemyAttackState(this, animationController);

        // idle state transitions
        At(idleState, roamState, new FuncPredicate(() => idleState.GetProgress() <= 0));
        
        // roam transitions
        At(roamState, alertState, new FuncPredicate(() => Vector3.Distance(player.position, this.transform.position) <= detectionRange));
        At(roamState, idleState, new FuncPredicate(() => roamState.ReachedDestination()));

        // alert transitions
        At(alertState, roamState, new FuncPredicate(() => Vector3.Distance(player.position, this.transform.position) > chaseRange));
        At(alertState, chaseState, new FuncPredicate(() => Vector3.Distance(player.position, this.transform.position) <= chaseRange));

        // chase transitions
        At(chaseState, attackState, new FuncPredicate(() => Vector3.Distance(player.position, this.transform.position) <= attackRange));
        At(chaseState, roamState, new FuncPredicate(() => Vector3.Distance(player.position, this.transform.position) <= chaseRange));

        // attack transitions
        At(attackState, chaseState, new FuncPredicate(() => attackState.GetProgress() <= 0 
                                                         && Vector3.Distance(player.position, this.transform.position) <= chaseRange));
        At(attackState, roamState, new FuncPredicate(() => attackState.GetProgress() <= 0 
                                                         && Vector3.Distance(player.position, this.transform.position) > chaseRange));
            
        fsm.SetState(roamState);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Update();
    }
    
    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }
    #endregion

    #region FSM
    void At(IState from, IState to, IPredicate condition) => fsm.AddTransition(from, to, condition);
    void Any(IState to, IPredicate condition) => fsm.AddAnyTransition(to, condition);
    #endregion
}
