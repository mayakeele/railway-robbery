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
    public LayerMask visionObstructingLayers;

    public float seenTimeToTriggerCombat;
    public float alertLevelDecrementInterval;


    [Header("External Objects")]

    public Transform playerHead;
    public Transform playerHandLeft;
    public Transform playerHandRight;

    public Transform currentTrainCar;


    [Header("Sensory Variables")]

    public int currentHealth;
    public bool isAlive;
    public bool isImmobilized;

    public AlertnessLevel currentAlertnessLevel = AlertnessLevel.Unwary;

    public bool canSeePlayer = false;
    public Vector3 lastSeenPlayerPosition;
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

        navMeshAgent.avoidancePriority = Random.Range(minNavigationPriority, maxNavigationPriority + 1);
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentState = BehaviorState.Patrolling;
    }

    void Update()
    {
        UpdateSensoryData();
        EvaluateCurrentState();
    }

    private void OnDrawGizmos() {
        if(canSeePlayer){
            Gizmos.color = Color.red;
            Gizmos.DrawLine(eyeTransform.position, playerHead.position);
        }
        
        Gizmos.color = Color.yellow;
        //Vector3 leftLine = Quaternion.Euler(0, -visionConeAngle/2, 0) * eyeTransform.forward * maxVisionDistance;
        //Vector3 rightLine = Quaternion.Euler(0, visionConeAngle/2, 0) * eyeTransform.forward * maxVisionDistance;
        //Gizmos.DrawLine(eyeTransform.position, eyeTransform.position + leftLine);
        //Gizmos.DrawLine(eyeTransform.position, eyeTransform.position + rightLine);
        Gizmos.DrawLine(eyeTransform.position, eyeTransform.position + (eyeTransform.forward * maxVisionDistance));
    }


    
    private void UpdateSensoryData(){
        // Collect data from the world and store in variables

        // Cast rays to each part of the player's body if the player is within the vision cone and within maximum sight distance
        Vector3 directionToPlayer = playerHead.position - eyeTransform.position;

        canSeePlayer = false;
        if(directionToPlayer.magnitude <= maxVisionDistance){
            if(Vector3.Angle(eyeTransform.forward, directionToPlayer) <= visionConeAngle / 2){

                if(Physics.Raycast(eyeTransform.position, directionToPlayer, maxVisionDistance, visionObstructingLayers) == false){
                    canSeePlayer = true;
                }
            }
        }

        // Increment the amount of time the player has or has not been seen
        if (canSeePlayer){
            lastSeenPlayerPosition = playerHead.position;

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
            TrySetState(BehaviorState.Dead, true);
        }

        // Main priority: look for player, raise alarm level if player is seen
        if(canSeePlayer){
            switch (currentAlertnessLevel){

                case AlertnessLevel.Unwary:
                    currentAlertnessLevel = AlertnessLevel.Suspicious;
                    TrySetState(BehaviorState.Investigating);
                break;

                case AlertnessLevel.Suspicious:
                    TrySetState(BehaviorState.Investigating);
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


        // Set state to patrolling if no higher priority state has been selected
        TrySetState(BehaviorState.Patrolling);
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

    public void TrySetState(BehaviorState state, bool overridePriority = false){
        // Reset action queue and change state IF the desired state is of a higher priority OR override priority is set to true  

        if (IsPriorityHigher(state) || overridePriority == true){
            currentState = state;

            Debug.Log(state.ToString());
            // reset action queue goes here
        }
    }

}
