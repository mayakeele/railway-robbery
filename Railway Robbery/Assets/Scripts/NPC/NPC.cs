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


    public NavMeshAgent navMeshAgent;
    public Animator animator;
    public Rigidbody rb;


    void Start()
    {
        currentState = BehaviorState.Patrolling;
    }

    void Update()
    {
        
    }
}
