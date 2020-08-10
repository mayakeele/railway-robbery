using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float playerHeight;
    [HideInInspector] public float currHeadsetHeight;

    public Transform cameraTransform;
    public Transform leftController;
    public Transform leftControllerAnchor;
    public Transform rightController;
    public Transform rightControllerAnchor;

    [SerializeField] private float gripThreshold = 0.5f;
    [SerializeField] private float triggerThreshold = 0.5f;

    [HideInInspector] public Rigidbody rb;
    public Transform bodyGameobject;
    private CapsuleCollider bodyCollider;
    //[SerializeField] private MeshFilter meshFilter;

    // A flag enumeration storing booleans for the state of controller inputs (those that can be represented as a boolean, including triggers once a threshold is passed)
    [Flags]
    public enum InputButton {
        L_ButtonOne,
        R_ButtonOne,
        L_ButtonTwo,
        R_ButtonTwo,
        L_Grip,
        R_Grip,
        L_Trigger,
        R_Trigger,
        L_StickPress,
        R_StickPress,
        L_Start
    }

    private bool[] inputHeld = new bool[Enum.GetValues(typeof(InputButton)).Length];


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bodyCollider = bodyGameobject.GetComponent<CapsuleCollider>();
        //meshFilter = GetComponent<MeshFilter>();
    }

    void FixedUpdate()
    {
        OVRInput.FixedUpdate();

        // Update inputHeld flags with current input states
        inputHeld[(int)InputButton.L_ButtonOne] = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch);
        inputHeld[(int)InputButton.R_ButtonOne] = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);

        inputHeld[(int)InputButton.L_ButtonTwo] = OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch);
        inputHeld[(int)InputButton.R_ButtonTwo] = OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch);

        inputHeld[(int)InputButton.L_Grip] = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) > gripThreshold ? true: false;
        inputHeld[(int)InputButton.R_Grip] = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) > gripThreshold ? true: false;

        inputHeld[(int)InputButton.L_Trigger] = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) > triggerThreshold ? true: false;
        inputHeld[(int)InputButton.R_Trigger] = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > triggerThreshold ? true: false;

        inputHeld[(int)InputButton.L_StickPress] = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch);
        inputHeld[(int)InputButton.R_StickPress] = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch);

        inputHeld[(int)InputButton.L_Start] = OVRInput.Get(OVRInput.Button.Start, OVRInput.Controller.LTouch);


        // Body should follow the camera's position and height
        currHeadsetHeight = cameraTransform.localPosition.y;

        bodyGameobject.transform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y - (currHeadsetHeight / 2), cameraTransform.position.z);
        bodyCollider.height = currHeadsetHeight;
    }


    public bool GetHeldState(InputButton inputButton){
        int index = (int)inputButton;
        return inputHeld[index];
    }
}
