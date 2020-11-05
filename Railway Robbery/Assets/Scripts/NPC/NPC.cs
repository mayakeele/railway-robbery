using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
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

    public enum AlertnessLevel{
        Unwary = 0,
        Suspicious = 1,
        Alerted = 2
    }


    public BehaviorState currentState;


    [Header("Default Properties")]

    public int minNavigationPriority;
    public int maxNavigationPriority;

    public int maxHealth;

    public Transform eyeTransform;
    public float maxVisionDistance;
    public float visionConeAngle;


    [Header("External Objects")]

    public Transform playerHead;
    public Transform playerHandLeft;
    public Transform playerHandRight;

    public Transform currentTrainCar;


    [Header("Sensory Variables")]

    public int currHealth;
    public bool isAlive;
    public bool isImmobilized;

    public AlertnessLevel currAlertnessLevel = AlertnessLevel.Unwary;

    public bool canSeePlayer;
    public Vector3 lastSeenPlayerPosition;
    public float timeSincePlayerSeen;


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

        // Cast rays to each part of the player's body
        Vector3 directionToPlayer = playerHead.position - eyeTransform.position;
        if(Vector3.Angle(eyeTransform.forward, directionToPlayer) <= visionConeAngle / 2){

        }
        if(Physics.Raycast(eyeTransform.position, directionToPlayer, out RaycastHit hitInfo, maxVisionDistance)){

        }
    }


    private void EvaluateCurrentState(){
        // Using sensory data variables, determine whether to change state based on priority
        
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
