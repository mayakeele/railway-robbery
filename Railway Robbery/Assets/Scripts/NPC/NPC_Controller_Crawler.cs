using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller_Crawler : NPC
{

    public List<Vector3> navigationWaypoints;

    public float walkSpeed = 0.75f;
    public float runSpeed = 1.5f;

    public float minShootingRange = 1f;
    public float maxShootingRange = 5f;

    public int numSearchAttempts = 5;


    public enum Action {
        Null,
        CalculateWaypoints,
        MoveToNextWaypoint,
        Attack_Shoot

    }

    public List<Action> actionQueue = new List<Action>();
    public Action currentAction;
    public bool actionCompleted = false;



    public void EvaluateActionQueue(){
        
        // Removes current action from queue if it has finished, and select next action
        if (actionCompleted == true){
            RemoveCurrentAction();

            // Add new actions to the queue if it is empty
            if (actionQueue.Count == 0){
                switch(currentState){
                
                    case BehaviorState.Idle:
                        // Queue idle animation
                    break;

                    case BehaviorState.Patrolling:
                        // Use navmeshagent to calculate a path to a random point in the train, then add actions to the queue for each waypoint in the path
                        Vector3 targetLocation = new Vector3();
                        navigationWaypoints = new List<Vector3>();

                        for (int i = 0; i < navigationWaypoints.Count; i++){
                            AddAction(Action.MoveToNextWaypoint);
                        }
                    break;

                    case BehaviorState.Investigating:
                    // Get the GameObject and position of whatever alerted this NPC (thrown object, sound, sees player, etc)

                    // Play surprise animation, turn towards object

                    // Use navmeshagent to calculate a path to this point, then add actions to the queue for each waypoint in the path

                    // Investigate the object, then return to patrolling behavior state
                    break;

                    case BehaviorState.Pursuit:
                    // Get last known position of the player and calculate path

                    // Add waypoints to path and queue movement actions to waypoints

                    // Search for player by picking random search points in a radius until it hits the attempts required to give up
                    
                    // Add each set of waypoints to action queue, with a 'look around' animation in between each group of waypoints

                    // When finished, change behavior state to Idle
                    break;

                    case BehaviorState.Combat:
                    // Make a weighted choice between shooting, dodging, flanking

                    // Add occasional combat idle animations to space apart combat actions
                    break;

                    case BehaviorState.Immobilized:
                    // Play electrocuted/immobilized animation

                    // Set ragdoll / let physics take over

                    // Wait for x seconds

                    // Reinstate animation control, set state to Idle
                    break;

                    case BehaviorState.Dead:
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
            case Action.CalculateWaypoints:
                // foo
            break;

            case Action.MoveToNextWaypoint:
                Vector3 currentWaypoint = navigationWaypoints[0];
                Vector3 directionToWayPoint = (currentWaypoint - transform.position).normalized;

                rb.velocity = directionToWayPoint * walkSpeed * Time.deltaTime;
            break;

            case Action.Attack_Shoot:
                // foo
                Vector3 shootingDirection = new Vector3();
            break;
            
            default:
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
        actionQueue.RemoveAt(0);
        UpdateCurrentAction();
    }
}
