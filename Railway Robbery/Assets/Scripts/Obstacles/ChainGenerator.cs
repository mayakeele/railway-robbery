using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainGenerator : MonoBehaviour
{  
    [Header("Chain Settings")]
    public int numSegments;
    public float segmentLength;
    public float turnAngle;

    [Header("Joint Settings")]
    public float maxTwist;
    public float maxAngle1;
    public float maxAngle2;

    [Header("Prefabs")]
    public GameObject segmentPrefab;
    public Transform startTransform;
    public Vector3 chainDirection;


    void Start()
    {
        chainDirection.Normalize();

        GenerateChain();
    }


    public void GenerateChain(){
        // Instantiates a chain of link prefabs and creates joints between them

        Rigidbody previousRigidbody = null;
                
        for(int i = 0; i < numSegments; i++){
            Vector3 currentOffset = chainDirection * segmentLength * i;
            Vector3 currentPosition = startTransform.position + currentOffset;

            float currentAngle = (turnAngle * i) % 360;
            Quaternion currentRotation = Quaternion.AngleAxis(currentAngle, chainDirection);


            GameObject currentSegment = Instantiate(segmentPrefab, currentPosition, currentRotation, startTransform);

            Joint currentJoint = currentSegment.GetComponent<Joint>();
            if(previousRigidbody) currentJoint.connectedBody = previousRigidbody;
            currentJoint.autoConfigureConnectedAnchor = true;

            previousRigidbody = currentSegment.GetComponent<Rigidbody>();
        }
    }
}
