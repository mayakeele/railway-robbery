using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    
    //public Transform carTransform;

    public float height;
    public float rungDistance;

    public bool isMoveable = false;


    private TrainPartFactory trainPartFactory;
    void Start() {
        trainPartFactory = GameObject.FindObjectOfType<TrainPartFactory>();
    }

     void Update()
    {
        
    }


    public void Initialize(float height, float rungDistance){
        this.height = height;
        this.rungDistance = rungDistance;
    }


   

    

}
