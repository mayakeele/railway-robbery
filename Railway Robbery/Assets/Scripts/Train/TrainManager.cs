﻿using System.Collections;
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
    public CarType test_cartype;

    public float standardCarLength;
    public float standardCarWidth;
    public float standardCarHeight;

    public float connectorLength;
    private float totalCarLength = 0;


    public Transform trainPosition;
    public Vector3 trainVelocity;


    void Start() {
        trainCarTypeContainer = GameObject.FindGameObjectWithTag("TrainCarTypeContainer");
        trainCars = new GameObject[numTrainCars];

        standardCarLength = Random.Range(8f, 20f);
        standardCarWidth = Random.Range(2.5f, 5f);
        standardCarHeight = Random.Range(2f, 4.3f);

        CreateNewTrain(numTrainCars);
    }

    void Update() {
        
    }


    public GameObject GenerateCarOfType(CarType carType, int seed, float length, float width, float height, float groundOffset){
        Random.InitState(seed);

        GameObject carObject;

        switch(carType){
            case CarType.FlatCar:
                length = length.RoundToMultiple(1f, false);
                width = width.RoundToMultiple(0.5f, false);
                height = height.RoundToMultiple(1.5f, false);

                carObject = trainCarTypeContainer.GetComponent<FlatCar>().GenerateCar(seed, length, width, height, groundOffset);
                
                break;

            case CarType.BoxCar:
                length = length.RoundToMultiple(1.5f, false);
                width = 3;//width.RoundToMultiple(1f, false);
                height = 3;//height.RoundToMultiple(3f, false);

                carObject = trainCarTypeContainer.GetComponent<BoxCar>().GenerateCar(seed, length, width, height, groundOffset);
                break;

            case CarType.Caboose:
                length = Random.Range(6f, 12f).RoundToMultiple(2f, false) + 1;
                width = 3f;
                height = 4.5f;

                carObject = trainCarTypeContainer.GetComponent<Caboose>().GenerateCar(seed, length, width, height, groundOffset);
                break;

            case CarType.PassengerCar:
                length = Random.Range(9f, 21f).RoundToMultiple(3f, false) + 3;
                width = 4f;
                height = 4f;

                carObject = trainCarTypeContainer.GetComponent<PassengerCar>().GenerateCar(seed, length, width, height, groundOffset);
                break;

            default:
                Debug.LogError("Undefined train car type!");
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
            GameObject car = GenerateCarOfType(test_cartype, trainSeed + i, standardCarLength, standardCarWidth, standardCarHeight, 1);

            car.name = "Train Car " + (int) i;
            car.transform.parent = this.transform;

            // If there is a car in front of this one, add half its length to the total length
            totalCarLength += i > 0 ? (trainCars[i-1].GetComponentInChildren<TrainCar>().length / 2) + connectorLength : 0;
            // Add half of the current car's length to the total length
            totalCarLength += (car.GetComponentInChildren<TrainCar>().length / 2) + connectorLength;

            car.transform.position = new Vector3(0, 0, -totalCarLength);
            
            trainCars[i] = car;
        }
    }

}
