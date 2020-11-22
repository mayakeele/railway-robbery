using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class LegRenderer : MonoBehaviour
{
    public GameObject middleLegSegment;
    public GameObject endLegSegment;

    private FastIKFabric IKSolver;

    private int numJoints;
    private int numSegments;

    // Starts at the innermost joint, ends at the ending joint
    private Transform[] jointHierarchy;
    private Transform[] segmentHierarchy;


    void Start()
    {
        IKSolver = GetComponent<FastIKFabric>();

        numJoints = IKSolver.ChainLength + 1;
        numSegments = IKSolver.ChainLength;

        jointHierarchy = new Transform[numJoints];
        segmentHierarchy = new Transform[numSegments];

        // Record the order of joints (calculated from outside to inside, stored from inside to outside)
        Transform currentTransform = this.transform;
        jointHierarchy[numJoints - 1] = currentTransform;
        for(int i = numJoints - 2; i >= 0; i--){
            currentTransform = currentTransform.parent;
            jointHierarchy[i] = currentTransform;
        }

        // Create segment prefabs as children of their connected joints
        for(int i = 0; i < numSegments; i++){
            Transform startingJoint = jointHierarchy[i];
            Transform endingJoint = jointHierarchy[i+1];

            GameObject segmentObject;
            if(i == numSegments - 1){ segmentObject = Instantiate(endLegSegment, startingJoint.position, Quaternion.identity); }
            else{ segmentObject = Instantiate(middleLegSegment, startingJoint.position, Quaternion.identity); }
            segmentObject.transform.parent = startingJoint;
            
            segmentHierarchy[i] = segmentObject.transform;

            float segmentLength = Vector3.Distance(startingJoint.position, endingJoint.position);

            Mesh segmentMesh = segmentObject.GetComponent<MeshFilter>().mesh;
            segmentMesh.ScaleVerticesNonUniform(1, segmentLength, 1);
        }
    }


    void Update()
    {
        // Rotate each leg segment to connect with the following joint
        for(int i = 0; i < numSegments; i++){
            Transform startingJoint = jointHierarchy[i];
            Transform endingJoint = jointHierarchy[i+1];

            Transform currentSegment = segmentHierarchy[i];

            Vector3 toNextJoint = endingJoint.position - startingJoint.position;

            currentSegment.rotation = Quaternion.FromToRotation(Vector3.up, toNextJoint);
        }
    }
}
