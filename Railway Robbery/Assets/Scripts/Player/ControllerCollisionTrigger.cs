using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCollisionTrigger : MonoBehaviour
{
    public bool isLeftController;

    private string climbableTag = "Climbable";
    private int triggerLayer;

    [HideInInspector] public bool isColliding;
    [HideInInspector] public Transform collidingTransform;


    private void Awake() {
        triggerLayer = LayerMask.NameToLayer("ClimbingTrigger");
    }

    void Update()
    {
        gameObject.layer = triggerLayer;
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
