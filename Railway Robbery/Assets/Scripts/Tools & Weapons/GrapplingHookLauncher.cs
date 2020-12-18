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
    [SerializeField] private float oneHandedSpringFrequency;
    [SerializeField] private float twoHandedSpringFrequency;

    [Header("Prefabs")]
    [SerializeField] private GameObject hookPrefab;

    [Header("References")]
    [SerializeField] private Transform barrelTip;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Grabbable grabbable;


    [HideInInspector] public HookState currentHookState;
    [SerializeField] private float cableLength;
    [SerializeField] private Hook attachedHook;
    [SerializeField] private Vector3 hookAnchor;


    void Start()
    {
        currentHookState = HookState.Unloaded;
    }

    void FixedUpdate()
    {
        if(currentHookState == HookState.Hooked){

            // Use cable distance and physics to constrain the launcher's position and velocity
            Vector3 initialhookToLauncher = transform.position - hookAnchor;
            if(initialhookToLauncher.magnitude > cableLength){
                transform.position = hookAnchor + (initialhookToLauncher.normalized * cableLength);
            }
            Vector3 newhookToLauncher = transform.position - hookAnchor;

            Vector3 initialVelocity = rb.velocity;
            Vector3 newVelocity = Vector3.ProjectOnPlane(initialVelocity, newhookToLauncher).normalized * initialVelocity.magnitude;
            rb.velocity = newVelocity;


            List<Hand> handsHolding = grabbable.heldBy;

            // Player controls movement while grounded, but constrained to within radius of cable length
            if(handsHolding.Count > 0 && handsHolding[0].playerBodyParts.groundedStateTracker.isGrounded){
                foreach(Hand hand in handsHolding){
                    hand.FollowController();
                }           
            }
            // Physics controls movement of the gun while the player is not grounded, and the player's body is attached to the hands by spring force
            else if(handsHolding.Count > 0 && handsHolding[0].playerBodyParts.groundedStateTracker.isGrounded == false){
                foreach(Hand hand in handsHolding){
                    hand.follow = hand.transform;
                }
            }
        }
        
    }



    public void LaunchHook(){
        currentHookState = HookState.Launched;
    }

    public void StartReelIn(){

    }

    public void StopReelIn(){

    }

    public void ReleaseCable(){
        currentHookState = HookState.Unloaded;
    }

    public void ReloadHook(){
        currentHookState = HookState.Loaded;
    }


    public void OnHookSuccess(){
        currentHookState = GrapplingHookLauncher.HookState.Hooked;

        hookAnchor = attachedHook.transform.position;
        cableLength = (hookAnchor - transform.position).magnitude;
    }

    public void OnHookFail(){
        currentHookState = GrapplingHookLauncher.HookState.Failed;
    }
}
