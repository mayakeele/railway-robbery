using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingHand : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;

    public bool isLeftHand;
    [HideInInspector] public InputHandler.InputButton grabButton;

    [SerializeField] private float maxHandDisplacement = 0.3f;
    
    [SerializeField] private Vector3 rotationOffsetEuler;
    [HideInInspector] public Quaternion rotationOffset;
    [SerializeField] private Vector3 positionOffset;

    [HideInInspector] public string collidingTag;
    [HideInInspector] public bool isClimbing;

    [HideInInspector] public float angleToController;
    [HideInInspector] public Vector3 controllerToBodyOffset;
    [HideInInspector] public Vector3 handToControllerOffset;

    private Rigidbody handRigidbody;
    public Transform controllerAnchor;
    [HideInInspector]public Vector3 climbingAnchorPosition;
    [HideInInspector]public Quaternion climbingAnchorRotation;


    void Awake()
    {
        grabButton = isLeftHand ? InputHandler.InputButton.L_Grip : InputHandler.InputButton.R_Grip;

        handRigidbody = GetComponent<Rigidbody>();
    }

    void Start() {
        Unfreeze();
        UpdateClimbingAnchor();
    }


    public void UpdateClimbingAnchor(){
        // Sets the transform of the climbing anchor to match the controller at this point in time
        climbingAnchorPosition = controllerAnchor.position;
        climbingAnchorRotation = controllerAnchor.rotation;
    }

    public void Freeze(){
        // Disables movement and grabbing with this hand, locking it in place

        handRigidbody.isKinematic = true;
        handRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        transform.SetPositionAndRotation(climbingAnchorPosition, climbingAnchorRotation);
    }

    public void Unfreeze(){
        // Re-enables movement and grabbing with this hand, locking it in place

        handRigidbody.isKinematic = false;
        handRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
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
        collidingTag = other.gameObject.tag;
    }

    private void OnCollisionExit(Collision other) {
        collidingTag = null;
    }
}
