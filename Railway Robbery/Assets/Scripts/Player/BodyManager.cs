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

    public Transform bodyGameobject;
    private CapsuleCollider bodyCollider;

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        climbingManager = GetComponent<ClimbingManager>();
        bodyCollider = bodyGameobject.GetComponent<CapsuleCollider>();
    }


    void FixedUpdate()
    {
        
        currHeadsetHeight = inputHandler.cameraTransform.localPosition.y;

        if (climbingManager.leftPhysicsHand.isClimbing || climbingManager.rightPhysicsHand.isClimbing){
            // Scale body capsule collider to match the current height of the player's headset and the height of the player's feet

            bodyCollider.height = currHeadsetHeight * (1 - legLiftPercentage);

            bodyGameobject.transform.position = new Vector3(
                inputHandler.cameraTransform.position.x, 
                inputHandler.cameraTransform.position.y - (bodyCollider.height / 2),
                inputHandler.cameraTransform.position.z
            );
            
        }
        else{
            // Scale body capsule collider to match the current height of the player's headset

            bodyCollider.height = currHeadsetHeight;

            bodyGameobject.transform.position = new Vector3(
                inputHandler.cameraTransform.position.x, 
                inputHandler.cameraTransform.position.y - (bodyCollider.height / 2),
                inputHandler.cameraTransform.position.z
            );
        }  
    }
}
