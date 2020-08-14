using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHand : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private ClimbingManager climbingManager;
    public bool isLeftController;

    [HideInInspector] public bool isColliding;
    [HideInInspector] public bool isClimbing;

    [HideInInspector] public Vector3 controllerToBodyOffset;
    [HideInInspector] public Vector3 handToControllerOffset;
    [HideInInspector] public Vector3 gripAnchor;

    void Start()
    {
        //inputHandler = GetComponent<InputHandler>();
    }

    void Update()
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
