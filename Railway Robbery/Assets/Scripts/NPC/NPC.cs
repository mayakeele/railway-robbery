using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{

    public int maxHealth;
    public int currHealth;

    public int alertnessLevel;

    public Vector3 lastSeenPlayerPosition;

    public bool isAlive;
    public bool isImmobilized;
    public bool canSeePlayer;


    public enum BehaviorState{
        Idle = 0,
        Patrolling,
        Investigating,
        Pursuit,
        Combat,
        Immobilized,
        Dead
    }

    public BehaviorState currentState;


    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;


    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        currentState = BehaviorState.Patrolling;
    }

    void Update()
    {
        
    }
}
