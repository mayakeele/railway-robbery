using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class Revolver : MonoBehaviour
{
    [Header("Gun Tuning")]
    [SerializeField] private int maxRoundsInChamber;
    [SerializeField] private float minTimeBetweenShots;
    [SerializeField] private float recoilPower;

    [Header("References")]
    [SerializeField] private Rigidbody gunRigidbody;
    [SerializeField] private Grabbable grabbable;

    [Header("Transforms")]
    [SerializeField] private Transform barrelTip;
    [SerializeField] private Transform chamberTransform;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [Space]
    [SerializeField] private List<AudioClip> shootSounds;
    [SerializeField] [Range(0, 1)] private float shootVolume;
    [SerializeField] private float shootPitchMin;
    [SerializeField] private float shootPitchMax;
    [Space]
    [SerializeField] private List<AudioClip> clickSounds;
    [SerializeField] [Range(0, 1)] private float clickVolume;
    [SerializeField] private float clickPitchMin;
    [SerializeField] private float clickPitchMax;
    [Space]
    [SerializeField] private List<AudioClip> chamberSpinSounds;
    [SerializeField] [Range(0, 1)] private float spinVolume;
    [SerializeField] private float spinPitchMin;
    [SerializeField] private float spinPitchMax;

    [Header("Haptics")]
    [SerializeField] [Range(0, 1)] private float hapticFrequency;
    [Space]
    [SerializeField] [Range(0, 1)] private float shootHapticAmplitude;
    [SerializeField] private float shootHapticDuration;
    [Space]
    [SerializeField] [Range(0, 1)] private float reloadHapticAmplitude;
    [SerializeField] private float reloadHapticDuration;
    [Space]
    
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
                audioSource.PlayClipPitchShifted(shootSounds.RandomChoice(), shootVolume, shootPitchMin, shootPitchMax);

                //gunRigidbody.AddForce(barrelTip.transform.up * recoilPower, ForceMode.Impulse);
                gunRigidbody.AddForceAtPosition(-barrelTip.transform.forward * recoilPower, barrelTip.position, ForceMode.Impulse);
                //gunRigidbody.AddRelativeTorque(new Vector3(recoilPower, 0, 0), ForceMode.Impulse);

                foreach(Hand hand in grabbable.heldBy){
                    hand.SetHapticsDuration(hapticFrequency, shootHapticAmplitude, shootHapticDuration);
                }
            }

            else{
                // No bullet in chamber, click
                audioSource.PlayClipPitchShifted(clickSounds.RandomChoice(), clickVolume, clickPitchMin, clickPitchMax);

                foreach(Hand hand in grabbable.heldBy){
                    hand.SetHapticsDuration(hapticFrequency, shootHapticAmplitude/2, shootHapticDuration/2);
                }
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

        audioSource.PlayClipPitchShifted(chamberSpinSounds.RandomChoice(), spinVolume, spinPitchMin, spinPitchMax);

        foreach(Hand hand in grabbable.heldBy){
            hand.SetHapticsDuration(hapticFrequency, reloadHapticAmplitude, reloadHapticDuration);
        }
    }
}
