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
    public bool behaviorStateChanged;


    [Header("Default Properties")]

    public int minNavigationPriority;
    public int maxNavigationPriority;

    public int maxHealth;

    public Transform eyeTransform;
    public float visionRange;
    public float visionConeAngle;
    public LayerMask visionObstructingLayers;

    public float seenTimeToTriggerCombat;
    public float obstructedTimeToTriggerPursuit;
    public float alertLevelDecrementInterval;


    [Header("External Objects")]

    public GameObject player;
    private BodyPartReferences playerParts;
    public Transform playerHead;
    public Transform playerBody;
    public Transform playerFeet;
    public Transform playerHandLeft;
    public Transform playerHandRight;

    public Transform currentTrainCar;


    [Header("Sensory Variables")]

    public float currentHealth;
    public bool isAlive;
    public bool isImmobilized;

    public AlertnessLevel currentAlertnessLevel = AlertnessLevel.Unwary;

    public bool canSeePlayer = false;
    public Vector3 lastSeenPlayerPosition;
    public Vector3 lastSeenPlayerVelocity;
    public float timePlayerIsSeen;
    public float timePlayerIsHidden;

    public float timeUntilAlertDecremented;


    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;


    void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerParts = player.GetComponent<BodyPartReferences>();
        playerHead = playerParts.cameraTransform;
        playerBody = playerParts.bodyTransform;
        playerFeet = playerParts.feetTransform;

        navMeshAgent.avoidancePriority = Random.Range(minNavigationPriority, maxNavigationPriority + 1);
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentState = BehaviorState.Patrolling;
    }

    void Update()
    {
        behaviorStateChanged = false;
        UpdateSensoryData();
        EvaluateCurrentState();
    }

    private void OnDrawGizmos() {
        if(canSeePlayer){
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eyeTransform.position, playerHead.position);
        }
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(eyeTransform.position, eyeTransform.position + (eyeTransform.forward * visionRange));

        Gizmos.DrawSphere(lastSeenPlayerPosition, 0.1f);
    }


    
    private void UpdateSensoryData(){
        // Collect data from the world and store in variables

        // Cast rays to each part of the player's body if the player is within the vision cone and within maximum sight distance
        Vector3 directionToPlayer = playerHead.position - eyeTransform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        directionToPlayer.Normalize();

        canSeePlayer = false;
        if(distanceToPlayer <= visionRange){
            if(Vector3.Angle(eyeTransform.forward, directionToPlayer) <= visionConeAngle / 2){

                if(Physics.Raycast(eyeTransform.position, directionToPlayer, distanceToPlayer, visionObstructingLayers) == false){
                    canSeePlayer = true;
                }
            }
        }

        // Increment the amount of time the player has or has not been seen
        if (canSeePlayer){
            lastSeenPlayerPosition = playerBody.position;
            //lastSeenPlayerVelocity = playerHead.GetComponent<Rigidbody>().velocity;  ~~~~~~~~~~~~~~~~~~~~~~~~~  EXPAND THIS ~~~~~~~~~~~~~~~~~~~~~~~~~~~

            timePlayerIsSeen += Time.deltaTime;
            timePlayerIsHidden = 0;
            timeUntilAlertDecremented = 0;
        }
        else{
            timePlayerIsSeen = 0;
            timePlayerIsHidden += Time.deltaTime;
            timeUntilAlertDecremented += Time.deltaTime;
        }

    }


    private void EvaluateCurrentState(){
        // Using sensory data variables, determine whether to change state based on priority
        
        // Immediately stop all processes if the NPC has died
        if(currentHealth <= 0){
            TrySetState(BehaviorState.Dead);
        }

        // Main priority: look for player, raise alarm level if player is seen
        if(canSeePlayer){
            switch (currentAlertnessLevel){

                case AlertnessLevel.Unwary:
                    // Immediately raise suspicion and investigate disturbance
                    currentAlertnessLevel = AlertnessLevel.Suspicious;
                    TrySetState(BehaviorState.Investigating);
                break;

                case AlertnessLevel.Suspicious:
                    // Re-trigger investigation on the first frame the player is seen after being hidden
                    if(timePlayerIsSeen <= Time.deltaTime){
                        TrySetState(BehaviorState.Investigating, true);
                    }

                    // Trigger combat if the player remains seen for too long
                    if(timePlayerIsSeen >= seenTimeToTriggerCombat){
                        currentAlertnessLevel = AlertnessLevel.Alerted;
                        TrySetState(BehaviorState.Combat);
                    }
                break;

                case AlertnessLevel.Alerted:
                    TrySetState(BehaviorState.Combat);
                break;
            }
        }


        // Enter pursuit mode if player has disappeared from view for too long during combat
        if(currentState == BehaviorState.Combat && timePlayerIsHidden >= obstructedTimeToTriggerPursuit){
            TrySetState(BehaviorState.Pursuit, true);
        }


        // Decrement alarm level if enough time has passed without seeing the player
        if(timeUntilAlertDecremented >= alertLevelDecrementInterval){
            timeUntilAlertDecremented = 0;

            switch (currentAlertnessLevel){
                case AlertnessLevel.Alerted:
                    currentAlertnessLevel = AlertnessLevel.Suspicious;
                    TrySetState(BehaviorState.Patrolling, true);
                break;
                
                case AlertnessLevel.Suspicious:
                    currentAlertnessLevel = AlertnessLevel.Unwary;
                    TrySetState(BehaviorState.Patrolling, true);
                break;

                default:
                    currentAlertnessLevel = AlertnessLevel.Unwary;
                break;
            }

        }

        // Set a default state if no higher priority state has been selected
        TrySetState(BehaviorState.Patrolling);
    }



    private bool IsPriorityHigher(BehaviorState newState, BehaviorState prevState){
        if ((int)newState < (int)prevState){
            return true;
        }
        else{
            return false;
        }
    }

    private bool IsPriorityGreaterOrEqual(BehaviorState newState, BehaviorState prevState){
        if ((int)newState <= (int)prevState){
            return true;
        }
        else{
            return false;
        }
    }

    private BehaviorState ReturnHigherPriority(BehaviorState newState, BehaviorState prevState){
        if ((int)newState < (int)prevState){
            return newState;
        }
        else{
            return prevState;
        }
    }

    public void TrySetState(BehaviorState state, bool overrideDefaultPriority = false, BehaviorState newPriority = BehaviorState.Combat){
        // Reset action queue and change state IF the desired state is of a higher priority OR override priority is set to true

        if (IsPriorityHigher(state, currentState) || (overrideDefaultPriority == true && IsPriorityGreaterOrEqual(newPriority, currentState))){
            currentState = state;

            //Debug.Log(state.ToString());
            // reset action queue goes here
            behaviorStateChanged = true;
        }
    }



    public void DealDamage(int baseDamageAmount, Transform bodyPartHit, Vector3 hitLocation){
        float damageMultiplier = 1;

        currentHealth -= (baseDamageAmount * damageMultiplier);

        Debug.Log("Enemy Health: " + currentHealth.ToString());
    }

}
