using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class LegManager : MonoBehaviour
{
    
    public List<FastIKFabric> legs;

    public List<Transform> restingTargets;

    public List<Transform> currentTargets;

    public Vector3 velocity;
    public float movingStepOffset;

    public bool useDynamicGait;

    public float minDisplacementToMove;
    public float stepCycleLength;
    public float stepDuration;

    private float timeSinceLastStep;
    private int currentLegIndex = 0;


    void Start()
    {
        for(int i = 0; i < legs.Count; i++){
            // Initialize leg targets to the positions of resting targets
            FastIKFabric currentLeg = legs[i];

            currentTargets.Add(Instantiate(restingTargets[i]));
            currentLeg.Target = currentTargets[i];
        }
    }

    void Update()
    {
        timeSinceLastStep += Time.deltaTime;

        // Move the current leg after its alotted time
        if(timeSinceLastStep >= stepCycleLength / legs.Count){
            timeSinceLastStep = 0;

            FastIKFabric currentLeg = legs[currentLegIndex];
            Transform currentTarget = currentTargets[currentLegIndex];

            Vector3 desiredPosition = GetDesiredPosition(currentLegIndex);
            float displacementFromDefault = Vector3.Distance(currentTarget.position, desiredPosition);

            if(displacementFromDefault >= minDisplacementToMove){
                currentTarget.position = desiredPosition;
                currentLeg.Target = currentTarget;
            }

            currentLegIndex++;
            if (currentLegIndex >= legs.Count){
                currentLegIndex = 0;
            }
        }

        // Evaluate each leg on whether it should move; if so, calculate new position and trigger transition between targets
        /*for(int i = 0; i < legs.Count; i++){
            FastIKFabric currentLeg = legs[i];
            Transform currentTarget = currentTargets[i];

            Vector3 desiredPosition = GetDesiredPosition(i);
            float displacementFromDefault = Vector3.Distance(currentTarget.position, desiredPosition);

            if(displacementFromDefault >= displacementToChangeTarget){
                currentTarget.position = desiredPosition;
                currentLeg.Target = currentTarget;
            }
        }*/
    }


    public Vector3 GetDesiredPosition(int legIndex){
        Vector3 restingPosition = restingTargets[legIndex].position;
        return restingPosition + (velocity * movingStepOffset);
    }
}
