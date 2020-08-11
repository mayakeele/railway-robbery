using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    private InputHandler inputHandler;

    //[SerializeField] private float climbingGripThreshold = 0.5f;

    [SerializeField] private float handRadius = 0.08f;
    [SerializeField] private int armForce = 900; // (in newtons)
    [SerializeField] private int bodyWeight = 625; // (in newtons)


    [HideInInspector] public bool leftHandColliding;
    [HideInInspector] public bool rightHandColliding;

    private bool leftHandClimbing;
    private bool rightHandClimbing;

    private Vector3 leftGripAnchor;
    private Vector3 rightGripAnchor;

    private Vector3 leftHandOffset;
    private Vector3 rightHandOffset;

    private Vector3 leftBodyTarget;
    private Vector3 rightBodyTarget;
    private Vector3 mainBodyTarget;


    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    void FixedUpdate()
    {
        leftHandClimbing = false;
        rightHandClimbing = false;

        leftHandOffset = this.transform.position - inputHandler.leftController.position; //inputHandler.cameraTransform.position - inputHandler.leftController.position;
        rightHandOffset = this.transform.position - inputHandler.rightController.position; //inputHandler.cameraTransform.position - inputHandler.rightController.position;


        // Set anchor point if grip is held on each hand
        /*if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch)){
            leftGripAnchor = inputHandler.leftController.transform.position;
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch)){
            rightGripAnchor = inputHandler.rightController.transform.position;
        }*/


        // Move body only if grip is held and the hand is touching climbable geometry. Otherwise, update anchor positions

        // Left hand
        if (leftHandColliding == true && inputHandler.GetHeldState(InputHandler.InputButton.L_Grip)){
            inputHandler.rb.velocity = Vector3.zero;

            leftBodyTarget = leftGripAnchor + leftHandOffset;
            leftHandClimbing = true;
        }
        else{
            leftGripAnchor = inputHandler.leftController.transform.position;
        }

        // Right hand
        if (rightHandColliding == true && inputHandler.GetHeldState(InputHandler.InputButton.R_Grip)){
            inputHandler.rb.velocity = Vector3.zero;

            rightBodyTarget = rightGripAnchor + rightHandOffset;
            rightHandClimbing = true;
        }
        else{
            rightGripAnchor = inputHandler.rightController.transform.position;
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
