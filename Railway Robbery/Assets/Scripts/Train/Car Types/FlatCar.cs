using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatCar : MonoBehaviour
{

    private TrainPartFactory trainPartFactory;
    private void Awake() {
        trainPartFactory = GameObject.FindObjectOfType<TrainPartFactory>();
    }


    public GameObject GenerateCar(int seed, float length, float width, float height, float groundOffset){
        GameObject parentObject = new GameObject("Flatcar");
        Transform parentTransform = parentObject.transform;

        // Create a base platform to build upon
        GameObject platform = trainPartFactory.CreateBasePlatform(length, width, 0.15f, groundOffset);
        platform.transform.SetParent(parentTransform);

        // Generate cargo on the deck of this car
        CargoGenerator cargoGenerator = gameObject.AddComponent<CargoGenerator>();

        GameObject cargo = cargoGenerator.GenerateCargoRoom(width, length);

        cargo.transform.parent = parentTransform;
        cargo.transform.position = new Vector3(0, groundOffset, 0);

        return parentObject;
    }
}
