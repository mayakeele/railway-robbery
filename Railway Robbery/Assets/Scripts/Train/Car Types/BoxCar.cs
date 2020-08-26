using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCar : MonoBehaviour
{
    [HideInInspector] public TrainPartPrefabs trainPartPrefabs;


    private void Awake() {
        trainPartPrefabs = GameObject.FindGameObjectWithTag("TrainPartPrefabsContainer").GetComponent<TrainPartPrefabs>();
    }


    public GameObject GenerateCar(int seed, float carLength, float carWidth, float carHeight, float groundOffset){
        float halfLength = carLength / 2;
        float halfWidth = carWidth / 2;
        float halfHeight = carHeight / 2;

        GameObject parentObject = new GameObject("Boxcar");
        Transform parentTransform = parentObject.transform;

        // Create a base platform to build upon
        GameObject platform = trainPartPrefabs.CreateBasePlatform(carLength, carWidth, 0.15f, groundOffset);
        platform.transform.SetParent(parentObject.transform);


        // Walls
        GameObject leftWall = trainPartPrefabs.CreateStraightWall(carLength, carHeight, 0.1f, true);
        leftWall.transform.SetParent(parentObject.transform);
        leftWall.transform.position = new Vector3(-halfWidth, halfHeight + groundOffset, 0);

        GameObject rightWall = trainPartPrefabs.CreateStraightWall(carLength, carHeight, 0.1f, true);
        rightWall.transform.SetParent(parentObject.transform);
        rightWall.transform.position = new Vector3(halfWidth, halfHeight + groundOffset, 0);

        GameObject frontWall = trainPartPrefabs.CreateStraightWall(carWidth, carHeight, 0.1f, false);
        frontWall.transform.SetParent(parentObject.transform);
        frontWall.transform.position = new Vector3(0, halfHeight + groundOffset, halfLength);

        GameObject backWall = trainPartPrefabs.CreateStraightWall(carWidth, carHeight, 0.1f, false);
        backWall.transform.SetParent(parentObject.transform);
        backWall.transform.position = new Vector3(0, halfHeight + groundOffset, -halfLength);


        // Roof
        GameObject roof = Instantiate(trainPartPrefabs.slantedBoxcarRoof);
        roof.transform.SetParent(parentTransform);
        roof.transform.position = new Vector3(0, groundOffset + carHeight, 0);
        
        Mesh roofMesh = roof.GetComponent<MeshFilter>().mesh;
        roofMesh.ScaleVerticesNonUniform(carWidth, 0.75f, carLength);
        roof.GetComponent<MeshCollider>().sharedMesh = roofMesh;


        // Place ladder(s) in random precalculated spots
        float sideInset = 0.6f;
        float distanceOut = 0.1f + 0.05f;

        Vector3[] possibleLadderPositions = new Vector3[] {
            // Back
            new Vector3(halfWidth - sideInset, groundOffset, -halfLength - distanceOut),
            new Vector3(-halfWidth + sideInset, groundOffset, -halfLength - distanceOut),

            // Left
            new Vector3(-halfWidth - distanceOut, groundOffset, -halfLength + sideInset),
            new Vector3(-halfWidth - distanceOut, groundOffset, halfLength - sideInset),

            // Front
            new Vector3(-halfWidth + sideInset, groundOffset, halfLength + distanceOut),
            new Vector3(halfWidth - sideInset, groundOffset, halfLength + distanceOut),

            // Right
            new Vector3(halfWidth + distanceOut, groundOffset, halfLength - sideInset),
            new Vector3(halfWidth + distanceOut, groundOffset, -halfLength + sideInset)
            
        };
        int[] possibleLadderYRotations = new int[] {
            0,
            0,

            90,
            90,

            180,
            180,
            
            270,
            270
        };

        int[] ladderPosIndices = ArrayUtils.ChooseRandomIndices(4, possibleLadderPositions.Length, false);
        for (int i = 0; i < ladderPosIndices.Length; i++){
            int index = ladderPosIndices[i];

            Vector3 ladderPos = possibleLadderPositions[index];
            float ladderYRot = possibleLadderYRotations[index];

            float ladderHeight = Random.Range(1.5f, carHeight);

            GameObject ladder = trainPartPrefabs.CreateLadder(ladderHeight);
            ladder.transform.parent = parentTransform;

            ladder.transform.position = ladderPos;
            ladder.transform.eulerAngles = new Vector3(0, ladderYRot, 0);
        }


        return parentObject;
    }

}
