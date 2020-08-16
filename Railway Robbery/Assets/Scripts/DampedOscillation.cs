using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DampedOscillation
{
    public static Vector3 GetDampedSpringForce(Transform objectTransform, Transform targetTransform, Vector3 targetVelocity, float mass, float springConstant, float dampingRatio){
        // Calculates the combined force of a spring and damping force on an object, with object velocity being relative to the target's velocity

        Rigidbody rb = objectTransform.GetComponent<Rigidbody>();

        Vector3 velocity = rb.velocity - targetVelocity;
        Vector3 displacement = objectTransform.position - targetTransform.position;

        Vector3 force = (-2 * dampingRatio * velocity * Mathf.Sqrt(mass * springConstant)) - (springConstant * displacement);

        return force;
    }

    public static Vector3 GetDampedSpringTorque(Transform objectTransform, Transform targetTransform, float momentOfInertia, float springConstant, float dampingRatio){
        // Calculates the combined torque of an angular spring and damping force on an object, with regards to the object's moment of inertia
        // All rotational vectors are in degrees
        
        Rigidbody rb = objectTransform.GetComponent<Rigidbody>();

        Vector3 angularVelocity = Mathf.Rad2Deg * rb.angularVelocity;
        Vector3 angularDisplacement = Mathf.Rad2Deg * Quaternion.ToEulerAngles(targetTransform.rotation * objectTransform.rotation);

        Vector3 dampingTorque = -2 * dampingRatio * angularVelocity * Mathf.Sqrt(momentOfInertia);
        Vector3 springTorque = angularDisplacement * (-springConstant);

        return dampingTorque + springTorque;
    }

    public static Vector3 GetUndampedSpringTorque(Transform objectTransform, Transform targetTransform, float springConstant){
        // Calculates the combined torque of an angular spring on an object, with regards to the object's moment of inertia
        // All rotational vectors are in radians
        
        Rigidbody rb = objectTransform.GetComponent<Rigidbody>();

        Vector3 torqueAxis = Vector3.Cross(objectTransform.eulerAngles, targetTransform.eulerAngles);

        //Vector3 angularVelocity = rb.angularVelocity;
        float angularDistance = Mathf.Deg2Rad * Quaternion.Angle(targetTransform.rotation, objectTransform.rotation);

        Vector3 springTorque = -springConstant * angularDistance * torqueAxis;

        return springTorque;
    }
}
