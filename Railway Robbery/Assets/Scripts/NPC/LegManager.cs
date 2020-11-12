using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class LegManager : MonoBehaviour
{

    public List<FastIKFabric> legs;
    public List<Transform> restingTargets;

    public float movingStepOffset;
    public float minDisplacementToMove;
    public float stepCycleLength;
    public float stepAnimationDuration;
    public float stepHeight;
    public AnimationCurve stepHeightCurve;

    public Vector3 velocity;
    public bool useDynamicGait;

    //private List<Transform> currentTargets;
    private float timeSinceLastStep;
    private int currentLegIndex = 0;


    void Start()
    {
        for(int i = 0; i < legs.Count; i++){
            // Initialize leg targets to the positions of resting targets
            FastIKFabric currentLeg = legs[i];

            Transform currentTarget = Instantiate(restingTargets[i], restingTargets[i].position, restingTargets[i].rotation);
            currentLeg.Target = currentTarget;
        }
    }

    void Update()
    {
        timeSinceLastStep += Time.deltaTime;

        // Move the current leg after its alotted time
        if(timeSinceLastStep >= stepCycleLength / legs.Count){
            timeSinceLastStep = 0;

            FastIKFabric currentLeg = legs[currentLegIndex];
            Transform currentTarget = currentLeg.Target;//currentTargets[currentLegIndex];

            Vector3 desiredPosition = GetDesiredPosition(currentLegIndex);
            float displacementFromDefault = Vector3.Distance(currentTarget.position, desiredPosition);

            if(displacementFromDefault >= minDisplacementToMove){
                StartCoroutine(MoveLeg(currentLegIndex, desiredPosition));
                //currentTarget.position = desiredPosition;
                //currentLeg.Target = currentTarget;
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

    IEnumerator MoveLeg(int legIndex, Vector3 newPosition){
        // Moves the given leg along a path defined by direction to the new target and the step animation curve
        FastIKFabric currentLeg = legs[legIndex];

        Vector3 oldPosition = currentLeg.Target.position;
        Vector3 totalDisplacement = newPosition - oldPosition;

        float percent = 0;
        while (percent <= 1){
            percent = Mathf.Clamp01(percent);

            float currentHeight = stepHeightCurve.Evaluate(percent) * stepHeight;
            Vector3 currentDirection = percent * totalDisplacement;

            Vector3 currentPosition = oldPosition + currentDirection;
            currentPosition.y += currentHeight;

            currentLeg.Target.position = currentPosition;

            percent += Time.deltaTime / stepAnimationDuration;
            yield return null;
        }

        yield break;
    }
}
