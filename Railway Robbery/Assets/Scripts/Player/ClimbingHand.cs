using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ClimbingHand : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Hand autoHand;

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

    public Transform controllerTransform;
    public Transform handTransform;
    private Rigidbody handRigidbody;
    
    [HideInInspector] public Vector3 controllerAnchorPosition;
    [HideInInspector] public Vector3 handAnchorPosition;
    [HideInInspector] public Quaternion handAnchorRotation;


    void Awake()
    {
        grabButton = isLeftHand ? InputHandler.InputButton.L_Grip : InputHandler.InputButton.R_Grip;

        handRigidbody = GetComponent<Rigidbody>();
        autoHand = GetComponent<Hand>();
    }

    void Start() {
        Unfreeze();
        UpdateControllerAnchor();
    }


    public void UpdateControllerAnchor(){
        // Sets the transform of the climbing anchor to match the controller at this point in time
        controllerAnchorPosition = controllerTransform.position;
    }

    public void UpdateHandAnchor(){
        // Sets the transform of the climbing anchor to match the controller at this point in time
        handAnchorPosition = handTransform.position;
        handAnchorRotation = handTransform.rotation;
    }

    public void Freeze(){
        // Disables movement and grabbing with this hand, locking it in place

        handRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        handRigidbody.isKinematic = true;

        autoHand.freezePos = true;
        autoHand.freezeRot = true;

        transform.SetPositionAndRotation(handAnchorPosition, handAnchorRotation);
    }

    public void Unfreeze(){
        // Re-enables movement and grabbing with this hand, locking it in place

        autoHand.freezePos = false;
        autoHand.freezeRot = false;

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
