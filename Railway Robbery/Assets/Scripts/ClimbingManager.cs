using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    private InputHandler inputHandler;

    //[SerializeField] private float climbingGripThreshold = 0.5f;

    [SerializeField] private float handRadius = 0.08f;
    [SerializeField] private int armForce = 900; // (in newtons)
    [SerializeField] private int handForce = 20; // (in newtons)
    [SerializeField] private int bodyWeight = 625; // (in newtons)


    [HideInInspector] public bool leftHandColliding;
    [HideInInspector] public bool rightHandColliding;

    private bool leftHandClimbing;
    private bool rightHandClimbing;

    private Vector3 leftGripAnchor;
    private Vector3 rightGripAnchor;

    private Vector3 leftControllerOffset;
    private Vector3 rightControllerOffset;

    private Vector3 leftBodyTarget;
    private Vector3 rightBodyTarget;
    private Vector3 mainBodyTarget;


    void Start()
    {
        inputHandler = GetComponent<InputHandler>();

        inputHandler.leftControllerAnchor.GetComponent<SphereCollider>().radius = handRadius;
        inputHandler.rightControllerAnchor.GetComponent<SphereCollider>().radius = handRadius;

        inputHandler.leftPhysicsHand.localScale = new Vector3(handRadius * 2, handRadius * 2, handRadius * 2);
        inputHandler.rightPhysicsHand.localScale = new Vector3(handRadius * 2, handRadius * 2, handRadius * 2);
    }

    void FixedUpdate()
    {
        leftHandClimbing = false;
        rightHandClimbing = false;

        leftControllerOffset = this.transform.position - inputHandler.leftControllerAnchor.position; //inputHandler.cameraTransform.position - inputHandler.leftController.position;
        rightControllerOffset = this.transform.position - inputHandler.rightControllerAnchor.position; //inputHandler.cameraTransform.position - inputHandler.rightController.position;


        // Attempt to match physics hand position with player's real life hands.
        if (leftHandColliding == true){
            Vector3 leftPhysicsHandOffset = inputHandler.leftControllerAnchor.position - inputHandler.leftPhysicsHand.position;
            inputHandler.leftPhysicsHand.GetComponent<Rigidbody>().AddForce(leftPhysicsHandOffset.normalized * handForce, ForceMode.Force);
        }
        else{
            inputHandler.leftPhysicsHand.position = inputHandler.leftControllerAnchor.position;
            inputHandler.leftPhysicsHand.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        inputHandler.leftPhysicsHand.rotation = inputHandler.leftControllerAnchor.rotation;

        if (rightHandColliding == true){
            Vector3 rightPhysicsHandOffset = inputHandler.rightControllerAnchor.position - inputHandler.rightPhysicsHand.position;
            inputHandler.rightPhysicsHand.GetComponent<Rigidbody>().AddForce(rightPhysicsHandOffset.normalized * handForce, ForceMode.Force);
        }
        else{
            inputHandler.rightPhysicsHand.position = inputHandler.rightControllerAnchor.position;
            inputHandler.rightPhysicsHand.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        inputHandler.rightPhysicsHand.rotation = inputHandler.rightControllerAnchor.rotation;


        // Initiate climbing only if grip is held and the hand is touching climbable geometry. Otherwise, update anchor positions
        if (leftHandColliding == true && inputHandler.GetHeldState(InputHandler.InputButton.L_Grip)){
            inputHandler.playerRigidbody.velocity = Vector3.zero;

            leftBodyTarget = leftGripAnchor + leftControllerOffset;
            leftHandClimbing = true;
        }
        else{
            leftGripAnchor = inputHandler.leftControllerAnchor.transform.position;
        }

        if (rightHandColliding == true && inputHandler.GetHeldState(InputHandler.InputButton.R_Grip)){
            inputHandler.playerRigidbody.velocity = Vector3.zero;

            rightBodyTarget = rightGripAnchor + rightControllerOffset;
            rightHandClimbing = true;
        }
        else{
            rightGripAnchor = inputHandler.rightControllerAnchor.transform.position;
        }


        // If both hands are holding climbable geometry, calculate the average displacement of each one to determine target body position
        if (leftHandClimbing && rightHandClimbing){
            mainBodyTarget = (leftBodyTarget + rightBodyTarget) / 2;
        }
        else if(leftHandClimbing){
            mainBodyTarget = leftBodyTarget;
        }
        else if (rightHandClimbing){
            mainBodyTarget = rightBodyTarget;   
        }
        else{
            mainBodyTarget = this.transform.position;
        }

        this.transform.position = mainBodyTarget;
    }
}
