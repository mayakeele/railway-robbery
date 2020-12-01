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


    [Header("References")]
    [SerializeField] private GameObject model;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AudioSource audioSource;


    [Header("Sound Effects")]
    [SerializeField] private AudioClip targetHitSound;
    [SerializeField] private AudioClip ricochetSound;
    [SerializeField] private AudioClip destroyedSound;
    

    [Header("Particle Effects")]
    //[SerializeField] private ParticleSystem particleSystem;


    // Variables
    private bool isActive = true;
    private GameObject player;
    private float trailEndTime;
    private int currentRicochetCount = 0;
    private float currentDistanceTravelled = 0;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        trailEndTime = trailRenderer.time;
    }


    void Update() {

        if(isActive){
            // Raycast forward to see if anything is between the current position and the next position

            float distancePerStep = distancePerStep = speed * Time.deltaTime;
            Vector3 translationAmount = transform.forward * distancePerStep;

            Ray forwardRay = new Ray(transform.position, transform.forward);

            if(Physics.Raycast(forwardRay, out RaycastHit hitInfo, distancePerStep, collidableLayers)){
                
                string hitTag = hitInfo.transform.tag;
                Transform hitTransform = hitInfo.transform;
                Vector3 hitPoint = hitInfo.point;
                Vector3 hitNormal = hitInfo.normal;

                transform.position = hitPoint;

                // Determine whether the bullet is hitting the player or an enemy
                if(hitTag == "Player"){
                    HealthManager playerHealth = player.GetComponent<HealthManager>();

                    playerHealth.DealDamage(hitDamage, hitTransform, hitPoint);

                    DestroyBullet(targetHitSound);
                }
                else if(hitTag == "Enemy"){
                    NPC hitNPC = GetComponentInParent<NPC>();

                    hitNPC.DealDamage(hitDamage, hitTransform, hitPoint);

                    DestroyBullet(targetHitSound);
                }
                // Determine whether the bullet should ricochet
                else{
                    float angleToSurface = 90 - Vector3.Angle(hitNormal, -transform.forward);

                    if (currentRicochetCount < maxRicochetsAllowed && angleToSurface <= maxRicochetSurfaceAngle){
                        // Ricochet bullet off of surface, play ricochet sound a leave a mark
                        currentRicochetCount++;
                        Vector3 newDirection = Vector3.Reflect(transform.forward, hitNormal);

                        transform.LookAt(transform.position + newDirection);

                        audioSource.clip = ricochetSound;
                        audioSource.Play();
                    }
                    else{
                        // Play bullet destruction animation
                        DestroyBullet(destroyedSound);
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
                DestroyBullet();
            }
        }
        
    }


    void DestroyBullet(AudioClip soundToPlay = null){
        // Stop rendering the bullet, leave the bullet trail intact until it runs out, and then destroy bullet
        isActive = false;
        model.SetActive(false);

        float soundLength = 0;
        if(soundToPlay != null){
            soundLength = soundToPlay.length;
            audioSource.PlayOneShot(soundToPlay);
        } 

        float timeUntilDestroyed = Mathf.Max(trailEndTime, soundLength);
        StartCoroutine(DestroyDelayed(timeUntilDestroyed));
    }

    IEnumerator DestroyDelayed(float delay){
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}
