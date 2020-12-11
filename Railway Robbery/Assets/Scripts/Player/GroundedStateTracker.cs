using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedStateTracker : MonoBehaviour
{
    public bool isGrounded;
    public string[] groundLayers;

    void Start() {
        isGrounded = false;
    }

    void OnCollisionEnter(Collision other) {
        Debug.Log("enter");
        string otherLayer = LayerMask.LayerToName(other.gameObject.layer);
        foreach (string groundLayer in groundLayers){
            if (otherLayer == groundLayer){
                isGrounded = true;
            }
        }
    }

    void OnCollisionExit(Collision other) {
        Debug.Log("exit");
        string otherLayer = LayerMask.LayerToName(other.gameObject.layer);
        foreach (string groundLayer in groundLayers){
            if (otherLayer == groundLayer){
                isGrounded = false;
            }
        }
    }
}
