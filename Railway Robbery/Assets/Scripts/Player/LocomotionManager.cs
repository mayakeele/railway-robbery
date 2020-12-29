using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    private BodyPartReferences bodyParts;
    
    [SerializeField] private float maxMovementSpeed;
    [SerializeField] private float maxRotationSpeed;

    [SerializeField] private float jumpSpeed;

    [SerializeField] private bool useHeadAsForward;

    [SerializeField] private float translationStickDeadzone;
    [SerializeField] private float rotationStickDeadzone;

    public Vector3 currentPosition;
    public Vector3 previousPosition;

    
    void Awake() {
        bodyParts = GetComponent<BodyPartReferences>();
    }


    void Start() {
        currentPosition = transform.position;
        previousPosition = transform.position;
    }

    private void Update() {
        // Calculate translational velocity of the player based on input

        currentPosition = transform.position;

        bool handsClimbing = bodyParts.leftClimbingHand.isClimbing || bodyParts.rightClimbingHand.isClimbing;
        bool climbingStatic = (bodyParts.leftClimbingHand.isClimbing && !bodyParts.leftClimbingHand.climbedRigidbody) || 
                              (bodyParts.rightClimbingHand.isClimbing && !bodyParts.rightClimbingHand.climbedRigidbody);
        bool isGrounded = bodyParts.groundedStateTracker.isGrounded;

        if(!handsClimbing || (!climbingStatic && isGrounded)){

            // Get left stick input, apply velocity if outside of deadzone
            Vector2 movementInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);

            //if(movementInput.magnitude > translationStickDeadzone && bodyParts.groundedStateTracker.isGrounded){
            if(movementInput.magnitude > translationStickDeadzone){
                // Determine whether 'forward' should be based on controller orientation or head orientation
                Vector3 inputForward = useHeadAsForward ? bodyParts.cameraTransform.forward : bodyParts.leftControllerTransform.forward;

                Vector3 targetForward = Vector3.ProjectOnPlane(inputForward, Vector3.up).normalized;
                Vector3 targetRight = Vector3.Cross(Vector3.up, targetForward);
                
                // Combine joystick input and forward direction to determine the direction of movement
                Vector3 movementDirection = (targetForward * movementInput.y) + (targetRight * movementInput.x);

                Vector3 inputVelocity = movementDirection * maxMovementSpeed;

                // Set x and z components of movement while preserving y
                bodyParts.playerRigidbody.velocity = new Vector3(inputVelocity.x, bodyParts.playerRigidbody.velocity.y, inputVelocity.z);

                // Translate hands along with body
                //Vector3 translationAmount = inputVelocity * Time.deltaTime;
                //bodyParts.leftHand.transform.Translate(translationAmount, Space.World);
                //bodyParts.rightHand.transform.Translate(translationAmount, Space.World);
            }

            // Move hands the same distance as the body
            Vector3 positionChange = currentPosition - previousPosition;
            //bodyParts.leftHand.transform.Translate(positionChange, Space.World);
            //bodyParts.rightHand.transform.Translate(positionChange, Space.World);

            previousPosition = transform.position;


            // Handle jumping if player is grounded and pressing jump button
            if(bodyParts.groundedStateTracker.isGrounded && bodyParts.inputHandler.GetHeldState(InputHandler.InputButton.R_StickPress)){
                bodyParts.playerRigidbody.velocity = new Vector3(bodyParts.playerRigidbody.velocity.x, jumpSpeed, bodyParts.playerRigidbody.velocity.z);
            }
        }
    }

    private void LateUpdate() {
        float rotationInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x;
        float rotationDegrees = rotationInput * maxRotationSpeed * Time.deltaTime;

        if(Mathf.Abs(rotationInput) > rotationStickDeadzone){
            // Rotate the player container object around the camera position
            this.transform.RotateAround(bodyParts.cameraTransform.position, Vector3.up, rotationDegrees);
            
            // Rotate hands with body
            //bodyParts.leftHand.transform.RotateAround(bodyParts.cameraTransform.position, Vector3.up, rotationDegrees);
            //bodyParts.rightHand.transform.RotateAround(bodyParts.cameraTransform.position, Vector3.up, rotationDegrees);
            // Rotate player's velocity as well
            //Quaternion velocityRotation = Quaternion.Euler(0, rotationDegrees, 0);
            //bodyParts.playerRigidbody.velocity = velocityRotation * bodyParts.playerRigidbody.velocity;
        }

        // Safeguard for testing, remove later
        if (transform.position.y < -20){
            transform.position = new Vector3(transform.position.x, 2, transform.position.z);
            bodyParts.playerRigidbody.velocity = Vector3.zero;
        }
    }
}
