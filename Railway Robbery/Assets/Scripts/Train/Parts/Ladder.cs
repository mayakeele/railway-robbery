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
    private void Awake() {
        trainPartFactory = GameObject.FindObjectOfType<TrainPartFactory>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    

}
