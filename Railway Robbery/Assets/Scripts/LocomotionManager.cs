using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    
    [SerializeField] private bool useHeadAsForward;
    private Quaternion targetOrientation;

    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float linearAcceleration;

    [SerializeField] private float maxRotationSpeed;
    [SerializeField] private float angularAcceleration;

    [SerializeField] private float stickDeadzone;

    private Vector3 linearVelocity = Vector3.zero;
    private float angularVelocity = 0;

    private InputHandler inputHandler;
    
    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
    }


    private void LateUpdate() {

        float rotationInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
        if(Mathf.Abs(rotationInput) > stickDeadzone){
            this.transform.RotateAround(inputHandler.cameraTransform.position, Vector3.up, rotationInput * maxRotationSpeed * Time.deltaTime);
        }


        // Get left stick input and determine whether 'forward' should be based on controller orientation or head orientation
        targetOrientation = useHeadAsForward ? inputHandler.cameraTransform.rotation : inputHandler.leftControllerAnchor.rotation;
        Vector2 movementInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        
        // Combine joystick input and forward direction to determine the direction of acceleration
        Vector3 movementDirection = targetOrientation * new Vector3(movementInput.x, 0, movementInput.y).normalized;

        linearVelocity += movementDirection * linearAcceleration * Time.deltaTime;
        linearVelocity = Vector3.ClampMagnitude(linearVelocity, maxMovementSpeed * movementInput.magnitude);

        // Set x and z components of movement while preserving y
        inputHandler.playerRigidbody.velocity = new Vector3(linearVelocity.x, inputHandler.playerRigidbody.velocity.y, linearVelocity.z);
    }
}
