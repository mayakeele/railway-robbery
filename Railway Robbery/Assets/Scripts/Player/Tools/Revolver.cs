using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : MonoBehaviour
{
    [Header("Gun Tuning")]
    [SerializeField] private int maxRoundsInChamber;
    [SerializeField] private float minTimeBetweenShots;
    [SerializeField] private float recoilPower;
    [SerializeField] private Rigidbody gunRigidbody;

    [Header("Transforms")]
    [SerializeField] private Transform barrelTip;
    [SerializeField] private Transform chamberTransform;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> shootSounds;
    [SerializeField] private List<AudioClip> clickSounds;
    [SerializeField] private List<AudioClip> chamberSpinSounds;
    
    [Header("Prefabs")]
    public GameObject bulletPrefab;
    public GameObject muzzleFlashParticleEffect;
    public GameObject shellParticleEffect;
    

    private int currentRoundsInChamber;
    private float timeSinceLastShot;

    
    void Start()
    {
        currentRoundsInChamber = maxRoundsInChamber;
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }


    public void Shoot(){
        // Shoot a bullet from the gun if the chamber is not empty and enough time has passed between shots
        if (timeSinceLastShot >= minTimeBetweenShots){

            timeSinceLastShot = 0;

            if (currentRoundsInChamber > 0){
                // Shoot a bullet
                currentRoundsInChamber -= 1;

                Instantiate(bulletPrefab, barrelTip.position, barrelTip.rotation);
                //Instantiate(muzzleFlashParticleEffect, barrelTip.position, barrelTip.rotation, barrelTip);
                audioSource.PlayOneShot(shootSounds.RandomChoice());

                //gunRigidbody.AddForce(barrelTip.transform.up * recoilPower, ForceMode.Impulse);
                gunRigidbody.AddForceAtPosition(-barrelTip.transform.forward * recoilPower, barrelTip.position, ForceMode.Impulse);
                //gunRigidbody.AddRelativeTorque(new Vector3(recoilPower, 0, 0), ForceMode.Impulse);
            }
            else{
                // No bullet in chamber, click
                audioSource.PlayOneShot(clickSounds.RandomChoice());
            }
        }
    }

    public void OpenChamber(){

    }

    public void CloseChamber(){

    }

    public void Reload(){
        currentRoundsInChamber = maxRoundsInChamber;
        timeSinceLastShot = 0;

        audioSource.PlayOneShot(chamberSpinSounds.RandomChoice());
    }
}
