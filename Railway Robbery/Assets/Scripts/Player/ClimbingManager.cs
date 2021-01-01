﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ClimbingManager : MonoBehaviour
{

    [Header("Climbing Settings")]
    [SerializeField] private float oneHandedSpringFrequency;
    [SerializeField] private float twoHandedSpringFrequency;
    [SerializeField] private float springDamping;

    [Header("Haptic Settings")]
    [SerializeField] [Range(0, 1)] private float climbingHapticFrequency;
    [SerializeField] [Range(0, 1)] private float climbingHapticMaxAmplitude;
    [SerializeField] [Range(0, 1)] private float climbingHapticBaseGradient;
    [SerializeField] private float maxHapticDistance;

    [Header("References")]
    //public ControllerCollisionTrigger leftClimbingTrigger;
    //public ControllerCollisionTrigger rightClimbingTrigger;
    private BodyPartReferences bodyParts;
    private ClimbingHand leftHand;
    private ClimbingHand rightHand;
    

    // Variables
    private float currentSpringFrequency;
    private Vector3 leftBodyTarget;
    private Vector3 rightBodyTarget;
    private Vector3 mainBodyTarget;



    void Awake() {
        bodyParts = GetComponent<BodyPartReferences>();

        leftHand = bodyParts.leftClimbingHand;
        rightHand = bodyParts.rightClimbingHand;
    }
    

    void FixedUpdate()
    { 

        // Update the climbing state and targets for both hands
        UpdateClimbingHandState(true);
        UpdateClimbingHandState(false);


        // Calculate the target body position based on which hands are climbing
        if (leftHand.isClimbing && rightHand.isClimbing){
            mainBodyTarget = (leftBodyTarget + rightBodyTarget) / 2;

            currentSpringFrequency = twoHandedSpringFrequency;
            bodyParts.playerRigidbody.useGravity = false;
        }

        else if(leftHand.isClimbing){
            mainBodyTarget = leftBodyTarget;

            currentSpringFrequency = oneHandedSpringFrequency;
            bodyParts.playerRigidbody.useGravity = false;       
        }

        else if (rightHand.isClimbing){
            mainBodyTarget = rightBodyTarget;

            currentSpringFrequency = oneHandedSpringFrequency;
            bodyParts.playerRigidbody.useGravity = false;
        }

        else{
            mainBodyTarget = transform.position;

            currentSpringFrequency = 0;
            bodyParts.playerRigidbody.useGravity = true;
        }


        Vector3 averageClimbedVelocity = Vector3.zero;
        int numClimbedRigidbodies = 0;
        if(leftHand.climbedRigidbody){
            averageClimbedVelocity += leftHand.climbedRigidbody.velocity;
            numClimbedRigidbodies++;
        }
        if(rightHand.climbedRigidbody){
            averageClimbedVelocity += rightHand.climbedRigidbody.velocity;
            numClimbedRigidbodies++;
        }
        if(numClimbedRigidbodies != 0) averageClimbedVelocity = averageClimbedVelocity / numClimbedRigidbodies;


        Vector3 playerRelativeVelocity = bodyParts.playerRigidbody.velocity - averageClimbedVelocity;


        Vector3 bodySpringForce = DampedSpring.GetDampedSpringAcceleration(
            transform.position, 
            mainBodyTarget, 
            playerRelativeVelocity,
            currentSpringFrequency,
            springDamping
        );


        // Applies forces to player and climbed object differently based on the state of the player and the climbed object(s)
        bool isClimbing = leftHand.isClimbing || rightHand.isClimbing;
        bool isClimbingStatic = (leftHand.isClimbing && !leftHand.climbedRigidbody) || 
                              (rightHand.isClimbing && !rightHand.climbedRigidbody);
        bool isGrounded = bodyParts.groundedStateTracker.isGrounded;


        if(isClimbing){

            // While climbing on a static surface, simply move player to where they should be
            if(isClimbingStatic){
                bodyParts.playerRigidbody.AddForce(bodySpringForce, ForceMode.Acceleration);
            }

            // Move the player if climbing on a dynamic object, while holding with both hands or not touching the ground
            else if(!isGrounded || (leftHand.climbedRigidbody && rightHand.climbedRigidbody)){
                bodyParts.playerRigidbody.AddForce(bodySpringForce, ForceMode.Acceleration);
            }


            // Apply hand forces to any climbed rigidbodies
            if(leftHand.climbedRigidbody || rightHand.climbedRigidbody){

                if(leftHand.climbedRigidbody){
                    Vector3 leftHandForce = DampedSpring.GetDampedSpringAcceleration(
                        transform.position, 
                        leftBodyTarget,
                        playerRelativeVelocity,
                        currentSpringFrequency,
                        springDamping
                    );

                    //Vector3 leftHandTorque = DampedSpring.GetDampedSpringAngularAcceleration(
                        //leftHand.climbingAnchor.rotation,
                        //bodyParts.leftControllerTransform.rotation,
                        //leftHand.climbedRigidbody,

                    //);

                    leftHand.climbedRigidbody.AddForceAtPosition(-leftHandForce / numClimbedRigidbodies, leftHand.climbingAnchor.position, ForceMode.Acceleration);
                } 
                if(rightHand.climbedRigidbody){
                    Vector3 rightHandForce = DampedSpring.GetDampedSpringAcceleration(
                        transform.position,
                        rightBodyTarget,
                        playerRelativeVelocity,
                        currentSpringFrequency,
                        springDamping
                    );

                    rightHand.climbedRigidbody.AddForceAtPosition(-rightHandForce / numClimbedRigidbodies, rightHand.climbingAnchor.position, ForceMode.Acceleration);
                } 
            }
        }
    }



    private void UpdateClimbingHandState(bool isLeft){
        // Continue climbing if grip is held and the hand is touching climbable geometry / was previously climbing. 
        // Otherwise, update anchor positions and unfreeze the physics hand

        Transform controllerTransform = isLeft ? bodyParts.leftControllerTransform : bodyParts.rightControllerTransform;
        ClimbingHand climbingHand = isLeft ? bodyParts.leftClimbingHand : bodyParts.rightClimbingHand;
        Hand autoHand = isLeft ? bodyParts.leftHand : bodyParts.rightHand;
        //ControllerCollisionTrigger climbingTrigger = isLeft ? leftClimbingTrigger : rightClimbingTrigger;

        if ((climbingHand.isTriggerColliding || climbingHand.isClimbing) 
        && bodyParts.inputHandler.GetHeldState(climbingHand.grabButton) 
        && !climbingHand.IsHoldingObject())
        {
            // Detect if this is the first frame climbing is initiated
            if(climbingHand.isClimbing == false){
                StartClimbing(autoHand, climbingHand);
            }

            climbingHand.FollowClimbingAnchor();

            autoHand.disableIK = true;
            autoHand.SetGrip(autoHand.gripOffset);

            //Vector3 targetPosition = transform.position - (controllerTransform.position - climbingHand.controllerAnchorPosition);
            Vector3 targetPosition = transform.position - (controllerTransform.position - climbingHand.climbingAnchor.position);

            if (isLeft) { leftBodyTarget = targetPosition; }
            else { rightBodyTarget = targetPosition; }

            float handDistance = (targetPosition - transform.position).magnitude;
            float gradient = Mathf.Clamp(handDistance / maxHapticDistance, climbingHapticBaseGradient, 1);
            autoHand.SetHaptics(climbingHapticFrequency, climbingHapticMaxAmplitude * gradient);
        }
        else{
            // Detect if climbing was just released this frame
            if(climbingHand.isClimbing){
                StopClimbing(autoHand, climbingHand);
            }

            autoHand.disableIK = false;

            climbingHand.UpdateControllerAnchor();
            climbingHand.UpdateHandAnchor();
        }

    }


    private void StartClimbing(Hand autoHand, ClimbingHand climbingHand){
        autoHand.Release();
        climbingHand.OnClimbingStart();
    }

    private void StopClimbing(Hand autoHand, ClimbingHand climbingHand){
        autoHand.ClearHaptics();
        climbingHand.OnClimbingStop();
    }
}
