using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCar : MonoBehaviour
{
    [HideInInspector] public TrainPartPrefabs trainPartPrefabs;


    private void Awake() {
        trainPartPrefabs = GameObject.FindGameObjectWithTag("TrainPartPrefabsContainer").GetComponent<TrainPartPrefabs>();
    }


    public GameObject GenerateCar(int seed, float length, float width, float height, float groundOffset){
        float halfLength = length / 2;
        float halfWidth = width / 2;
        float halfHeight = height / 2;

        GameObject parentObject = new GameObject("Boxcar");
        Transform parentTransform = parentObject.transform;

        // Create a base platform to build upon
        GameObject platform = trainPartPrefabs.CreateBasePlatform(length, width, 0.15f, groundOffset);
        platform.transform.SetParent(parentObject.transform);


        // Walls
        GameObject leftWall = trainPartPrefabs.CreateStraightWall(length, height, 0.1f, true);
        leftWall.transform.SetParent(parentObject.transform);
        leftWall.transform.position = new Vector3(-(width/2), (height/2) + groundOffset, 0);

        GameObject rightWall = trainPartPrefabs.CreateStraightWall(length, height, 0.1f, true);
        rightWall.transform.SetParent(parentObject.transform);
        rightWall.transform.position = new Vector3((width/2), (height/2) + groundOffset, 0);

        GameObject frontWall = trainPartPrefabs.CreateStraightWall(width, height, 0.1f, false);
        frontWall.transform.SetParent(parentObject.transform);
        frontWall.transform.position = new Vector3(0, (height/2) + groundOffset, (length/2));

        GameObject backWall = trainPartPrefabs.CreateStraightWall(width, height, 0.1f, false);
        backWall.transform.SetParent(parentObject.transform);
        backWall.transform.position = new Vector3(0, (height/2) + groundOffset, -(length/2));


        // Roof
        GameObject roof = Instantiate(trainPartPrefabs.slantedBoxcarRoof);
        roof.transform.SetParent(parentTransform);
        roof.transform.position = new Vector3(0, groundOffset + height, 0);
        
        Mesh roofMesh = roof.GetComponent<MeshFilter>().mesh;
        roofMesh.ScaleVerticesNonUniform(width, 0.75f, length);
        roof.GetComponent<MeshCollider>().sharedMesh = roofMesh;


        return parentObject;
    }
}
