using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCollisionTrigger : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private ClimbingManager climbingManager;
    public bool isLeftController;

    [HideInInspector] public bool isColliding;

    void Start()
    {

    }

    void Update()
    {
 
    }


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Climbable"){
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Climbable"){
            isColliding = false;
        }
    }
}
