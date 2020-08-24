using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{

    public enum CarType {
        Default,
        Locomotive,
        Caboose,
        FlatCar,
        BoxCar,
        OreCar,
        PassengerCar,
        TankCar,
        StockCar,
        SpecialCar
    }


    public GameObject trainCarTypeContainer;
    public GameObject[] trainCars;


    public int numTrainCars;

    public float standardCarLength;
    public float standardCarWidth;
    public float standardCarHeight;

    public float connectorLength;
    private float totalCarLength;


    public Transform trainPosition;
    public Vector3 trainVelocity;


    private void Awake() {
        trainCarTypeContainer = GameObject.FindGameObjectWithTag("TrainCarTypeContainer");

        totalCarLength = standardCarLength + (connectorLength * 2);
    }

    void Start() {
        CreateNewTrain(numTrainCars);
    }

    void Update() {
        
    }


    public GameObject GenerateCarOfType(CarType carType, int seed, float length, float width, float height, float groundOffset){
        GameObject carObject;

        switch(carType){
            case CarType.FlatCar:
                carObject = trainCarTypeContainer.GetComponent<FlatCar>().GenerateCar(seed, length, width, height, groundOffset);
                break;
            case CarType.BoxCar:
                carObject = trainCarTypeContainer.GetComponent<BoxCar>().GenerateCar(seed, length, width, height, groundOffset);
                break;
            default:
                carObject = new GameObject();
                break;
        }

        TrainCar carScript = carObject.AddComponent<TrainCar>();
        carScript.SetCarParameters(carType, seed, length, width, height, groundOffset);
        
        return carObject;
    }


    public void CreateNewTrain(int numCars){
        int trainSeed = Random.Range(1, 65535);

        for (int i = 0; i < numCars; i++){
            GameObject car = GenerateCarOfType(CarType.BoxCar, trainSeed, standardCarLength, standardCarWidth, standardCarHeight, 1);

            car.name = "Train Car " + (int) i;
            car.transform.parent = this.transform;
            car.transform.position = new Vector3(0, 0, -(totalCarLength) * i);
        }
    }

}
