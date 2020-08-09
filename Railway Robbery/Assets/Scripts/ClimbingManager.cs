using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{
    private InputHandler inputHandler;

    //[SerializeField] private float climbingGripThreshold = 0.5f;

    [SerializeField] private int armForce = 900; // (in newtons)
    [SerializeField] private int bodyWeight = 625; // (in newtons)



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

        // Set anchor point on the first frame grip is held
        if (inputHandler.GetHeldState(InputHandler.InputButton.L_Grip) == false){
            leftGripAnchor = inputHandler.leftController.transform.position;
            bodyAnchor = this.transform.position;
        }
        if (inputHandler.GetHeldState(InputHandler.InputButton.R_Grip) == false){ 
            rightGripAnchor = inputHandler.rightController.transform.position;
            bodyAnchor = this.transform.position;
        }
        

        // Calculate hand position relative to the anchor point and move the player body accordingly
        if (inputHandler.GetHeldState(InputHandler.InputButton.L_Grip) == true){
            targetBodyPosition = leftGripAnchor - inputHandler.leftController.transform.position;
            this.transform.position = bodyAnchor + targetBodyPosition;
        }
        else if (inputHandler.GetHeldState(InputHandler.InputButton.R_Grip) == true){
            targetBodyPosition = rightGripAnchor - inputHandler.rightController.transform.position;
            this.transform.position = bodyAnchor + targetBodyPosition;
        }
    }
}
