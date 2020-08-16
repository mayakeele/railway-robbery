using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DampedOscillation
{
    public static Vector3 GetDampedSpringForce(Transform objectTransform, Transform targetTransform, float springConstant, float dampingRatio){
        // Calculates the combined force of a spring and damping force on an object

        Rigidbody rb = objectTransform.GetComponent<Rigidbody>();

        Vector3 velocity = rb.velocity;
        Vector3 dispacement = objectTransform.position - targetTransform.position;
        float mass = rb.mass;

        Vector3 force = (-2 * dampingRatio * velocity * Mathf.Sqrt(mass * springConstant)) - (springConstant * dispacement);

        return force;
    }
}
