using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHand : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private ClimbingManager climbingManager;
    [HideInInspector] public Rigidbody rb;

    public bool isLeftController;

    [SerializeField] private float springFrequency;
    [SerializeField] private float springDampingRatio;
    [SerializeField] private float angularSpringFrequency;
    [SerializeField] private float angularSpringDampingRatio;

    [HideInInspector] public float angleToController;

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


    void Update()
    {
        if (isLeftController){
            handToControllerOffset = inputHandler.leftController.position - transform.position;
            controllerToBodyOffset = climbingManager.transform.position - inputHandler.leftController.position;
            angleToController = Quaternion.Angle(inputHandler.leftController.rotation, transform.rotation);
        }
        else{
            handToControllerOffset = inputHandler.rightController.position - transform.position;
            controllerToBodyOffset = climbingManager.transform.position - inputHandler.rightController.position;
            angleToController = Quaternion.Angle(inputHandler.rightController.rotation, transform.rotation);
        }
    }

    private void FixedUpdate() {
        if (isLeftController){
            Vector3 springAcceleration = DampedSpring.GetDampedSpringAcceleration(
                this.transform.position, 
                inputHandler.leftController.transform.position + (transform.rotation * positionOffset), 
                rb.velocity - inputHandler.playerRigidbody.velocity, 
                springFrequency, 
                springDampingRatio);

            rb.AddForce(springAcceleration, ForceMode.Acceleration);

            Vector3 springAngularAcceleration = DampedSpring.GetDampedSpringAngularAcceleration(
                this.transform.rotation, 
                inputHandler.leftController.transform.rotation * rotationOffset, 
                rb.angularVelocity, 
                angularSpringFrequency,
                angularSpringDampingRatio);

            rb.AddTorque(springAngularAcceleration, ForceMode.Acceleration);
    
        }
        else{
            Vector3 springAcceleration = DampedSpring.GetDampedSpringAcceleration(
                this.transform.position, 
                inputHandler.rightController.transform.position + (transform.rotation * positionOffset), 
                rb.velocity - inputHandler.playerRigidbody.velocity, 
                springFrequency, 
                springDampingRatio);

            rb.AddForce(springAcceleration, ForceMode.Acceleration);

            Vector3 springAngularAcceleration = DampedSpring.GetDampedSpringAngularAcceleration(
                this.transform.rotation, 
                inputHandler.rightController.transform.rotation * rotationOffset, 
                rb.angularVelocity, 
                angularSpringFrequency,
                angularSpringDampingRatio);

            rb.AddTorque(springAngularAcceleration, ForceMode.Acceleration);

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
