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
        GameObject parentObject = new GameObject("Flatcar");
        Transform parentTransform = parentObject.transform;

        // Create a base platform to build upon
        GameObject platform = trainPartPrefabs.CreateBasePlatform(length, width, 0.15f, groundOffset);
        platform.transform.SetParent(parentTransform);


        // Create a ladder that floats in the air (spoooookyyyy)
        GameObject ladder = trainPartPrefabs.CreateLadder(Random.Range(1f, 5f));
        ladder.transform.parent = parentTransform;
        ladder.transform.position = new Vector3(0, groundOffset + 1, 0);

        return parentObject;
    }
}
