using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    private InputHandler inputHandler;

    [SerializeField] private float handRadius = 0.08f;
    [SerializeField] private int armForce = 900; // (in newtons)
    [SerializeField] private int handForce = 10; // (in newtons)
    [SerializeField] private int bodyWeight = 625; // (in newtons)


    private PhysicsHand leftPhysicsHand;
    private PhysicsHand rightPhysicsHand;
    private ControllerCollisionTrigger leftControllerCollider;
    private ControllerCollisionTrigger rightControllerCollider;

    private Vector3 leftGripAnchor; //
    private Vector3 rightGripAnchor; //

    private Vector3 leftControllerOffset; //
    private Vector3 rightControllerOffset; //

    private Vector3 leftBodyTarget;
    private Vector3 rightBodyTarget;
    private Vector3 mainBodyTarget;


    void Start()
    {
        inputHandler = GetComponent<InputHandler>();

        inputHandler.leftControllerAnchor.GetComponent<SphereCollider>().radius = handRadius;
        inputHandler.rightControllerAnchor.GetComponent<SphereCollider>().radius = handRadius;

        leftPhysicsHand = inputHandler.leftPhysicsHand.GetComponent<PhysicsHand>();
        rightPhysicsHand = inputHandler.rightPhysicsHand.GetComponent<PhysicsHand>();

        inputHandler.leftPhysicsHand.localScale = new Vector3(handRadius * 2, handRadius * 2, handRadius * 2);
        inputHandler.rightPhysicsHand.localScale = new Vector3(handRadius * 2, handRadius * 2, handRadius * 2);
    }

    void Update()
    {
        // Attempt to match physics hand position with player's real life hands.
        if (leftPhysicsHand.isColliding){
            inputHandler.leftPhysicsHand.GetComponent<Rigidbody>().AddForce(leftPhysicsHand.handToControllerOffset.normalized * handForce, ForceMode.Force);
        }
        else{
            inputHandler.leftPhysicsHand.GetComponent<Rigidbody>().velocity = Vector3.zero;
            
            inputHandler.leftPhysicsHand.position = inputHandler.leftControllerAnchor.position;
            inputHandler.leftPhysicsHand.rotation = inputHandler.leftControllerAnchor.rotation;
        }

        if (rightPhysicsHand.isColliding){
            inputHandler.rightPhysicsHand.GetComponent<Rigidbody>().AddForce(rightPhysicsHand.handToControllerOffset.normalized * handForce, ForceMode.Force);
        }
        else{
            inputHandler.rightPhysicsHand.GetComponent<Rigidbody>().velocity = Vector3.zero;

            inputHandler.rightPhysicsHand.position = inputHandler.rightControllerAnchor.position;
            inputHandler.rightPhysicsHand.rotation = inputHandler.rightControllerAnchor.rotation;
        }


        // Continue climbing if grip is held and the hand is touching climbable geometry / was previously climbing. Otherwise, update anchor positions and unfreeze the physical hand
        if ((leftPhysicsHand.isColliding || leftPhysicsHand.isClimbing) && inputHandler.GetHeldState(InputHandler.InputButton.L_Grip)){
            leftBodyTarget = leftPhysicsHand.gripAnchor + leftPhysicsHand.controllerToBodyOffset;

            leftPhysicsHand.isClimbing = true;

            //inputHandler.leftPhysicsHand.GetComponent<Rigidbody>().isKinematic = true;
            inputHandler.playerRigidbody.velocity = Vector3.zero;
        }
        else{
            leftPhysicsHand.gripAnchor = inputHandler.leftControllerAnchor.transform.position;
            leftPhysicsHand.isClimbing = false;
        }

        if ((rightPhysicsHand.isColliding || rightPhysicsHand.isClimbing) && inputHandler.GetHeldState(InputHandler.InputButton.R_Grip)){
            rightBodyTarget = rightPhysicsHand.gripAnchor + rightPhysicsHand.controllerToBodyOffset;

            rightPhysicsHand.isClimbing = true;

            //inputHandler.rightPhysicsHand.GetComponent<Rigidbody>().isKinematic = true;
            inputHandler.playerRigidbody.velocity = Vector3.zero;
        }
        else{
            rightPhysicsHand.gripAnchor = inputHandler.rightControllerAnchor.transform.position;
            rightPhysicsHand.isClimbing = false;
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
