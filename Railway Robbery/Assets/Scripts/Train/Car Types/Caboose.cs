using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caboose : MonoBehaviour
{
    private TrainPartFactory trainPartFactory;
    void Awake() {
        trainPartFactory = GameObject.FindGameObjectWithTag("TrainPartPrefabsContainer").GetComponent<TrainPartFactory>();
    }


    public GameObject GenerateCar(int seed, float carLength, float carWidth, float carHeight, float groundOffset){
        
        float halfLength = carLength / 2;
        float halfWidth = carWidth / 2;
        float halfHeight = carHeight / 2;

        GameObject parentObject = new GameObject("Boxcar");
        Transform parentTransform = parentObject.transform;

        // Create a base platform to build upon
        GameObject platform = trainPartFactory.CreateBasePlatform(carLength, carWidth, 0.15f, groundOffset);
        platform.transform.SetParent(parentObject.transform);


        // Specialist car generation code goes here
        

        return parentObject;
    }
}
