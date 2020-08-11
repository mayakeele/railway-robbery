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
    private Vector3 bodyAnchor;

    private Vector3 leftTarget;
    private Vector3 rightTarget;
    private Vector3 bodyTarget;


    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    void FixedUpdate()
    {
        leftHandClimbing = false;
        rightHandClimbing = false;

        // Move body only if grip is held and the hand is touching climbable geometry. Otherwise, update anchor positions

        // Left hand
        if (leftHandColliding == true && inputHandler.GetHeldState(InputHandler.InputButton.L_Grip) == true){
            inputHandler.rb.velocity = Vector3.zero;

            leftTarget = leftGripAnchor - inputHandler.leftController.transform.position;
            leftHandClimbing = true;
        }
        else{
            leftGripAnchor = inputHandler.leftController.transform.position;
            bodyAnchor = this.transform.position;
        }

        // Right hand
        if (rightHandColliding == true && inputHandler.GetHeldState(InputHandler.InputButton.R_Grip) == true){
            inputHandler.rb.velocity = Vector3.zero;

            rightTarget = rightGripAnchor - inputHandler.rightController.transform.position;
            rightHandClimbing = true;
        }
        else{
            rightGripAnchor = inputHandler.rightController.transform.position;
            bodyAnchor = this.transform.position;
        }


        // If both hands are holding climbable geometry, calculate the average displacement of each one to determine target body position
        if (leftHandClimbing && rightHandClimbing){
            bodyTarget = (leftTarget + rightTarget) / 2;
        }
        else if(leftHandClimbing){
            bodyTarget = leftTarget;
        }
        else if (rightHandClimbing){
            bodyTarget = rightTarget;
        }
        else{
            bodyTarget = Vector3.zero;
        }

        this.transform.position = bodyAnchor + bodyTarget;
    }
}
