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


    void FixedUpdate()
    {   
        // Get left stick input and determine whether 'forward' should be based on controller orientation or head orientation
        targetOrientation = useHeadAsForward ? inputHandler.cameraTransform.rotation : inputHandler.leftController.rotation;
        Vector2 movementInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        
        // Combine joystick input and forward direction to determine the direction of acceleration
        Vector3 movementDirection = targetOrientation * new Vector3(movementInput.x, 0, movementInput.y).normalized;

        // Apply acceleration if the stick is pushed outside the deadzone, clamping velocity with regard to max speed AND the magnitude of the input
        /*if (movementInput.magnitude >= stickDeadzone) {    
            linearVelocity += movementDirection * linearAcceleration * Time.deltaTime;
            linearVelocity = Vector3.ClampMagnitude(linearVelocity, maxMovementSpeed * movementInput.magnitude);
        }
        // If stick is not pushed past deadzone, decelerate the player to zero
        else{
            movementInput = Vector2.zero;
            if (linearVelocity.magnitude > 0){
                linearVelocity -= linearVelocity.normalized * linearAcceleration * Time.deltaTime;
            }
            else{
                linearVelocity = Vector2.zero;
            }
        }*/

        linearVelocity += movementDirection * linearAcceleration * Time.deltaTime;
        linearVelocity = Vector3.ClampMagnitude(linearVelocity, maxMovementSpeed * movementInput.magnitude);

        // Set x and z components of movement while preserving y
        inputHandler.rb.velocity = new Vector3(linearVelocity.x, inputHandler.rb.velocity.y, linearVelocity.z);


        float rotationInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * maxRotationSpeed * Time.deltaTime;
        this.transform.RotateAround(inputHandler.cameraTransform.position, Vector3.up, rotationInput);
    }
}
