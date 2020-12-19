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
    [SerializeField] private float oneHandedSpringFrequency;
    [SerializeField] private float twoHandedSpringFrequency;
    [SerializeField] private float springDamping;

    [Header("Prefabs")]
    [SerializeField] private GameObject hookPrefab;

    [Header("References")]
    [SerializeField] private Transform barrelTipTransform;
    [SerializeField] private Transform cableOriginTransform;
    [SerializeField] private Transform hookSpawnTransform;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private LineRenderer lineRenderer;


    // Hook variables
    [SerializeField] private HookState currentHookState;
    [SerializeField] private float cableLength;
    [SerializeField] private Hook attachedHook;
    [SerializeField] private Vector3 hookAnchor;
    private Coroutine reelCoroutine;

    // Hand-to-body spring variables
    private float currentSpringFrequency;
    private Vector3 leftBodyTarget;
    private Vector3 rightBodyTarget;
    private Vector3 mainBodyTarget;


    void Start()
    {
        currentHookState = HookState.Unloaded;
    }

    void FixedUpdate()
    {
        List<Hand> handsHolding = grabbable.heldBy;

        if(currentHookState == HookState.Hooked){

            // Use cable distance and physics to constrain the launcher's position and velocity
            Vector3 initialhookToLauncher = transform.position - hookAnchor;
            if(initialhookToLauncher.magnitude > cableLength){
                transform.position = hookAnchor + (initialhookToLauncher.normalized * cableLength);
            }
            Vector3 newHookToLauncher = transform.position - hookAnchor;

            Vector3 initialVelocity = rb.velocity;
            Vector3 newVelocity = Vector3.ProjectOnPlane(initialVelocity, newHookToLauncher).normalized * initialVelocity.magnitude * velocityDecayMultiplier;
            rb.velocity = newVelocity;


            // Player controls movement while grounded, but constrained to within radius of cable length
            if(handsHolding.Count > 0 && handsHolding[0].playerBodyParts.groundedStateTracker.isGrounded){
                foreach(Hand hand in handsHolding){
                    hand.EnableFollowForce();
                }
            }

            // Physics controls movement of the gun while the player is not grounded, and the player's body is attached to the hands by spring force
            else if(handsHolding.Count > 0 && handsHolding[0].playerBodyParts.groundedStateTracker.isGrounded == false){
                BodyPartReferences bodyParts = handsHolding[0].playerBodyParts;

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
                float combinedMass = rb.mass + (handsHolding.Count * handsHolding[0].GetMass());
                rb.AddForce(new Vector3(0, -9.81f * combinedMass, 0), ForceMode.Force);
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
            currentHookState = HookState.Loaded;
            cableLength = minCableLength;
        }
    }


    public void LaunchHook(){
        if(currentHookState == HookState.Loaded){
            currentHookState = HookState.Launched;

            attachedHook = Instantiate(hookPrefab, hookSpawnTransform.position, hookSpawnTransform.rotation).GetComponent<Hook>();
            attachedHook.Attach(this);
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


    public void StartReelIn(){
        if(currentHookState == HookState.Hooked){
            reelCoroutine = StartCoroutine(ReelCable());
        }
    }
    public void StopReelIn(){
        if(reelCoroutine != null) StopCoroutine(reelCoroutine);
    }
    private IEnumerator ReelCable(){
        // Continues to reel in the cable length until it gets to zero
        while(cableLength > minCableLength){
            cableLength -= reelInRate * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        cableLength = minCableLength;
    }


    public void ReleaseCable(){
        if(currentHookState == HookState.Launched || currentHookState == HookState.Hooked || currentHookState == HookState.Failed){
            attachedHook.GetComponent<Hook>().Detach();
            attachedHook = null;
        
            currentHookState = HookState.Unloaded;
            cableLength = minCableLength;
        } 
    }
}
