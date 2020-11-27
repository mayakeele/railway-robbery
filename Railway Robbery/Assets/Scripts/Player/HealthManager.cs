using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int maxHealth; 
    [SerializeField] private int currentHealth;

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


    public void DealDamage(int damageAmount){
        currentHealth -= damageAmount;
    }

    public void DisplayHitLocation(Vector3 position){
        Debug.Log("Current Health: " + currentHealth.ToString());
    }
}
