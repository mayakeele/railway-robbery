using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DampedOscillation
{
    public static Vector3 GetDampedSpringForce(Vector3 objectPosition, Vector3 targetPosition, Vector3 objectVelocity, Vector3 targetVelocity, float mass, float springConstant, float dampingRatio){
        // Calculates the combined force of a spring and damping force on an object, with object velocity being relative to the target's velocity

        Vector3 relativeVelocity = objectVelocity - targetVelocity;
        Vector3 displacement = objectPosition - targetPosition;

        Vector3 force = (-2 * dampingRatio * relativeVelocity * Mathf.Sqrt(mass * springConstant)) - (springConstant * displacement);

        return force;
    }

    public static Vector3 GetDampedSpringTorque(Quaternion objectRotation, Quaternion targetRotation, Vector3 angularVelocity, float momentOfInertia, float springConstant, float dampingRatio){
        // Calculates the combined torque of an angular spring and damping force on an object, with regards to the object's moment of inertia
        // All rotational vectors are in radians

        Quaternion rotation = targetRotation * Quaternion.Inverse(objectRotation);
        Vector3 torqueAxis = new Vector3(rotation.x, rotation.y, rotation.z) * rotation.w * -1;

        float angularDistance = Mathf.Deg2Rad * Quaternion.Angle(targetRotation, objectRotation);

        Vector3 dampingTorque = -2 * dampingRatio * angularVelocity * Mathf.Sqrt(momentOfInertia * springConstant);
        Vector3 springTorque = -springConstant * angularDistance * torqueAxis;

        return dampingTorque + springTorque;
    }

    public static Vector3 GetUndampedSpringTorque(Quaternion objectRotation, Quaternion targetRotation, float springConstant){
        // Calculates the combined torque of an angular spring on an object, with regards to the object's moment of inertia
        // All rotational vectors are in radians
        
        Quaternion rotation = targetRotation * Quaternion.Inverse(objectRotation);
        Vector3 torqueAxis = new Vector3(rotation.x, rotation.y, rotation.z) * rotation.w * -1;

        float angularDistance = Mathf.Deg2Rad * Quaternion.Angle(targetRotation, objectRotation);

        Vector3 springTorque = -springConstant * angularDistance * torqueAxis;

        return springTorque;
    }
}
