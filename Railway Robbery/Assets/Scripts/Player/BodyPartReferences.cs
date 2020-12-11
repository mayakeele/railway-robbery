using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class BodyPartReferences : MonoBehaviour
{
    [Header("Transforms")]
    public Transform cameraTransform;
    public Transform bodyTransform;
    public Transform feetTransform;
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;
    public Transform leftHandTransform;
    public Transform rightHandTransform;

    [Header("Physics")]
    public Rigidbody playerRigidbody;
    public SphereCollider headCollider;
    public CapsuleCollider bodyCollider;
    public Rigidbody leftHandRigidbody;
    public Rigidbody rightHandRigidbody;

    [Header("Hand Scripts")]
    public Hand leftHand;
    public Hand rightHand;
    public ClimbingHand leftClimbingHand;
    public ClimbingHand rightClimbingHand;

    [Header("Player Manager Components")]
    public InputHandler inputHandler;
    public BodyManager bodyManager;
    public LocomotionManager locomotionManager;
    public ClimbingManager climbingManager;
    public HealthManager healthManager;
    public GroundedStateTracker groundedStateTracker;
}
