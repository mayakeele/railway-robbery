using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{

    public int maxHealth;
    public int currHealth;

    public int alertnessLevel;

    public Vector3 lastSeenPlayerPosition;

    public bool isAlive;
    public bool isImmobilized;
    public bool canSeePlayer;


    public enum BehaviorState{
        Idle = 0,
        Patrolling,
        Curious,
        Alerted_Pursuit,
        Alerted_Combat,
        Attacking,
        Immobilized,
        Dead
    }

    public BehaviorState currentState;


    public Animator animator;
    public Rigidbody rb;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
