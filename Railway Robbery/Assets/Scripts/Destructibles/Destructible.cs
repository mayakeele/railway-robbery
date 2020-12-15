using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class Destructible : MonoBehaviour
{

    [Header("Destruction Characteristics")]
    public bool canBreakBySpeed;
    public float destructionSpeed;
    [Space]
    public bool canBreakByProjectile;
    [Space]
    public bool canBreakByForce;
    public float destructionForce;

    [Header("Destruction Effects")]
    public GameObject destroyedBySpeedPrefab;
    public GameObject destroyedByProjectilePrefab;
    public GameObject destroyedByForcePrefab;
    public GameObject destroyedByHandsPrefab;

    [Header("Components")]
    public Collider coll;
    public Rigidbody rb;
    public Grabbable grabbable;


    void Start() {
        if(coll == null) coll = GetComponent<Collider>();
        if(rb == null) rb = GetComponent<Rigidbody>();
        if(grabbable == null) grabbable = GetComponent<Grabbable>();
    }


    private void OnCollisionEnter(Collision other) {
        if(canBreakBySpeed){
            if(rb.velocity.magnitude >= destructionSpeed){
                DestroyBySpeed();
            }
        }

        if(canBreakByForce){
            float forceDelivered = other.impulse.magnitude / Time.fixedDeltaTime;
            if(forceDelivered > destructionForce){
                DestroyByForce();
            }
        }
    }


    public void DestroyBySpeed(){
        if(destroyedBySpeedPrefab != null) Instantiate(destroyedBySpeedPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }

    public void DestroyByProjectile(){
        if(destroyedByProjectilePrefab != null) Instantiate(destroyedByProjectilePrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }

    public void DestroyByForce(){
        if(destroyedByForcePrefab != null) Instantiate(destroyedByForcePrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }

    public void DestroyByHands(){
        if(destroyedByHandsPrefab != null) Instantiate(destroyedByHandsPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}