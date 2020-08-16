using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    private InputHandler inputHandler;

    [SerializeField] private float handRadius = 0.08f;
    [SerializeField] private int armForce = 900; // (in newtons)
    [SerializeField] private int handForce; // (in newtons)
    [SerializeField] private int bodyWeight = 625; // (in newtons)


    private PhysicsHand leftPhysicsHand;
    private PhysicsHand rightPhysicsHand;
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


    void LateUpdate()
    {
        // Continue climbing if grip is held and the hand is touching climbable geometry / was previously climbing. Otherwise, update anchor positions and unfreeze the physics hand

        if ((leftPhysicsHand.isColliding || leftPhysicsHand.isClimbing) && inputHandler.GetHeldState(InputHandler.InputButton.L_Grip)){

            leftPhysicsHand.isClimbing = true;
            leftPhysicsHand.rb.isKinematic = true;

            leftPhysicsHand.transform.position = leftPhysicsHand.physicsHandPositionAnchor;
            leftPhysicsHand.transform.rotation = leftPhysicsHand.physicsHandRotationAnchor;
            
            inputHandler.playerRigidbody.velocity = Vector3.zero;

            leftBodyTarget = leftPhysicsHand.controllerAnchor + leftPhysicsHand.controllerToBodyOffset;
        }
        else{
            
            leftPhysicsHand.isClimbing = false;
            leftPhysicsHand.rb.isKinematic = false;

            leftPhysicsHand.controllerAnchor = inputHandler.leftControllerAnchor.transform.position;
            leftPhysicsHand.physicsHandPositionAnchor = leftPhysicsHand.transform.position;
            leftPhysicsHand.physicsHandRotationAnchor = leftPhysicsHand.transform.rotation;

            if (!leftPhysicsHand.isColliding){
                leftPhysicsHand.SetRotationOffset(inputHandler.leftControllerAnchor.rotation);
            }       
        }


        if ((rightPhysicsHand.isColliding || rightPhysicsHand.isClimbing) && inputHandler.GetHeldState(InputHandler.InputButton.R_Grip)){
            
            rightPhysicsHand.isClimbing = true;
            rightPhysicsHand.rb.isKinematic = true;

            rightPhysicsHand.transform.position = rightPhysicsHand.physicsHandPositionAnchor;
            rightPhysicsHand.transform.rotation = rightPhysicsHand.physicsHandRotationAnchor;

            inputHandler.playerRigidbody.velocity = Vector3.zero;

            rightBodyTarget = rightPhysicsHand.controllerAnchor + rightPhysicsHand.controllerToBodyOffset;      
        }
        else{

            rightPhysicsHand.isClimbing = false;
            rightPhysicsHand.rb.isKinematic = false;

            rightPhysicsHand.controllerAnchor = inputHandler.rightControllerAnchor.transform.position;
            rightPhysicsHand.physicsHandPositionAnchor = rightPhysicsHand.transform.position;
            rightPhysicsHand.physicsHandRotationAnchor = rightPhysicsHand.transform.rotation;

            if (!rightPhysicsHand.isColliding){
                rightPhysicsHand.SetRotationOffset(inputHandler.rightControllerAnchor.rotation);
            }    
        }



        // If both hands are holding climbable geometry, calculate the average displacement of each one to determine target body position
        if (leftPhysicsHand.isClimbing && rightPhysicsHand.isClimbing){
            mainBodyTarget = (leftBodyTarget + rightBodyTarget) / 2;
        }
        else if(leftPhysicsHand.isClimbing){
            mainBodyTarget = leftBodyTarget;
        }
        else if (rightPhysicsHand.isClimbing){
            mainBodyTarget = rightBodyTarget;
        }
        else{
            mainBodyTarget = this.transform.position;
        }

        this.transform.position = mainBodyTarget;

    }
}
