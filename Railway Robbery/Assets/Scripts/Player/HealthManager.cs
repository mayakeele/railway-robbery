using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth; 
    [SerializeField] private float currentHealth;

    [Header("Body Parts")]
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0){
            isDead = true;
        }
    }


    public void DealDamageRaw(int damageAmount){

        currentHealth -= damageAmount;

        Debug.Log("Current Health: " + currentHealth.ToString());
    }

    public void DealDamage(int baseDamageAmount, Transform bodyPartHit, Vector3 hitLocation){
        float damageMultiplier = 1;

        if(bodyPartHit == bodyTransform){
            damageMultiplier = 1;
        }
        else if (bodyPartHit == leftHandTransform || bodyPartHit == rightHandTransform){
            damageMultiplier = 0.5f;
        }
        else if (bodyPartHit == headTransform){
            damageMultiplier = 2.5f;
        }
        else{
            damageMultiplier = 1;
        }

        currentHealth -= (baseDamageAmount * damageMultiplier);

        Debug.Log("Current Health: " + currentHealth.ToString());
    }
}
