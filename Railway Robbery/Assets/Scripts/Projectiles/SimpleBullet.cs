using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBullet : MonoBehaviour
{

    [Header("Bullet Properties")]
    [SerializeField] private float speed;
    [SerializeField] private int hitDamage;
    [SerializeField] private float maxTravelDistance;
    [SerializeField] private int maxRicochetsAllowed;
    [SerializeField] private float maxRicochetSurfaceAngle;
    [SerializeField] private LayerMask collidableLayers;


    [Header("Effects")]
    [SerializeField] private AudioClip playerHitSound;
    [SerializeField] private AudioClip ricochetSound;
    [SerializeField] private AudioClip destroyedSound;
    

    // Variables
    private GameObject player;
    [SerializeField] private int currentRicochetCount = 0;
    [SerializeField] private float currentDistanceTravelled = 0;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update() {
        // Raycast forward to see if anything is between the current position and the next position
        float distancePerStep = distancePerStep = speed * Time.deltaTime;
        Vector3 translationAmount = transform.forward * distancePerStep;

        Ray forwardRay = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(forwardRay, out RaycastHit hitInfo, distancePerStep, collidableLayers)){
            
            string hitTag = hitInfo.transform.tag;
            Vector3 hitPoint = hitInfo.point;
            Vector3 hitNormal = hitInfo.normal;

            transform.position = hitPoint;

            // Determine whether the bullet is hitting the player
            if(hitTag == "Player"){
                HealthManager playerHealth = player.GetComponent<HealthManager>();

                playerHealth.DealDamage(hitDamage);
                playerHealth.DisplayHitLocation(hitPoint);

                Destroy(this.gameObject);
            }
            // Determine whether the bullet should ricochet
            else{
                float angleToSurface = 90 - Vector3.Angle(hitNormal, -transform.forward);

                if (currentRicochetCount < maxRicochetsAllowed && angleToSurface <= maxRicochetSurfaceAngle){
                    // Ricochet bullet off of surface, play ricochet sound a leave a mark
                    currentRicochetCount++;
                    Vector3 newDirection = Vector3.Reflect(transform.forward, hitNormal);

                    transform.LookAt(transform.position + newDirection);

                    Debug.Log("Ricochet");
                }
                else{
                    // Play
                    Destroy(this.gameObject);
                    Debug.Log("Bullet Destroyed");
                }
            }
        }
        else{
            // Nothing in the way, march the bullet forward along its current trajectory
            transform.Translate(translationAmount, Space.World);
        }


        // Keep track of how far the bullet has travelled
        currentDistanceTravelled += speed * Time.deltaTime;
        if(currentDistanceTravelled > maxTravelDistance){
            Destroy(this.gameObject);
            Debug.Log("Bullet Despawned");
        }
    }
}
