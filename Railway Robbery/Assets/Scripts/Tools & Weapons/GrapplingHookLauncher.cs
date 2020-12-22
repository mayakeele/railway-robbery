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
    [SerializeField] private float reelInTriggerThreshold;
    [SerializeField] private float oneHandedSpringFrequency;
    [SerializeField] private float twoHandedSpringFrequency;
    [SerializeField] private float springDamping;
    [SerializeField] private float cablePullMultiplier;
    [SerializeField] private float reelPercentToMovePlayer;

    [Header("Cable Settings")]
    [SerializeField] private float minCableLength;
    [SerializeField] private int numCableSegments;
    [SerializeField] private float cableBreakHandDistance;
    [SerializeField] private AnimationCurve lowestCablePointAtDistance;
    [SerializeField] private AnimationCurve cableShapeCurve;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [Space]
    [SerializeField] private List<AudioClip> launchSounds;
    [SerializeField] [Range(0, 1)] private float launchVolume;
    [Space]
    [SerializeField] private AudioClip reelSound;
    [SerializeField] [Range(0, 1)] private float reelVolume;
    [SerializeField] private float reelPitchMin;
    [SerializeField] private float reelPitchMax;
    [Space]
    [SerializeField] private List<AudioClip> releaseSounds;
    [SerializeField] [Range(0, 1)] private float releaseVolume;

    [SerializeField] private List<AudioClip> cableSnapSounds;
    [SerializeField] [Range(0, 1)] private float cableSnapVolume;

    [Header("Haptics")]
    [SerializeField] [Range(0, 1)] private float launchHapticFrequency;
    [SerializeField] [Range(0, 1)] private float launchHapticAmplitude;
    [SerializeField] private float launchHapticDuration;
    [Space]
    [SerializeField] [Range(0, 1)] private float hookImpactHapticFrequency;
    [SerializeField] [Range(0, 1)] private float hookImpactHapticAmplitude;
    [SerializeField] private float hookImpactHapticDuration;
    [Space]
    [SerializeField] [Range(0, 1)] private float reelInHapticFrequency;
    [SerializeField] [Range(0, 1)] private float reelInHapticAmplitude;
    [Space]
    [SerializeField] [Range(0, 1)] private float swingingHapticFrequency;
    [SerializeField] [Range(0, 1)] private float swingingHapticMaxAmplitude;
    [SerializeField] [Range(0, 1)] private float swingingHapticBaseGradient;
    [SerializeField] private float maxSwingingHapticDistance;
    [Space]
    [SerializeField] [Range(0, 1)] private float cableSnapHapticFrequency;
    [SerializeField] [Range(0, 1)] private float cableSnapHapticAmplitude;
    [SerializeField] private float cableSnapHapticDuration;

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
        swingingVelocity = Vector3.zero;

        currentHookState = HookState.Unloaded;
        //ReloadHook();
        //LaunchHook();
    }

    void Update()
    {
        List<Hand> handsHolding = grabbable.heldBy;

        if(currentHookState == HookState.Hooked){

            // Use cable distance and physics to constrain the launcher's position and velocity
            Vector3 initialhookToLauncher = cableOriginTransform.position - hookAnchor;
            Vector3 newHookToLauncher = initialhookToLauncher;

            Vector3 originOffset = cableOriginTransform.position - transform.position;

            if(initialhookToLauncher.magnitude > cableLength){

                transform.position = hookAnchor + (initialhookToLauncher.normalized * cableLength) - originOffset;

                newHookToLauncher = cableOriginTransform.position - hookAnchor;
                Vector3 initialVelocity = rb.velocity;
                Vector3 newVelocity = Vector3.ProjectOnPlane(initialVelocity, newHookToLauncher).normalized * initialVelocity.magnitude;
                rb.velocity = newVelocity;

                rb.AddForceAtPosition(-newHookToLauncher * rb.mass * 9.81f * cablePullMultiplier, cableOriginTransform.position);
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
                    ReelInCable(triggerPullPercent * reelInRate, Time.fixedDeltaTime, bodyParts.playerRigidbody, handsHolding);
                    
                    audioSource.clip = reelSound;
                    audioSource.pitch = triggerPullPercent * (reelPitchMax - reelPitchMin) + reelPitchMin;
                    audioSource.loop = true;
                    if(!audioSource.isPlaying){
                        audioSource.Play();
                    }

                    foreach(Hand hand in handsHolding){
                        hand.SetHaptics(reelInHapticFrequency, reelInHapticAmplitude * triggerPullPercent);
                    }
                }
                else{
                    audioSource.clip = null;
                    audioSource.pitch = 1;
                    audioSource.loop = false;
                    audioSource.Stop();

                    foreach(Hand hand in handsHolding){
                        hand.ClearHaptics();
                    }
                }


                // Player controls movement while grounded or climbing, but constrained to within radius of cable length
                if(isGrounded || isClimbing){
                    foreach(Hand hand in handsHolding){
                        hand.EnableFollowForce();
                    }

                    // Break the cable if it is stretched too far
                    Vector3 controllerPosition = handsHolding[0].follow.position;
                    Vector3 handPosition = handsHolding[0].transform.position;
                    float controllerToAnchorDistance = (controllerPosition - hookAnchor).magnitude;
                    if(controllerToAnchorDistance - cableLength >= cableBreakHandDistance){
                        SnapCable();
                        return;
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


        // Give hands control of movement if not hooked
        else{
            if(handsHolding.Count > 0){
                foreach(Hand hand in handsHolding){
                    hand.EnableFollowForce();
                }
            }
        }


        // Render cable differently based on the current hook state
        if (currentHookState == HookState.Launched){
            RenderCableLinear(cableOriginTransform.position, attachedHook.transform.position);
        }
        else if(currentHookState == HookState.Hooked || currentHookState == HookState.Failed){
            RenderCableCurved(cableOriginTransform.position, attachedHook.transform.position, numCableSegments);
        }
        else if(currentHookState == HookState.Loaded){
            RenderCableLinear(cableOriginTransform.position, barrelTipTransform.position);
        }
        else{
            HideCable();
        }
    }



    // ~~~ Regular Functions ~~~ //

    public void RenderCableLinear(Vector3 startPosition, Vector3 endPosition){
        // Enables the attached line renderer and sets its endpoints between the launcher and hook
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
        lineRenderer.enabled = true;
    }
    public void RenderCableCurved(Vector3 startPosition, Vector3 endPosition, int numSegments){
        // Enables the attached line renderer, sets its endpoints between the launcher and hook and accounts sag due to slack
        int numVertices = numSegments + 1;

        float distanceBetween = Mathf.Clamp((endPosition - startPosition).magnitude, 0, cableLength);
        float distancePercentBetween = distanceBetween / cableLength;

        float curveHeight = -cableLength * lowestCablePointAtDistance.Evaluate(distancePercentBetween);

        // Evaluate the cable shape curve at multiple sample points
        float segmentLength = distanceBetween / numSegments;
        List<Vector3> samplePoints = new List<Vector3>();
        for(int i = 0; i < numVertices; i++){
            float samplePercent = (float) i / numSegments;
            float heightOffset = curveHeight * cableShapeCurve.Evaluate(samplePercent);

            Vector3 currentPoint = Vector3.Lerp(startPosition, endPosition, samplePercent) + new Vector3(0, heightOffset, 0);
            samplePoints.Add(currentPoint);
        }

        lineRenderer.positionCount = samplePoints.Count;
        lineRenderer.SetPositions(samplePoints.ToArray());
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

            Vector3 parentVelocity = Vector3.zero;
            if(grabbable.heldBy.Count > 0) parentVelocity = grabbable.heldBy[0].playerBodyParts.playerRigidbody.velocity;
            attachedHook.Launch(parentVelocity);

            currentHookState = HookState.Launched;

            foreach(Hand hand in grabbable.heldBy){
                hand.SetHapticsDuration(launchHapticFrequency, launchHapticAmplitude, launchHapticDuration);
            }

            audioSource.PlayClip(launchSounds.RandomChoice(), launchVolume);
        }       
    }


    public void OnHookSuccess(){
        currentHookState = GrapplingHookLauncher.HookState.Hooked;

        hookAnchor = attachedHook.transform.position;
        cableLength = (hookAnchor - transform.position).magnitude;

        foreach(Hand hand in grabbable.heldBy){
            hand.SetHapticsDuration(hookImpactHapticFrequency, hookImpactHapticAmplitude, hookImpactHapticDuration);
        }
    }
    public void OnHookFail(){
        currentHookState = GrapplingHookLauncher.HookState.Failed;
        cableLength = minCableLength;

        foreach(Hand hand in grabbable.heldBy){
            hand.SetHapticsDuration(hookImpactHapticFrequency, hookImpactHapticAmplitude, hookImpactHapticDuration);
        }
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
    private void ReelInCable(float rate, float timeStep, Rigidbody playerRigidbody, List<Hand> handsHolding){
        // Continues to reel in the cable length until it gets to the minimum length

        if(cableLength > minCableLength){
            Vector3 launcherToHook = hookAnchor - cableOriginTransform.position;

            float distanceMoved = rate * timeStep;
            Vector3 translation = launcherToHook.normalized * distanceMoved;

            if(launcherToHook.magnitude >= cableLength * reelPercentToMovePlayer){
                playerRigidbody.transform.Translate(translation, Space.World);
                //transform.Translate(translation, Space.World);
                foreach(Hand hand in handsHolding){
                    hand.transform.Translate(translation, Space.World);
                }
            }
            

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

            audioSource.PlayClip(releaseSounds.RandomChoice(), releaseVolume);

            ReloadHook();
        } 
    }

    public void SnapCable(){
        if(currentHookState == HookState.Hooked){
            attachedHook.GetComponent<Hook>().Detach();
            attachedHook = null;
        
            currentHookState = HookState.Unloaded;
            cableLength = minCableLength;

            audioSource.PlayClip(cableSnapSounds.RandomChoice(), cableSnapVolume);
            
            foreach(Hand hand in grabbable.heldBy){
                hand.SetHapticsDuration(cableSnapHapticFrequency, cableSnapHapticAmplitude, cableSnapHapticDuration);
            }

            //ReloadHook();
        } 
    }
}
