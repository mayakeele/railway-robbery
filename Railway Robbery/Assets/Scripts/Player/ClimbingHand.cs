using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ClimbingHand : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public Hand autoHand;
    public Transform controllerTransform;
    private Rigidbody handRigidbody;


    [Header("Hand Settings")]
    public bool isLeftHand;
    [SerializeField] private string handLayerName;
    [SerializeField] private string handClimbingLayerName;

    [SerializeField] private float maxHandDistance;
    [HideInInspector] public InputHandler.InputButton grabButton;



    // Climbing Variables
    [HideInInspector] public bool isClimbing;

    [HideInInspector] public Vector3 controllerAnchorPosition;
    [HideInInspector] public Vector3 handAnchorPosition;
    [HideInInspector] public Quaternion handAnchorRotation;
    
    [HideInInspector] public GameObject climbedObject;
    [HideInInspector] public Rigidbody climbedRigidbody;
    [HideInInspector] public Transform climbingAnchor;



    void Awake()
    {
        grabButton = isLeftHand ? InputHandler.InputButton.L_Grip : InputHandler.InputButton.R_Grip;

        handRigidbody = GetComponent<Rigidbody>();
        autoHand = GetComponent<Hand>();

        climbingAnchor = new GameObject("Climbing Anchor").transform;
    }


    void Start() {
        OnClimbingStop();
        UpdateControllerAnchor();
    }


    void Update() {
        float handDistance = (controllerTransform.position - transform.position).magnitude;

        if(isClimbing && handDistance > maxHandDistance){
            OnClimbingStop();
        }
    }



    public void UpdateControllerAnchor(){
        // Sets the transform of the climbing anchor to match the controller at this point in time
        controllerAnchorPosition = controllerTransform.position;
    }

    public void UpdateHandAnchor(){
        // Sets the transform of the climbing anchor to match the controller at this point in time
        handAnchorPosition = transform.position;
        handAnchorRotation = transform.rotation;
    }


    public bool IsHoldingObject(){
        // Returns whether the attached hand is holding on to a grabbable
        if(autoHand.GetHeldGrabbable() != null){
            return true;
        }
        else{
            return false;
        }
    }

    
    public void OnClimbingStart(Transform climbingParent){
        // Disables movement and grabbing with this hand, locking it in place
        climbedObject = climbingParent.gameObject;

        isClimbing = true;

        handRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        handRigidbody.isKinematic = true;

        climbingAnchor.SetPositionAndRotation(handAnchorPosition, handAnchorRotation);
        climbingAnchor.SetParent(climbingParent);

        transform.SetPositionAndRotation(handAnchorPosition, handAnchorRotation);

        
        DynamicClimbable dynamicClimbable = climbedObject.GetComponent<DynamicClimbable>();
        if(dynamicClimbable){
            //dynamicClimbable.SetAttachedMass(autoHand.playerBodyParts.playerRigidbody.mass);
            //dynamicClimbable.SetClimbingState(true);
        }


        Hand.SetLayerRecursive(autoHand.transform, LayerMask.NameToLayer(handClimbingLayerName));
    }

    public void OnClimbingStop(){
        // Re-enables movement and grabbing with this hand
        isClimbing = false;

        climbingAnchor.SetPositionAndRotation(transform.position, transform.rotation);
        climbingAnchor.SetParent(this.transform);

        handRigidbody.isKinematic = false;
        handRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        if(climbedObject){
            DynamicClimbable dynamicClimbable = climbedObject.GetComponent<DynamicClimbable>();
            if(dynamicClimbable){
                //dynamicClimbable.SetAttachedMass(0);
                //dynamicClimbable.SetClimbingState(false);
            }
            climbedObject = null;
        }
        
        Hand.SetLayerRecursive(autoHand.transform, LayerMask.NameToLayer(handLayerName));
    }


    public void FollowClimbingAnchor(){
        // Keep this hand locked onto the climbing anchor, even if the anchor is moving
        transform.SetPositionAndRotation(climbingAnchor.position, climbingAnchor.rotation);
    }
}
