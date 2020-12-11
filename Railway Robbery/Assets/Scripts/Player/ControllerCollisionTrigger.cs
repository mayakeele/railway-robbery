using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCollisionTrigger : MonoBehaviour
{
    public bool isLeftController;

    [HideInInspector] public bool isColliding;


    void Update()
    {
        gameObject.layer = LayerMask.NameToLayer("ClimbingTrigger");
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
