using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{
    private InputHandler inputHandler;
    private ClimbingManager climbingManager;

    public float playerHeight;
    private float currHeadsetHeight;
    [SerializeField] private float legLiftPercentage;

    public Transform headTransform;
    public Transform bodyTransform;
    public Transform feetTransform;
    private CapsuleCollider bodyCollider;

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        climbingManager = GetComponent<ClimbingManager>();
        bodyCollider = bodyTransform.GetComponent<CapsuleCollider>();
    }


    void FixedUpdate()
    {
        headTransform.position = inputHandler.cameraTransform.position;
        feetTransform.position = transform.position;

        currHeadsetHeight = inputHandler.cameraTransform.localPosition.y;

        if (climbingManager.leftPhysicsHand.isClimbing || climbingManager.rightPhysicsHand.isClimbing){
            // Scale body capsule collider to match the current height of the player's headset and the height of the player's feet

            bodyCollider.height = currHeadsetHeight * (1 - legLiftPercentage);

            bodyTransform.position = new Vector3(
                inputHandler.cameraTransform.position.x, 
                inputHandler.cameraTransform.position.y - (bodyCollider.height / 2),
                inputHandler.cameraTransform.position.z
            );
            
        }
        else{
            // Scale body capsule collider to match the current height of the player's headset

            bodyCollider.height = currHeadsetHeight;

            bodyTransform.position = new Vector3(
                inputHandler.cameraTransform.position.x, 
                inputHandler.cameraTransform.position.y - (bodyCollider.height / 2),
                inputHandler.cameraTransform.position.z
            );
        }  
    }
}
