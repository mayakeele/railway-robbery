using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedStateTracker : MonoBehaviour
{
    public bool isGrounded;
    public LayerMask groundLayers;

    void Start() {
        isGrounded = false;
    }

    void OnCollisionEnter(Collision other) {
        if(groundLayers.Contains(other.gameObject.layer)){
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision other) {
        if(groundLayers.Contains(other.gameObject.layer)){
            isGrounded = false;
        }
    }
}
