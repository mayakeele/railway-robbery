using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{
    public float playerHeight;
    private float currHeadsetHeight;

    public Transform bodyGameobject;
    private CapsuleCollider bodyCollider;

    private InputHandler inputHandler;

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        bodyCollider = bodyGameobject.GetComponent<CapsuleCollider>();
    }


    void FixedUpdate()
    {
        // Scale body capsule collider to match the current height of the player's headset
        currHeadsetHeight = inputHandler.cameraTransform.localPosition.y;

        bodyGameobject.transform.position = new Vector3(
            inputHandler.cameraTransform.position.x, 
            inputHandler.cameraTransform.position.y - (currHeadsetHeight / 2), 
            inputHandler.cameraTransform.position.z
        );

        bodyCollider.height = currHeadsetHeight;
    }
}
