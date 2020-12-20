using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class GrapplingHookLauncher : MonoBehaviour
{
    public enum HookState{
        Unloaded,
        Loaded,
        Launched,
        Hooked,
        Failed
    }


    [Header("Grappling Gun Settings")]
    [SerializeField] private float reelInRate;
    [SerializeField] private float velocityDecayMultiplier;
    [SerializeField] private float minCableLength;
    [SerializeField] private float reelInTriggerThreshold;
    [SerializeField] private float oneHandedSpringFrequency;
    [SerializeField] private float twoHandedSpringFrequency;
    [SerializeField] private float springDamping;

    [Header("Haptic Settings")]
    [SerializeField] [Range(0, 1)] private float reelInHapticFrequency;
    [SerializeField] [Range(0, 1)] private float reelInHapticAmplitude;
    [Space]
    [SerializeField] [Range(0, 1)] private float swingingHapticFrequency;
    [SerializeField] [Range(0, 1)] private float swingingHapticMaxAmplitude;
    [SerializeField] [Range(0, 1)] private float swingingHapticBaseGradient;
    [SerializeField] private float maxSwingingHapticDistance;

    [Header("Prefabs")]
    [SerializeField] private GameObject hookPrefab;
    [SerializeField] private GameObject hookLoadedModel;

    [Header("References")]
    [SerializeField] private Transform barrelTipTransform;
    [SerializeField] private Transform cableOriginTransform;
    [SerializeField] private Transform hookSpawnTransform;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private LineRenderer lineRenderer;


    // Hook variables
    private HookState currentHookState;
    private float cableLength;
    private Hook attachedHook;
    private Vector3 hookAnchor;

    // Hand-to-body spring variables
    private float currentSpringFrequency;
    private Vector3 leftBodyTarget;
    private Vector3 rightBodyTarget;
    private Vector3 mainBodyTarget;
    private Vector3 swingingVelocity;

    // Detail variables
    private GameObject loadedHook;



    void Start()
    {
        currentHookState = HookState.Unloaded;
        swingingVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        List<Hand> handsHolding = grabbable.heldBy;

        if(currentHookState == HookState.Hooked){

            // Use cable distance and physics to constrain the launcher's position and velocity
            Vector3 initialhookToLauncher = transform.position - hookAnchor;
            Vector3 newHookToLauncher = initialhookToLauncher;

            if(initialhookToLauncher.magnitude > cableLength){
                transform.position = hookAnchor + (initialhookToLauncher.normalized * cableLength);

                newHookToLauncher = transform.position - hookAnchor;
                Vector3 initialVelocity = rb.velocity;
                Vector3 newVelocity = Vector3.ProjectOnPlane(initialVelocity, newHookToLauncher).normalized * initialVelocity.magnitude * velocityDecayMultiplier;
                rb.velocity = newVelocity;
            }
            

            if(handsHolding.Count > 0){

                BodyPartReferences bodyParts = handsHolding[0].playerBodyParts;
                bool isGrounded = bodyParts.groundedStateTracker.isGrounded;
                bool isClimbing = (bodyParts.leftClimbingHand.isClimbing || bodyParts.rightClimbingHand.isClimbing);


                // Determine whether the cable should reel in
                float triggerPullPercent = 0;
                foreach(Hand hand in handsHolding){
                    OVRInput.Controller controller = hand.GetComponent<OVRHandControllerLink>().controller;
                    OVRInput.Axis1D trigger = OVRInput.Axis1D.PrimaryIndexTrigger;
                    triggerPullPercent = Mathf.Max(triggerPullPercent, OVRInput.Get(trigger, controller));
                }
                // Reel in cable at a speed determined by the trigger pull percentage, activate haptics
                if(triggerPullPercent > reelInTriggerThreshold){
                    ReelInCable(triggerPullPercent * reelInRate, Time.fixedDeltaTime, bodyParts.playerRigidbody, -newHookToLauncher.normalized);
                    
                    foreach(Hand hand in handsHolding){
                        hand.SetHaptics(reelInHapticFrequency, reelInHapticAmplitude * triggerPullPercent);
                    }
                }
                else{
                    foreach(Hand hand in handsHolding){
                        hand.ClearHaptics();
                    }
                }


                // Player controls movement while grounded or climbing, but constrained to within radius of cable length
                if(isGrounded || isClimbing){
                    foreach(Hand hand in handsHolding){
                        hand.EnableFollowForce();
                    }
                }
  
                // Physics controls movement of the gun while the player is not grounded, and the player's body is attached to the hands by spring force
                else {
                    Hand leftHand = null;
                    Hand rightHand = null;

                    foreach(Hand hand in handsHolding){
                        hand.DisableFollowForce();

                        if(hand.left) leftHand = hand;
                        else{ rightHand = hand; }
                    }

                    if(leftHand) leftBodyTarget = bodyParts.transform.position - (bodyParts.leftControllerTransform.position - leftHand.transform.position);
                    if(rightHand) rightBodyTarget = bodyParts.transform.position - (bodyParts.rightControllerTransform.position - rightHand.transform.position);

                    // Calculate the target body position based on which hands are climbing
                    if (leftHand && rightHand){
                        mainBodyTarget = (leftBodyTarget + rightBodyTarget) / 2;

                        currentSpringFrequency = twoHandedSpringFrequency;
                        bodyParts.playerRigidbody.useGravity = false;
                    }
                    else if(leftHand){
                        mainBodyTarget = leftBodyTarget;

                        currentSpringFrequency = oneHandedSpringFrequency;
                        bodyParts.playerRigidbody.useGravity = false;
                    }
                    else if (rightHand){
                        mainBodyTarget = rightBodyTarget;

                        currentSpringFrequency = oneHandedSpringFrequency;
                        bodyParts.playerRigidbody.useGravity = false;
                    }
                    else{
                        mainBodyTarget = transform.position;

                        currentSpringFrequency = 0;
                        bodyParts.playerRigidbody.useGravity = true;
                    }

                    Vector3 bodySpringForce = DampedSpring.GetDampedSpringAcceleration(
                        bodyParts.transform.position, 
                        mainBodyTarget, 
                        bodyParts.playerRigidbody.velocity - rb.velocity,
                        currentSpringFrequency, 
                        springDamping
                    );

                    if (leftHand || rightHand){
                        bodyParts.playerRigidbody.AddForce(bodySpringForce, ForceMode.Acceleration);
                    }


                    rb.isKinematic = false;
                    rb.useGravity = true;
                    float combinedMass = rb.mass + (handsHolding.Count * handsHolding[0].GetMass());
                    //float combinedMass = rb.mass;
                    rb.AddForce(new Vector3(0, -9.81f * combinedMass, 0), ForceMode.Force);


                    // Set haptic strength based on distance from ideal position
                    foreach(Hand hand in handsHolding){
                        float handDistance = (mainBodyTarget - bodyParts.transform.position).magnitude;
                        float gradient = Mathf.Clamp(handDistance / maxSwingingHapticDistance, swingingHapticBaseGradient, 1);
                        hand.SetHaptics(swingingHapticFrequency, swingingHapticMaxAmplitude * gradient);
                    } 
                }
            }
            
        }

        else{
            if(handsHolding.Count > 0){
                foreach(Hand hand in handsHolding){
                    hand.EnableFollowForce();
                }
            }
        }


        if (currentHookState == HookState.Launched || currentHookState == HookState.Hooked || currentHookState == HookState.Failed){
            RenderCable(cableOriginTransform.position, attachedHook.transform.position);
        }
        else if(currentHookState == HookState.Loaded){
            RenderCable(cableOriginTransform.position, barrelTipTransform.position);
        }
        else{
            HideCable();
        }
    }



    // ~~~ Regular Functions ~~~ //

    public void RenderCable(Vector3 startPosition, Vector3 endPosition){
        // Enables the attached line renderer and sets its endpoints between the launcher and hook
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
        lineRenderer.enabled = true;
    }
    public void HideCable(){
        lineRenderer.enabled = false;
    }



    // ~~~ OnEvent function calls ~~~ //

    public void ReloadHook(){
        if(currentHookState == HookState.Unloaded){
            loadedHook = Instantiate(hookLoadedModel, hookSpawnTransform.position, hookSpawnTransform.rotation, this.transform);

            currentHookState = HookState.Loaded;
            cableLength = minCableLength;
        }
    }


    public void LaunchHook(){
        if(currentHookState == HookState.Loaded){
            Destroy(loadedHook);
            loadedHook = null;

            attachedHook = Instantiate(hookPrefab, hookSpawnTransform.position, hookSpawnTransform.rotation).GetComponent<Hook>();
            attachedHook.Attach(this);

            currentHookState = HookState.Launched;
   
        }       
    }


    public void OnHookSuccess(){
        currentHookState = GrapplingHookLauncher.HookState.Hooked;

        hookAnchor = attachedHook.transform.position;
        cableLength = (hookAnchor - transform.position).magnitude;
    }
    public void OnHookFail(){
        currentHookState = GrapplingHookLauncher.HookState.Failed;
        cableLength = minCableLength;
    }


    private void ReelInCable(float rate, float timeStep){
        // Continues to reel in the cable length until it gets to the minimum length
        if(cableLength > minCableLength){
            float distanceMoved = rate * timeStep;
            cableLength -= distanceMoved;
        }
        else{
            cableLength = minCableLength;
        }    
    }
    private void ReelInCable(float rate, float timeStep, Rigidbody playerRigidbody, Vector3 direction){
        // Continues to reel in the cable length until it gets to the minimum length

        if(cableLength > minCableLength){
            float distanceMoved = rate * timeStep;
            Vector3 translation = direction.normalized * distanceMoved;
            playerRigidbody.transform.Translate(translation, Space.World);
            //playerRigidbody.velocity = rate * direction.normalized;

            cableLength -= distanceMoved;
        }
        else{
            cableLength = minCableLength;
        }    
    }


    public void ReleaseCable(){
        if(currentHookState == HookState.Launched || currentHookState == HookState.Hooked || currentHookState == HookState.Failed){
            attachedHook.GetComponent<Hook>().Detach();
            attachedHook = null;
        
            currentHookState = HookState.Unloaded;
            cableLength = minCableLength;

            ReloadHook();
        } 
    }
}
