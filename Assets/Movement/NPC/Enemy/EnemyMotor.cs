using System;
using UnityEngine;
using UnityEngine.AI;

// basically, contains movement logic for 
public class EnemyMotor : MonoBehaviour
{
    // needed components
    [SerializeField] NavMeshAgent agent;
    [SerializeField] AnimationController animationController;
    [SerializeField] Transform player;

    // fsm & states
    [SerializeField] StateMachine fsm;


    // detection 
    float detectionRange = 5f;
    float chaseRange = 10f;

    // attack
    float attackRange = 1f;

    #region MonoBehaviour
    void Awake()
    {
        
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
