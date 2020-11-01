using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [Header("Default Properties")]
    public int minNavigationPriority;
    public int maxNavigationPriority;
    public int maxHealth;

    [Header("Internal State Variables")]
    public int currHealth;
    public int alertnessLevel;
    public bool isAlive;
    public bool isImmobilized;

    [Header("Sensory Variables")]
    public bool canSeePlayer;
    public Vector3 lastSeenPlayerPosition;
    public float timeSincePlayerSeen;


    // Behavior states are ordered by priority; lowest index = highest priority
    public enum BehaviorState{
        Dead = 0,
        Immobilized,
        Combat,
        Pursuit,
        Investigating,
        Patrolling,
        Idle     
    }

    public BehaviorState currentState;


    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;



    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        navMeshAgent.avoidancePriority = Random.Range(minNavigationPriority, maxNavigationPriority + 1);
    }

    void Start()
    {
        currHealth = maxHealth;
        currentState = BehaviorState.Patrolling;
    }

    void Update()
    {
        UpdateSensoryData();
        EvaluateCurrentState();
    }


    
    private void UpdateSensoryData(){
        // Collect data from the world and store in variables
    }


    private void EvaluateCurrentState(){
        // Using sensory data variables, determine whether to change state based on priority
        
        switch (alertnessLevel){
            // Idle, not alert
            case 0:
            break;

            case 1:
            break;

            case 2:
            break;

            default:
                alertnessLevel = 0;
            break;
        }

        if(canSeePlayer){
            
        }
    }



    private bool IsPriorityHigher(BehaviorState newState){
        if ((int)newState < (int)currentState){
            return true;
        }
        else{
            return false;
        }
    }

    private BehaviorState ReturnHigherPriority(BehaviorState newState){
        if ((int)newState < (int)currentState){
            return newState;
        }
        else{
            return currentState;
        }
    }

    private void SetState(BehaviorState state){
        // Reset action queue and change state ONLY IF the desired state is different from the current one
    }
}
