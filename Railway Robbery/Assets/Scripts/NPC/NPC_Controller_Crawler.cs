using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Controller_Crawler : MonoBehaviour
{

    // NPC Properties
    [Header("References")] 
    public NPC npc;
    //public Transform testSphere;

    [Header("Patrol Properties")] 
    public float patrolWalkSpeed;
    public float patrolTurnSpeed;
    public float patrolAcceleration;
    public float patrolStoppingDistance;
    public float patrolWaypointRadius;
    
    [Header("Investigate Properties")] 
    public float investigateStoppingDistance;

    [Header("Pursuit Properties")] 
    public float pursuitStoppingDistance;
    public int numSearchAttempts;

    [Header("Attack Properties")] 
    public float minShootingRange;
    public float maxShootingRange;


    // Action queue variables
    private enum Action {
        Null,
        BeginMoveToTarget,
        WaitUntilTargetReached,
        RotateTowardsLookTarget,
        WaitForSeconds,
        Attack_Shoot
    }

    [SerializeField] private List<Action> actionQueue = new List<Action>();
    [SerializeField] private Action currentAction;
    private bool actionCompleted = true;


    // Action parameters
    [Header("Action Parameters")]
    [SerializeField] private Vector3 navigationTarget;
    [SerializeField] private Vector3 lookTarget = Vector3.one;
    [SerializeField] private float waitTimeRemaining;
    // Add variable to track how long it has been blocked by other npcs


    void Start() {
        npc = GetComponent<NPC>();
    }

    void Update() {
        EvaluateActionQueue();
        PerformCurrentAction();
    }


    public void EvaluateActionQueue(){
        
        // Removes current action from queue if it has finished, and select next action
        // Otherwise, do nothing (let current action continue)
        if (actionCompleted == true){
            RemoveCurrentAction();

            // Add new actions to the queue if it is empty
            if (actionQueue.Count == 0){
                switch(npc.currentState){
                
                    case NPC.BehaviorState.Idle:
                        // Queue idle animation
                    break;

                    case NPC.BehaviorState.Patrolling:
                        // Use navmeshagent to calculate a path to a random point in the train, then add actions to the queue for each waypoint in the path
                        Vector2 randomWaypoint = Random.insideUnitCircle * patrolWaypointRadius;
                        
                        navigationTarget = new Vector3(randomWaypoint.x + transform.position.x, transform.position.y, randomWaypoint.y + transform.position.z);
                        //testSphere.position = navigationTarget;

                        npc.navMeshAgent.speed = patrolWalkSpeed;
                        npc.navMeshAgent.angularSpeed = patrolTurnSpeed;
                        npc.navMeshAgent.acceleration = patrolAcceleration;
                        npc.navMeshAgent.stoppingDistance = patrolStoppingDistance;

                        waitTimeRemaining = Random.Range(0.5f, 1.5f);
                        AddAction(Action.WaitForSeconds);

                        //lookTarget = navigationTarget;
                        //AddAction(Action.RotateTowardsLookTarget);

                        AddAction(Action.BeginMoveToTarget);
                        AddAction(Action.WaitUntilTargetReached);

                    break;

                    case NPC.BehaviorState.Investigating:
                    // Get the GameObject and position of whatever alerted this NPC (thrown object, sound, sees player, etc)

                    // Play surprise animation, turn towards object

                    // Use navmeshagent to calculate a path to this point, then add actions to the queue for each waypoint in the path

                    // Investigate the object, then return to patrolling behavior state
                    break;

                    case NPC.BehaviorState.Pursuit:
                    // Get last known position of the player and calculate path

                    // Add waypoints to path and queue movement actions to waypoints

                    // Search for player by picking random search points in a radius until it hits the attempts required to give up
                    
                    // Add each set of waypoints to action queue, with a 'look around' animation in between each group of waypoints

                    // When finished, change behavior state to Idle
                    break;

                    case NPC.BehaviorState.Combat:
                    // Make a weighted choice between shooting, dodging, flanking

                    // Add occasional combat idle animations to space apart combat actions
                    break;

                    case NPC.BehaviorState.Immobilized:
                    // Start electrocuted/immobilized animation

                    // Set ragdoll / let physics take over

                    // Wait for x seconds

                    // Reinstate animation control, set state to Idle
                    break;

                    case NPC.BehaviorState.Dead:
                    break;

                    default:
                    break;
                }
            }

            UpdateCurrentAction();
            actionCompleted = false;

        }            
    } 


    public void PerformCurrentAction(){
        // Carries out scripted behavior for the current action, sets actionCompleted to true if prerequisites are met

        switch (currentAction){

            case Action.BeginMoveToTarget:
                // Triggers nav mesh agent to begin movement
                npc.navMeshAgent.destination = navigationTarget;
                npc.navMeshAgent.isStopped = false;

                actionCompleted = true;
            break;

            case Action.WaitUntilTargetReached:
                // Action is completed only if the npc has reached its destination or has been stopped
                if (npc.navMeshAgent.remainingDistance <= npc.navMeshAgent.stoppingDistance){
                    npc.navMeshAgent.isStopped = true;

                    actionCompleted = true;
                }
            break;

            case Action.RotateTowardsLookTarget:
                // Calculate which direction to turn around the y axis, and linearly interpolate rotation towards the first waypoint in the path
                npc.navMeshAgent.isStopped = true;

                Vector2 currLookDirection = new Vector2(transform.forward.x, transform.forward.z).normalized;

                //Vector3 targetDirection3D = navigationTarget - transform.position;
                //Vector2 targetDirection = new Vector2(targetDirection3D.x, targetDirection3D.z);

                Vector3 targetDirection3D = lookTarget - transform.position;
                Vector2 targetDirection = new Vector2(targetDirection3D.x, targetDirection3D.z).normalized;

                float originalAngleBetween = Vector2.SignedAngle(targetDirection, currLookDirection);

                float rotationAmount = originalAngleBetween.SignZero() * npc.navMeshAgent.angularSpeed * Time.deltaTime;
                transform.Rotate(0, rotationAmount, 0);

                float newAngleBetween = Vector2.SignedAngle(targetDirection, currLookDirection);

                // If the difference is zero or has changed sign, the target rotation has been reached
                if(newAngleBetween == 0 || newAngleBetween.SignZero() != originalAngleBetween.SignZero()){
                    transform.localEulerAngles -= new Vector3(0, newAngleBetween, 0);
                    actionCompleted = true;
                }        
                
            break;

            case Action.WaitForSeconds:
                // Wait for the stored number of seconds
                waitTimeRemaining -= Time.deltaTime;
                if(waitTimeRemaining <= 0){
                    actionCompleted = true;
                }
            break;

            case Action.Attack_Shoot:
                // bang bang
            break;
            
            default:
                actionCompleted = true;
            break;
        }
    }
    

    private void UpdateCurrentAction(){
        if (actionQueue.Count > 0){
            currentAction = actionQueue[0];
        }
        else{
            currentAction = Action.Null;
        }
    }

    private void AddAction(Action action){
        actionQueue.Add(action);
    }

    private void AddActionNext(Action action){
        actionQueue.Insert(1, action);
    }

    private void AddActionImmediate(Action action){
        actionQueue.Insert(0, action);
        UpdateCurrentAction();
    }

    private void AddActionAtIndex(Action action, int index){
        actionQueue.Insert(index, action);
        UpdateCurrentAction();
    }

    private void RemoveCurrentAction(){
        if (actionQueue.Count > 0){
            actionQueue.RemoveAt(0);
            UpdateCurrentAction();
        } 
    }
}
