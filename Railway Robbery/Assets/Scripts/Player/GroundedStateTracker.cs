using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedStateTracker : MonoBehaviour
{
    public bool isGrounded;
    public LayerMask groundLayers;

    private int numCollisions;


    void Start() {
        isGrounded = false;
        numCollisions = 0;
    }


    void Update() {
        if (numCollisions > 0){
            isGrounded = true;
        }
        else{
            isGrounded = false;
        }
    }


    void OnTriggerEnter(Collider other) {
        if(groundLayers.Contains(other.gameObject.layer)){
            numCollisions++;
        }
    }

    void OnTriggerExit(Collider other) {
        if(groundLayers.Contains(other.gameObject.layer)){
            numCollisions--;
        }
    }
}
