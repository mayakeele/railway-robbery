using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainCar : MonoBehaviour
{

    public GameObject trainCarGameobject;


    public TrainManager.CarType carType;
    public int seed;
    public float length;
    public float width;
    public float height;
    public float groundOffset;

    public Vector3 velocity;

    
    void Start() {
        
    }

    void Update() {
        
    }


    public void SetCarParameters(TrainManager.CarType inputCarType, int inputSeed, float inputLength, float inputWidth, float inputHeight, float inputGroundOffset){
        carType = inputCarType;
        seed = inputSeed;
        length = inputLength;
        width = inputWidth;
        height = inputHeight;
        groundOffset = inputGroundOffset;
    }
}
