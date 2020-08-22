using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatCar : MonoBehaviour
{

    [HideInInspector] public TrainPartPrefabs trainPartPrefabs;


    private void Awake() {
        trainPartPrefabs = GameObject.FindGameObjectWithTag("TrainPartPrefabsContainer").GetComponent<TrainPartPrefabs>();
    }


    public GameObject GenerateCar(int seed, float length, float width, float height, float groundOffset){
        GameObject thisCar = new GameObject("Flatcar");

        // Create a base platform to build upon
        GameObject platform = trainPartPrefabs.CreateTrainPlatform(length, width, height, groundOffset);
        platform.transform.SetParent(thisCar.transform);


        // Specialist car generation code goes here
        

        return thisCar;
    }
}
