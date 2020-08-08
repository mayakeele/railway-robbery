using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingManager : MonoBehaviour
{

    [SerializeField] private Transform leftController;
    [SerializeField] private Transform rightController;

    [SerializeField] private float climbingGripThreshold = 0.5f;

    private bool leftGripHeld = false;
    private bool rightGripHeld = false;
    private Vector3 leftGripAnchor;
    private Vector3 rightGripAnchor;
    private Vector3 bodyAnchor;
    private Vector3 targetBodyPosition;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        OVRInput.FixedUpdate();

        // Set anchor point on the first frame grip is held
        if (!leftGripHeld){ 
            leftGripAnchor = leftController.transform.position;
            bodyAnchor = this.transform.position;
        }
        if (!rightGripHeld){ 
            rightGripAnchor = rightController.transform.position;
            bodyAnchor = this.transform.position;
        }
        
        // Read grip input for each hand
        leftGripHeld = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) >= climbingGripThreshold ? true : false;
        rightGripHeld = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) >= climbingGripThreshold ? true : false;
        //Debug.Log(leftGripHeld);

        // Calculate hand position relative to the anchor point and move the player body accordingly
        if (leftGripHeld){
            targetBodyPosition = leftGripAnchor - leftController.transform.position;
            this.transform.position = bodyAnchor + targetBodyPosition;

            Gizmos.DrawSphere(leftGripAnchor, 0.5f);
        }
        else if (rightGripHeld){
            targetBodyPosition = rightGripAnchor - rightController.transform.position;
            this.transform.position = bodyAnchor + targetBodyPosition;

            Gizmos.DrawSphere(leftGripAnchor, 0.5f);
        }
    }
}
