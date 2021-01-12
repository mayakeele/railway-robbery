using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class Destructible : MonoBehaviour
{

    [Header("Destruction Characteristics")]
    public bool canBreakByImpact;
    public float destructionImpactEnergy;
    [Space]
    public bool canBreakBySpeed;
    public float destructionSpeed;
    [Space]
    public bool canBreakByCrushing;
    public float destructionCrushingForce;
    [Space]
    public bool canBreakByProjectile;


    [Header("Destruction Effects")]
    public GameObject destroyedByImpactPrefab;
    public GameObject destroyedBySpeedPrefab;
    public GameObject destroyedByCrushingPrefab;
    public GameObject destroyedByProjectilePrefab;
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

        if(canBreakByImpact && other.rigidbody){
            float energyDelivered = other.rigidbody.mass * Mathf.Pow(other.rigidbody.velocity.magnitude, 2);
            if(energyDelivered >= destructionImpactEnergy){
                DestroyByImpact();
            }
        }

        if(canBreakBySpeed){
            if(rb.velocity.magnitude >= destructionSpeed){
                DestroyBySpeed();
            }
        }

        if(canBreakByCrushing && other.rigidbody){
            float forceDelivered = other.impulse.magnitude / Time.fixedDeltaTime;
            if(forceDelivered >= destructionCrushingForce){
                DestroyByCrushing();
            }
        }
    }



    public void DestroyByImpact(){
        if(destroyedByImpactPrefab != null) Instantiate(destroyedByImpactPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
    public void DestroyBySpeed(){
        if(destroyedBySpeedPrefab != null) Instantiate(destroyedBySpeedPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
    public void DestroyByCrushing(){
        if(destroyedByCrushingPrefab != null) Instantiate(destroyedByCrushingPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
    public void DestroyByProjectile(){
        if(destroyedByProjectilePrefab != null) Instantiate(destroyedByProjectilePrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
    public void DestroyByHands(){
        if(destroyedByHandsPrefab != null) Instantiate(destroyedByHandsPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}