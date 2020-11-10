using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class LegManager : MonoBehaviour
{
    public float displacementToChangeTarget;
    public List<FastIKFabric> legs;

    public List<Transform> restingTargets;

    public List<Transform> currentTargets;
    public List<bool> isLegMoving;

    public Vector3 velocity;
    public float movingStepOffset;

    public float stepDuration;


    void Start()
    {
        for(int i = 0; i < legs.Count; i++){
            // Initialize leg targets to the positions of resting targets
            FastIKFabric currentLeg = legs[i];

            currentTargets.Add(Instantiate(restingTargets[i]));
            currentLeg.Target = currentTargets[i];

            isLegMoving[i] = false;
        }
    }

    void Update()
    {
        // Evaluate each leg on whether it should move; if so, calculate new position and trigger transition between targets
        for(int i = 0; i < legs.Count; i++){
            FastIKFabric currentLeg = legs[i];
            Transform currentTarget = currentTargets[i];

            Vector3 desiredPosition = GetDesiredPosition(i);
            float displacementFromDefault = Vector3.Distance(currentTarget.position, desiredPosition);

            if(displacementFromDefault >= displacementToChangeTarget){
                currentTarget.position = desiredPosition;
                currentLeg.Target = currentTarget;
            }
        }
    }


    public Vector3 GetDesiredPosition(int legIndex){
        Vector3 restingPosition = restingTargets[legIndex].position;
        return restingPosition + (velocity * movingStepOffset);
    }
}
