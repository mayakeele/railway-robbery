using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class LegManager : MonoBehaviour
{
    [Header("References")]
    public List<FastIKFabric> legs;
    public List<Transform> restingTargets;
    public Transform bodyTransform;
    public List<Transform> legRoots;

    [Header("Movement Properties")]
    public float movingStepOffset;
    public float minDisplacementToMove;
    public bool rotateBodyWithLegs;
    public float bodyRotationRate;

    [Header("Step Animation Properties")]
    public float stepCycleLength;
    public float stepAnimationDuration;
    public float stepAnimationHeight;
    public AnimationCurve stepHeightCurve;

    [Header("Step Calculation Properties")]
    public float maxFootHeightDisplacement;
    public LayerMask walkableLayers;
    public LayerMask obstacleLayers;
    public bool canStepOnObstacles;
    public int maxRaycastsPerStep;
    public float maxObstructedStepInset;
    

    [Header("Variables")]
    public Vector3 velocity;
    public bool useDynamicGait;
    public Vector3 currentUpDirection;

    private float timeSinceLastStep;
    private int currentLegIndex = 0;


    void Start()
    {
        GameObject targetHolder = new GameObject("NPC Leg Targets");
        for(int i = 0; i < legs.Count; i++){
            // Initialize leg targets to the positions of resting targets
            FastIKFabric currentLeg = legs[i];

            restingTargets[i].position = currentLeg.transform.position;

            Transform currentTarget = Instantiate(restingTargets[i], restingTargets[i].position, restingTargets[i].rotation);
            currentTarget.SetParent(targetHolder.transform);
            currentLeg.Target = currentTarget;
        }

        currentUpDirection = Vector3.up;
        SetLegRoots();
    }

    void Update()
    {
        timeSinceLastStep += Time.deltaTime;

        // Move the current leg after its alotted time
        if(timeSinceLastStep >= stepCycleLength / legs.Count){
            timeSinceLastStep = 0;

            FastIKFabric currentLeg = legs[currentLegIndex];
            Transform currentTarget = currentLeg.Target;//currentTargets[currentLegIndex];

            Vector3 desiredPosition = CastToGround(currentLegIndex, out Vector3 groundNormal);
            float displacementFromDefault = Vector3.Distance(currentTarget.position, desiredPosition);

            if(displacementFromDefault >= minDisplacementToMove){
                StartCoroutine(MoveLeg(currentLegIndex, desiredPosition, groundNormal));
                //currentTarget.position = desiredPosition;
                //currentLeg.Target = currentTarget;
            }

            currentLegIndex++;
            if (currentLegIndex >= legs.Count){
                currentLegIndex = 0;
            }
        }

        if(rotateBodyWithLegs){
            float rollAngle = CalculateBodyAngle(transform.forward, true);
            float pitchAngle = CalculateBodyAngle(transform.right, true);
            Quaternion desiredBodyRotation = Quaternion.Euler(pitchAngle, 0, rollAngle);
            bodyTransform.localRotation = Quaternion.Lerp(bodyTransform.localRotation, desiredBodyRotation, bodyRotationRate);
        }
        
    }


    public Vector3 GetDesiredPosition(int legIndex){
        // Finds the optimal position for this leg to be in
        Vector3 restingPosition = restingTargets[legIndex].position;
        return restingPosition + (velocity * movingStepOffset);
    }

    public Vector3 CastToGround(int legIndex, out Vector3 groundNormal){
        // Casts a ray down and through the desired position to find solid ground
        FastIKFabric currentLeg = legs[legIndex];

        Vector3 desiredPosition = GetDesiredPosition(legIndex);
        Vector3 rootPosition = legRoots[legIndex].position;
        Vector3 inwardsDirection = (maxObstructedStepInset / maxRaycastsPerStep) * Vector3.ProjectOnPlane(transform.position - currentLeg.Target.position, currentUpDirection);
        
        Vector3 targetPosition = desiredPosition;
        for(int i = 0; i < maxRaycastsPerStep; i++){
            // Cast a ray down from above the desired position to find solid ground
            Ray groundRay = new Ray(new Vector3(targetPosition.x, targetPosition.y + maxFootHeightDisplacement, targetPosition.z), -currentUpDirection);
            if (Physics.Raycast(groundRay, out RaycastHit groundHitInfo, maxFootHeightDisplacement * 2, walkableLayers)){

                // Check if the current target is inside of an obstacle; if so, move foot to wall of obstacle instead
                Ray obstacleRay = new Ray(rootPosition, targetPosition - rootPosition);
                if (Physics.Raycast(obstacleRay, out RaycastHit obstacleHitInfo, Vector3.Distance(targetPosition, rootPosition), obstacleLayers)){
                    if(canStepOnObstacles){
                        groundNormal = obstacleHitInfo.normal;
                        return obstacleHitInfo.point;
                    }
                    else{
                        targetPosition += inwardsDirection;
                    }        
                }
                else{
                    groundNormal = groundHitInfo.normal;
                    return groundHitInfo.point;
                }
       
            }
            else{
                targetPosition += inwardsDirection;
            }
        }

        // If no valid position is available, move the leg to the closest possible position
        groundNormal = currentUpDirection;
        return targetPosition;
        
    }

    IEnumerator MoveLeg(int legIndex, Vector3 newPosition, Vector3 newNormal){
        // Moves the given leg along a path defined by direction to the new target and the step animation curve
        FastIKFabric currentLeg = legs[legIndex];

        Vector3 oldPosition = currentLeg.Target.position;
        Vector3 totalDisplacement = newPosition - oldPosition;

        newNormal.Normalize();

        float percent = 0;
        while (percent <= 1){
            percent = Mathf.Clamp01(percent);

            Vector3 currentHeight = stepHeightCurve.Evaluate(percent) * stepAnimationHeight * newNormal;
            Vector3 currentDirection = percent * totalDisplacement;

            Vector3 currentPosition = oldPosition + currentDirection + currentHeight;

            currentLeg.Target.position = currentPosition;

            percent += Time.deltaTime / stepAnimationDuration;
            yield return null;
        }

        yield break;
    }

    float CalculateBodyAngle(Vector3 axis, bool includeMovingLeg){
        // Calculates the average angle of leg displacement around the forward axis
        float averageAngle = 0;
        
        for(int i = 0; i < legs.Count; i++){
            if(includeMovingLeg == true || i != currentLegIndex){
                FastIKFabric currentLeg = legs[i];

                Vector3 defaultPosition = restingTargets[i].position;
                Vector3 currentPosition = new Vector3(defaultPosition.x, currentLeg.Target.position.y, defaultPosition.z);

                // Angle of displacement is the angle between a vector from center to default and a vector from center to current
                Vector3 toDefault = Vector3.ProjectOnPlane(defaultPosition - bodyTransform.position, axis);
                Vector3 toTarget = Vector3.ProjectOnPlane(currentPosition - bodyTransform.position, axis);

                float angle;
                angle = Vector3.SignedAngle(toDefault, toTarget, axis);
                averageAngle += angle;
            }    
        }

        return includeMovingLeg ? averageAngle / legs.Count : averageAngle / (legs.Count - 1);
    }

    void SetLegRoots(){
        // Find the nth parent of each leg (the 1st bone in the chain) and store in legRoots
        for(int l = 0; l < legs.Count; l++){
            FastIKFabric currentLeg = legs[l];

            Transform currentJoint = currentLeg.transform;
            for(int i = 0; i < currentLeg.ChainLength; i++){
                currentJoint = currentJoint.parent;
            }

            legRoots.Add(currentJoint);
        }
    }

}
