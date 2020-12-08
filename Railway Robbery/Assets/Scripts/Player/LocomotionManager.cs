using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    private BodyPartReferences bodyParts;
    
    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float maxRotationSpeed;

    [SerializeField] private bool useHeadAsForward;

    [SerializeField] private float stickDeadzone;

    
    void Awake() {
        bodyParts = GetComponent<BodyPartReferences>();
    }


    private void FixedUpdate() {

        // If the player is not climbing, receive left stick input and calculate resulting velocity
        if(bodyParts.leftClimbingHand.isClimbing == false && bodyParts.rightClimbingHand.isClimbing == false){

            // Get left stick input and determine whether 'forward' should be based on controller orientation or head orientation
            Vector3 inputForward = useHeadAsForward ? bodyParts.cameraTransform.forward : bodyParts.leftControllerTransform.forward;

            Vector3 targetForward = Vector3.ProjectOnPlane(inputForward, Vector3.up).normalized;
            Vector3 targetRight = Vector3.Cross(Vector3.up, targetForward);

            Vector2 movementInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
            
            // Combine joystick input and forward direction to determine the direction of movement
            Vector3 movementDirection = (targetForward * movementInput.y) + (targetRight * movementInput.x);

            Vector3 inputVelocity = movementDirection * maxMovementSpeed;

            // Set x and z components of movement while preserving y
            bodyParts.playerRigidbody.velocity = new Vector3(inputVelocity.x, bodyParts.playerRigidbody.velocity.y, inputVelocity.z);
        }
    }

    private void Update() {
        float rotationInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
        float rotationDegrees = rotationInput * maxRotationSpeed * Time.deltaTime;

        if(Mathf.Abs(rotationInput) > stickDeadzone){
            // Rotate the player container object around the camera position
            this.transform.RotateAround(bodyParts.cameraTransform.position, Vector3.up, rotationDegrees);

            // Rotate player's velocity as well
            Quaternion velocityRotation = Quaternion.Euler(0, rotationDegrees, 0);
            bodyParts.playerRigidbody.velocity = velocityRotation * bodyParts.playerRigidbody.velocity;
        }

        // Safeguard for testing, remove later
        if (transform.position.y < -20){
            transform.position = new Vector3(transform.position.x, 5, transform.position.z);
            bodyParts.playerRigidbody.velocity = Vector3.zero;
        }
    }
}
