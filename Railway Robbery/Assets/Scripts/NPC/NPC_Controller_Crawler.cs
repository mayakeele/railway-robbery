﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Controller_Crawler : NPC
{

    public List<Vector3> navigationWaypoints;

    public float walkSpeed = 0.75f;
    public float runSpeed = 1.5f;


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

                    case BehaviorState.Curious:
                    break;

                    case BehaviorState.Alerted_Pursuit:
                    break;

                    case BehaviorState.Alerted_Combat:
                    break;

                    case BehaviorState.Attacking:
                    break;

                    case BehaviorState.Immobilized:
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
