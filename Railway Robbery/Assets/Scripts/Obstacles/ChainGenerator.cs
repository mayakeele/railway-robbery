using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChainGenerator : MonoBehaviour
{
    public enum AttachedMassDistrubution{
        Zero,
        TopDown,
        Equal,
        Neighbors
        //Edges,
        //Middle
    }


    [Header("Chain Settings")]
    public int numSegments;
    public float segmentLength;
    public float turnAngle;
    public bool lockFirstSegment;
    public bool lockLastSegment;
    public AttachedMassDistrubution attachedMassDistrubution;
    public int neighborRadius = 1;

    private float segmentMass;


    [Header("Prefabs")]
    public GameObject segmentPrefab;
    public Transform startTransform;
    public Vector3 chainDirection;


    void Start()
    {
        chainDirection.Normalize();
        segmentMass = segmentPrefab.GetComponent<DynamicClimbable>().rb.mass;

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


            GameObject currentSegment = PrefabUtility.InstantiatePrefab(segmentPrefab) as GameObject;
            currentSegment.transform.position = currentPosition;
            currentSegment.transform.rotation = currentRotation;
            currentSegment.transform.parent = startTransform;
            

            Rigidbody currentRigidbody = currentSegment.GetComponent<Rigidbody>();
            DynamicClimbable currentDynamicClimbable = currentSegment.GetComponent<DynamicClimbable>();

            switch(attachedMassDistrubution){
                case AttachedMassDistrubution.Zero:
                    currentDynamicClimbable.attachedMass = 0;
                break;

                case AttachedMassDistrubution.TopDown:
                    currentDynamicClimbable.attachedMass = segmentMass * (numSegments - (i + 1));
                break;

                case AttachedMassDistrubution.Equal:
                    currentDynamicClimbable.attachedMass = segmentMass * (numSegments-1);
                break;

                case AttachedMassDistrubution.Neighbors:
                    float neighborMass = 0;
                    for (int n = -neighborRadius; n <= neighborRadius; n++){
                        int neighborIndex = i + n;
                        if(neighborIndex >= 0 && neighborIndex < numSegments && neighborIndex != i){
                            neighborMass += segmentMass;
                        }
                    }
                    currentDynamicClimbable.attachedMass = neighborMass;
                break;
            }
            

            if((lockFirstSegment && i == 0) || (lockLastSegment && i == numSegments-1)){
                currentRigidbody.isKinematic = true;
            }

            Joint currentJoint = currentSegment.GetComponent<Joint>();
            if(previousRigidbody) currentJoint.connectedBody = previousRigidbody;
            currentJoint.autoConfigureConnectedAnchor = true;

            previousRigidbody = currentSegment.GetComponent<Rigidbody>();
        }
    }
}
