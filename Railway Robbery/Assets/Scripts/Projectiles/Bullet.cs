using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    [SerializeField] private float speed;
    [SerializeField] private int hitDamage;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private int maxRicochetsAllowed;
    [SerializeField] private float minRicochetAngle;

    [Header("Effects")]
    [SerializeField] private AudioClip playerHitSound;
    [SerializeField] private AudioClip ricochetSound;
    [SerializeField] private AudioClip destroyedSound;

    private GameObject player;
    private Rigidbody rigidbody;
    private SphereCollider collider;

    private int currentRicochetCount = 0;
    private float currentDistanceTravelled = 0;


    void Awake() {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<SphereCollider>();
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        rigidbody.useGravity = false;
        rigidbody.isKinematic = false;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        //collider.isTrigger = true;

        rigidbody.velocity = transform.forward * speed;
    }


    void FixedUpdate() {
        currentDistanceTravelled += speed * Time.fixedDeltaTime;
        if(currentDistanceTravelled > maxTravelDistance){
            Destroy(this);
        }
    }


    private void OnCollisionEnter(Collision other) {
        // Handle collision with player or the environment, either dealing damage, ricocheting or being destroyed

        if(other.transform.tag == "Player"){
            HealthManager playerHealth = player.GetComponent<HealthManager>();

            playerHealth.DealDamage(hitDamage);
            playerHealth.DisplayHitLocation(other.GetContact(0).point);
        }
        else{
            Vector3 surfaceNormal = other.GetContact(0).normal;
            Vector3 bulletNormal = -rigidbody.velocity.normalized;

            if (currentRicochetCount < maxRicochetsAllowed && Vector3.Angle(surfaceNormal, bulletNormal) >= minRicochetAngle){
                // Ricochet bullet off of surface, play ricochet sound a leave a mark
                currentRicochetCount++;
                Vector3 newVelocity = Vector3.Reflect(rigidbody.velocity, surfaceNormal);

                rigidbody.velocity = newVelocity;
                //transform.rotation = Quaternion.FromToRotation(transform.forward, newVelocity);

                Debug.Log("Ricochet");
            }
            else{
                // Play
                Destroy(this.gameObject);
                Debug.Log("Bullet Destroyed");
            }
        }
    }
}
