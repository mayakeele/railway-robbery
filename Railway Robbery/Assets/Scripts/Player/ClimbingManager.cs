using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    private InputHandler inputHandler;

    [SerializeField] private float oneHandedSpringFrequency;
    [SerializeField] private float twoHandedSpringFrequency;
    private float currentSpringFrequency;
    [SerializeField] private float springDamping;

    [SerializeField] private float handRadius;

    [HideInInspector] public PhysicsHand leftPhysicsHand;
    [HideInInspector] public PhysicsHand rightPhysicsHand;
    private ControllerCollisionTrigger leftControllerCollider;
    private ControllerCollisionTrigger rightControllerCollider;

    private Vector3 leftBodyTarget;
    private Vector3 rightBodyTarget;
    private Vector3 mainBodyTarget;


    void Start()
    {
        inputHandler = GetComponent<InputHandler>();

        leftPhysicsHand = inputHandler.leftPhysicsHand.GetComponent<PhysicsHand>();
        rightPhysicsHand = inputHandler.rightPhysicsHand.GetComponent<PhysicsHand>();
    }


    void Update()
    {
        // Continue climbing if grip is held and the hand is touching climbable geometry / was previously climbing. Otherwise, update anchor positions and unfreeze the physics hand

        if ((leftPhysicsHand.isColliding || leftPhysicsHand.isClimbing) && inputHandler.GetHeldState(InputHandler.InputButton.L_Grip)){

            leftPhysicsHand.isClimbing = true;
            leftPhysicsHand.rb.isKinematic = true;
            leftPhysicsHand.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            leftPhysicsHand.transform.position = leftPhysicsHand.physicsHandPositionAnchor;
            leftPhysicsHand.transform.rotation = leftPhysicsHand.physicsHandRotationAnchor;

            leftBodyTarget = transform.position - (inputHandler.leftController.position - leftPhysicsHand.controllerAnchor); //leftPhysicsHand.controllerAnchor + leftPhysicsHand.controllerToBodyOffset;
        }
        else{
            
            leftPhysicsHand.isClimbing = false;
            leftPhysicsHand.rb.isKinematic = false;
            leftPhysicsHand.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

            leftPhysicsHand.controllerAnchor = inputHandler.leftController.transform.position;
            leftPhysicsHand.physicsHandPositionAnchor = leftPhysicsHand.transform.position;
            leftPhysicsHand.physicsHandRotationAnchor = leftPhysicsHand.transform.rotation;      
        }


        if ((rightPhysicsHand.isColliding || rightPhysicsHand.isClimbing) && inputHandler.GetHeldState(InputHandler.InputButton.R_Grip)){
            
            rightPhysicsHand.isClimbing = true;
            rightPhysicsHand.rb.isKinematic = true;
            rightPhysicsHand.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            rightPhysicsHand.transform.position = rightPhysicsHand.physicsHandPositionAnchor;
            rightPhysicsHand.transform.rotation = rightPhysicsHand.physicsHandRotationAnchor;

            rightBodyTarget = transform.position - (inputHandler.rightController.position - rightPhysicsHand.controllerAnchor); //rightPhysicsHand.controllerAnchor + rightPhysicsHand.controllerToBodyOffset;      
        }
        else{

            rightPhysicsHand.isClimbing = false;
            rightPhysicsHand.rb.isKinematic = false;
            rightPhysicsHand.rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

            rightPhysicsHand.controllerAnchor = inputHandler.rightController.transform.position;
            rightPhysicsHand.physicsHandPositionAnchor = rightPhysicsHand.transform.position;
            rightPhysicsHand.physicsHandRotationAnchor = rightPhysicsHand.transform.rotation;
        }



        // If both hands are holding climbable geometry, calculate the average displacement of each one to determine target body position
        if (leftPhysicsHand.isClimbing && rightPhysicsHand.isClimbing){
            mainBodyTarget = (leftBodyTarget + rightBodyTarget) / 2;

            currentSpringFrequency = twoHandedSpringFrequency;
            inputHandler.playerRigidbody.useGravity = false;
        }
        else if(leftPhysicsHand.isClimbing){
            mainBodyTarget = leftBodyTarget;

            currentSpringFrequency = oneHandedSpringFrequency;
            inputHandler.playerRigidbody.useGravity = false;
        }
        else if (rightPhysicsHand.isClimbing){
            mainBodyTarget = rightBodyTarget;

            currentSpringFrequency = oneHandedSpringFrequency;
            inputHandler.playerRigidbody.useGravity = false;
        }
        else{
            mainBodyTarget = transform.position;

            currentSpringFrequency = 0;
            inputHandler.playerRigidbody.useGravity = true;
        }


        Vector3 bodySpringForce = DampedSpring.GetDampedSpringAcceleration(
            transform.position, 
            mainBodyTarget, 
            inputHandler.playerRigidbody.velocity - Vector3.zero,
            currentSpringFrequency, 
            springDamping
        );

        if (leftPhysicsHand.isClimbing || rightPhysicsHand.isClimbing){
            inputHandler.playerRigidbody.AddForce(bodySpringForce, ForceMode.Acceleration);
        }    
    }
}
