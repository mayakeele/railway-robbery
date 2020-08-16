using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_cube : MonoBehaviour
{   
    public float springConstant;
    public float dampingRatio;
    public Transform target;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 force = DampedOscillation.GetDampedSpringForce(this.transform, target.transform, rb.mass, springConstant, dampingRatio);
        rb.AddForce(force);
    }
}
