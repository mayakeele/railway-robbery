using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ClimbingManager : MonoBehaviour
{
    private BodyPartReferences bodyParts;

    [SerializeField] private float oneHandedSpringFrequency;
    [SerializeField] private float twoHandedSpringFrequency;
    private float currentSpringFrequency;
    [SerializeField] private float springDamping;

    [SerializeField] private float handRadius;

    private string climbingTag = "Climbable";

    private ClimbingHand leftHand;
    private ClimbingHand rightHand;

    private ControllerCollisionTrigger leftControllerCollider;
    private ControllerCollisionTrigger rightControllerCollider;

    private Vector3 leftBodyTarget;
    private Vector3 rightBodyTarget;
    private Vector3 mainBodyTarget;


    void Awake() {
        bodyParts = GetComponent<BodyPartReferences>();

        leftHand = bodyParts.leftClimbingHand;
        rightHand = bodyParts.rightClimbingHand;
    }
    

    void Update()
    {
        // Update the climbing state and targets for both hands
        UpdateClimbingHandState(true);
        UpdateClimbingHandState(false);

        // If both hands are holding climbable geometry, calculate the average displacement of each one to determine target body position
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

        if (leftHand.isClimbing || rightHand.isClimbing){
            bodyParts.playerRigidbody.AddForce(bodySpringForce, ForceMode.Acceleration);
        }    
    }

    private void UpdateClimbingHandState(bool isLeft){
        // Continue climbing if grip is held and the hand is touching climbable geometry / was previously climbing. 
        // Otherwise, update anchor positions and unfreeze the physics hand

        Transform controllerTransform = isLeft ? bodyParts.leftControllerTransform : bodyParts.rightControllerTransform;
        ClimbingHand climbingHand = isLeft ? bodyParts.leftClimbingHand : bodyParts.rightClimbingHand;
        Hand autoHand = isLeft ? bodyParts.leftHand : bodyParts.rightHand;

        if ((climbingHand.collidingTag == climbingTag || climbingHand.isClimbing) && bodyParts.inputHandler.GetHeldState(climbingHand.grabButton)){
            climbingHand.isClimbing = true;
            climbingHand.Freeze();

            Vector3 targetPosition = transform.position - (controllerTransform.position - climbingHand.climbingAnchorPosition); //leftHand.controllerAnchor + leftHand.controllerToBodyOffset;

            if (isLeft) { leftBodyTarget = targetPosition; }
            else { rightBodyTarget = targetPosition; }
        }
        else{
            climbingHand.isClimbing = false;
            climbingHand.Unfreeze();

            climbingHand.UpdateClimbingAnchor();
        }

    }

}
