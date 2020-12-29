using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ClimbingManager : MonoBehaviour
{

    [Header("Climbing Settings")]
    [SerializeField] private float oneHandedSpringFrequency;
    [SerializeField] private float twoHandedSpringFrequency;
    [SerializeField] private float springDamping;
    private string climbingTag = "Climbable";

    [Header("Haptic Settings")]
    [SerializeField] [Range(0, 1)] private float climbingHapticFrequency;
    [SerializeField] [Range(0, 1)] private float climbingHapticMaxAmplitude;
    [SerializeField] [Range(0, 1)] private float climbingHapticBaseGradient;
    [SerializeField] private float maxHapticDistance;

    [Header("References")]
    public ControllerCollisionTrigger leftClimbingTrigger;
    public ControllerCollisionTrigger rightClimbingTrigger;
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


        Vector3 bodySpringForce = DampedSpring.GetDampedSpringAcceleration(
            transform.position, 
            mainBodyTarget, 
            bodyParts.playerRigidbody.velocity - Vector3.zero,
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


            // If a dynamic object is being climbed on, apply a force to it if the player is grounded or also climbing a static surface
            if(isGrounded || isClimbingStatic){
                leftHand.climbedRigidbody?.AddForceAtPosition(-bodySpringForce, leftHand.climbingAnchor.position, ForceMode.Force);
                rightHand.climbedRigidbody?.AddForceAtPosition(-bodySpringForce, rightHand.climbingAnchor.position, ForceMode.Force);
            }
            else{
                //Vector3 bodyTargetOffset = mainBodyTarget - transform.position;

                //Vector3 leftHandOffset;
            }


            /*bool isClimbingStatic = false;
            if((leftHand.isClimbing && !leftHand.climbedRigidbody) || (rightHand.isClimbing && !rightHand.climbedRigidbody)){
                isClimbingStatic = true;
            }

            int numSurfacesClimbed = 0;
            if(leftHand.climbedRigidbody) numSurfacesClimbed++;
            if(rightHand.climbedRigidbody) numSurfacesClimbed++;
            //if(leftHand.climbedRigidbody == rightHand.climbedRigidbody) numSurfacesClimbed = 1;


            // Climbing on a static surface only
            if(!leftHand.climbedRigidbody && !rightHand.climbedRigidbody){
                bodyParts.playerRigidbody.AddForce(bodySpringForce, ForceMode.Acceleration);    
            }

            // Climbing on a mix between static and dynamic objects
            else if(isClimbingStatic){
                leftHand.climbedRigidbody?.AddForce(-bodySpringForce / numSurfacesClimbed, ForceMode.Acceleration);
                rightHand.climbedRigidbody?.AddForce(-bodySpringForce / numSurfacesClimbed, ForceMode.Acceleration);

                bodyParts.playerRigidbody.AddForce(bodySpringForce, ForceMode.Acceleration);
            }

            // Climbing on dynamic objects only
            else{
                leftHand.climbedRigidbody?.AddForce(-bodySpringForce / numSurfacesClimbed, ForceMode.Acceleration);
                rightHand.climbedRigidbody?.AddForce(-bodySpringForce / numSurfacesClimbed, ForceMode.Acceleration);

                if(!isGrounded){
                    bodyParts.playerRigidbody.AddForce(bodySpringForce, ForceMode.Acceleration);
                }
            }*/

        }

    }



    private void UpdateClimbingHandState(bool isLeft){
        // Continue climbing if grip is held and the hand is touching climbable geometry / was previously climbing. 
        // Otherwise, update anchor positions and unfreeze the physics hand

        Transform controllerTransform = isLeft ? bodyParts.leftControllerTransform : bodyParts.rightControllerTransform;
        ClimbingHand climbingHand = isLeft ? bodyParts.leftClimbingHand : bodyParts.rightClimbingHand;
        Hand autoHand = isLeft ? bodyParts.leftHand : bodyParts.rightHand;
        ControllerCollisionTrigger climbingTrigger = isLeft ? leftClimbingTrigger : rightClimbingTrigger;

        if ((climbingTrigger.isColliding || climbingHand.isClimbing) 
        && bodyParts.inputHandler.GetHeldState(climbingHand.grabButton) 
        && !climbingHand.IsHoldingObject())
        {
            // Detect if this is the first frame climbing is initiated
            if(climbingHand.isClimbing == false){
                StartClimbing(autoHand, climbingHand, climbingTrigger.collidingTransform, climbingTrigger);
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


    private void StartClimbing(Hand autoHand, ClimbingHand climbingHand, Transform climbingParent, ControllerCollisionTrigger climbingTrigger){
        autoHand.Release();
        climbingHand.OnClimbingStart(climbingParent);

        Rigidbody climbedRigidbody = climbingTrigger.collidingTransform.GetComponent<Rigidbody>();
        if(climbedRigidbody){
            climbingHand.climbedRigidbody = climbedRigidbody;
        }
        else{
            climbingHand.climbedRigidbody = null;
        }
    }

    private void StopClimbing(Hand autoHand, ClimbingHand climbingHand){
        autoHand.ClearHaptics();
        climbingHand.OnClimbingStop();
    }
}
