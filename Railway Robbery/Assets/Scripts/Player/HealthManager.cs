using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    private BodyPartReferences bodyParts;

    [Header("Health")]
    [SerializeField] private float maxHealth; 
    [SerializeField] private float currentHealth;

    private bool isDead = false;

    void Awake() {
        bodyParts = GetComponent<BodyPartReferences>();
    }
    
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

        if(bodyPartHit == bodyParts.bodyCollider.transform){
            damageMultiplier = 1;
        }
        else if (bodyPartHit == bodyParts.leftHandTransform || bodyPartHit == bodyParts.rightHandTransform){
            damageMultiplier = 0.5f;
        }
        else if (bodyPartHit == bodyParts.headCollider.transform){
            damageMultiplier = 2.5f;
        }
        else{
            damageMultiplier = 1;
        }

        currentHealth -= (baseDamageAmount * damageMultiplier);

        Debug.Log("Current Health: " + currentHealth.ToString());
    }
}
