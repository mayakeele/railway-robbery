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
        GameObject parentObject = new GameObject("Boxcar");
        Transform parentTransform = parentObject.transform;

        // Create a base platform to build upon
        GameObject platform = trainPartPrefabs.CreateBasePlatform(length, width, 0.15f, groundOffset);
        platform.transform.SetParent(parentObject.transform);


        // Specialist car generation code goes here
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

        return parentObject;
    }
}
