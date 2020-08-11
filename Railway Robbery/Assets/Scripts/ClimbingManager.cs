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

    private Vector3 leftGripAnchor;
    private Vector3 rightGripAnchor;
    private Vector3 bodyAnchor;
    private Vector3 targetBodyPosition;


    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    void FixedUpdate()
    {
        // Move body only if grip is held and the hand is touching climbable geometry. Otherwise, update anchor positions

        // Left hand
        if (leftHandColliding == true && inputHandler.GetHeldState(InputHandler.InputButton.L_Grip) == true){
            inputHandler.rb.velocity = Vector3.zero;

            targetBodyPosition = leftGripAnchor - inputHandler.leftController.transform.position;
            this.transform.position = bodyAnchor + targetBodyPosition;
        }
        else{
            leftGripAnchor = inputHandler.leftController.transform.position;
            bodyAnchor = this.transform.position;
        }

        // Right hand
        if (rightHandColliding == true && inputHandler.GetHeldState(InputHandler.InputButton.R_Grip) == true){
            inputHandler.rb.velocity = Vector3.zero;

            targetBodyPosition = rightGripAnchor - inputHandler.rightController.transform.position;
            this.transform.position = bodyAnchor + targetBodyPosition;
        }
        else{
            rightGripAnchor = inputHandler.rightController.transform.position;
            bodyAnchor = this.transform.position;
        }
    }
}
