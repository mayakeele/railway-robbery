using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingedDoor : MonoBehaviour
{
    Vector3 hingeAxis;
    Vector3 hingeAnchor;

    private Rigidbody frameRigidbody;
    private Rigidbody doorRigidbody;
    private BoxCollider boxCollider;

    [SerializeField] private HingeJoint hinge;


    void Start()
    {
        frameRigidbody = GetComponentInParent<Rigidbody>();
        doorRigidbody = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        
    }
}
