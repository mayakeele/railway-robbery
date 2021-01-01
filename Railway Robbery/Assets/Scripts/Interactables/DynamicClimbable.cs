using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicClimbable : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float defaultMass;
    [SerializeField] private string defaultLayerName = "DynamicStructure";
    [SerializeField] private string beingClimbedLayerName = "BeingClimbed";

    [Header("References")]
    public Rigidbody rb;


    void Awake() {
        if(!rb) rb = GetComponent<Rigidbody>();
        gameObject.tag = "DynamicClimbable";
    }
    void Start() {
        rb.mass = defaultMass;
    }


    public void SetAttachedMass(float attachedMass){
        // Adds to the mass of this object, simulating an attached mass without using joints
        rb.mass = defaultMass + attachedMass;
    }

    public void SetClimbingState(bool isBeingClimbed){
        if(isBeingClimbed){
            //Autohand.Hand.SetLayerRecursive(transform, LayerMask.NameToLayer(beingClimbedLayerName));
            //gameObject.layer = LayerMask.NameToLayer(beingClimbedLayerName);
        }
        else{
            //Autohand.Hand.SetLayerRecursive(transform, LayerMask.NameToLayer(defaultLayerName));
            //gameObject.layer = LayerMask.NameToLayer(defaultLayerName);
        }
    }
}
