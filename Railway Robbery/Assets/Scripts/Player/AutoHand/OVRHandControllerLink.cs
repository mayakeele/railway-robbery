using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

namespace Autohand{
    public class OVRHandControllerLink : MonoBehaviour{

        public InputHandler inputHandler;

        public Hand hand;
        public OVRInput.Controller controller;
        public OVRInput.Axis1D grabAxis;

        public OVRInput.Button grabButton;
        public OVRInput.Button squeezeButton;
        public OVRInput.Button action1Button;
        public OVRInput.Button action2Button;

        public void Update() {

            if(OVRInput.GetDown(grabButton, controller)) {
                Debug.Log("Grab");
                hand.Grab();
                //hand.gripOffset += 1;
            }
            if(OVRInput.GetUp(grabButton, controller)) {
                Debug.Log("Release");
                hand.Release();
                //hand.gripOffset -= 1;
            }
            if(OVRInput.GetDown(squeezeButton, controller)) {
                hand.Squeeze();
            }
            if(OVRInput.GetUp(squeezeButton, controller)) {
                hand.Unsqueeze();
            }

            if(OVRInput.GetDown(action1Button, controller)) {
                
            }

            if(hand.disableIK == false){
                hand.SetGrip(OVRInput.Get(grabAxis, controller));
            }
            
        }

        public float GetAxis(OVRInput.Axis1D axis) {
            return OVRInput.Get(axis, controller);
        }

        public Vector2 GetAxis2D(OVRInput.Axis2D axis) {
            return OVRInput.Get(axis, controller);
        }
        
        public bool ButtonPressed(OVRInput.Button button) {
            return OVRInput.Get(button, controller);
        }
        
        public bool ButtonPressed(OVRInput.Touch button) {
            return OVRInput.Get(button, controller);
        }
        
        public bool ButtonTouched(OVRInput.Touch button) {
            return OVRInput.Get(button, controller);
        }
    }
}
