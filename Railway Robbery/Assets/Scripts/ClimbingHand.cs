using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingHand : MonoBehaviour
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private ClimbingManager climbingManager;
    public bool leftController;

    void Start()
    {
        //inputHandler = GetComponent<InputHandler>();
    }

    void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Climbable"){
            if(leftController){
                climbingManager.leftHandColliding = true;
            }
            else{
                climbingManager.rightHandColliding = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Climbable"){
            if(leftController){
                climbingManager.leftHandColliding = false;
            }
            else{
                climbingManager.rightHandColliding = false;
            }
        }
    }
}
