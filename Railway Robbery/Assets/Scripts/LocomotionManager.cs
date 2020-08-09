using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    private InputHandler inputHandler;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
    }


    void FixedUpdate()
    {
        // Handle player movement with the left joystick
        Vector2 movementInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) * movementSpeed * Time.deltaTime;
        Vector3 movementVector = inputHandler.leftController.rotation * new Vector3(movementInput.x, 0, movementInput.y);
        this.transform.position += movementVector;

        float rotationInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x * rotationSpeed * Time.deltaTime;
        this.transform.RotateAround(inputHandler.cameraTransform.position, Vector3.up, rotationInput);
    }
}
