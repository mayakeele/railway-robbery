using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHand : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private ClimbingManager climbingManager;
    [HideInInspector] public Rigidbody rb;
    public bool isLeftController;

    [SerializeField] private Vector3 rotationOffsetEuler;
    [HideInInspector] public Quaternion rotationOffset;

    [SerializeField] private Vector3 positionOffset;

    [HideInInspector] public bool isColliding;
    [HideInInspector] public bool isClimbing;

    [HideInInspector] public Vector3 controllerToBodyOffset;
    [HideInInspector] public Vector3 handToControllerOffset;

    [HideInInspector] public Vector3 controllerAnchor;
    [HideInInspector] public Vector3 physicsHandPositionAnchor;
    [HideInInspector] public Quaternion physicsHandRotationAnchor;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rotationOffset = Quaternion.Euler(rotationOffsetEuler);
    }


    void LateUpdate()
    {
        if (isLeftController){
            handToControllerOffset = inputHandler.leftControllerAnchor.position - transform.position;
            controllerToBodyOffset = climbingManager.transform.position - inputHandler.leftControllerAnchor.position;
        }
        else{
            handToControllerOffset = inputHandler.rightControllerAnchor.position - transform.position;
            controllerToBodyOffset = climbingManager.transform.position - inputHandler.rightControllerAnchor.position;
        }
 
    }

    public void SetPositionOffset(Vector3 newPos){
        // Sets the position of this hand to position, with respect to hand offset
        transform.position = newPos + positionOffset;
    }

    public void SetRotationOffset(Quaternion newRot){
        // Sets the position of this hand to position, with respect to hand offset
        transform.rotation = newRot * rotationOffset;
    }


    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Climbable"){
            isColliding = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        if (other.gameObject.tag == "Climbable"){
            isColliding = false;
        }
    }
}
