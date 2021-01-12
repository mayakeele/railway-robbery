using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicClimbable : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maximumMassDifference = 10;
    public float attachedMass;
    private float defaultMass;

    [Header("References")]
    public Rigidbody rb;


    void Awake() {
        if(!rb) rb = GetComponent<Rigidbody>();
        gameObject.tag = "DynamicClimbable";
        defaultMass = rb.mass;
    }


    public void SetMass(float extraMass){
        // Adds to the mass of this object, simulating an attached mass without using joints
        float maxMass = defaultMass * maximumMassDifference;

        rb.mass = Mathf.Clamp(defaultMass + extraMass, defaultMass, maxMass);
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
