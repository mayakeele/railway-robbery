using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCollisionTrigger : MonoBehaviour
{
    public bool isLeftController;

    private string climbableTag = "Climbable";
    [HideInInspector] public bool isColliding;
    [HideInInspector] public Transform collidingTransform;

    void Update()
    {
        gameObject.layer = LayerMask.NameToLayer("ClimbingTrigger");
    }


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == climbableTag){
            isColliding = true;
            collidingTransform = other.gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == climbableTag){
            isColliding = false;
            collidingTransform = null;
        }
    }
}
