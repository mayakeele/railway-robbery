using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private Hand autoHand;
    void Start()
    {
        playerRigidbody = playerTransform.GetComponent<Rigidbody>();
        autoHand = GetComponent<Hand>();
    }

    void FixedUpdate()
    {
        Vector3 translationAmount = playerRigidbody.velocity * Time.fixedDeltaTime;
        transform.Translate(translationAmount, Space.World);
    }
}
