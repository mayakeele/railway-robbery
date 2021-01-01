using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerCollisionTrigger : MonoBehaviour
{
    public bool isLeftController;

    public string targetClimbableTag;
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
        if (other.gameObject.tag == targetClimbableTag){
            isColliding = true;
            collidingTransform = other.gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == targetClimbableTag){
            isColliding = false;
            collidingTransform = null;
        }
    }
}
