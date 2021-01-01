using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_cube : MonoBehaviour
{   
    public float springFrequency;
    public float dampingRatio;

    public float angularSpringFrequency;
    public float angularDampingRatio;
    public Transform target;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.maxAngularVelocity = 30;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 accel = DampedSpring.GetDampedSpringAcceleration(
            this.transform.position, 
            target.transform.position,
            rb.velocity, 
            springFrequency,
            dampingRatio);
        rb.AddForce(accel, ForceMode.Acceleration);

        Vector3 angularAccel = DampedSpring.GetDampedSpringAngularAcceleration(
            this.transform.rotation, 
            target.transform.rotation,
            rb.angularVelocity, 
            angularSpringFrequency, 
            angularDampingRatio);
        rb.AddTorque(angularAccel, ForceMode.Acceleration);
    }
}
