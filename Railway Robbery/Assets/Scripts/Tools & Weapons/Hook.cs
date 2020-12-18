using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("Hook Properties")]
    public float launchSpeed;
    public float minHookingSpeed;
    public int ignoreLauncherFrames;
    public LayerMask hookableLayers;

    [Header("References")]
    public Transform buryPoint;
    public Rigidbody rb;
    public GrapplingHookLauncher attachedLauncher;


    bool hookable = true;
    int numFramesIgnored = 0;

 
    void Start()
    {
        rb.velocity = transform.forward * launchSpeed;
    }

    void Update()
    {   
        if(hookable){
            Vector3 forwardTarget = transform.position + rb.velocity.normalized;
            transform.LookAt(forwardTarget, Vector3.up);
        }
    }


    private void OnCollisionEnter(Collision other) {    

        if(hookable){

             // Ignore collision with the launcher in the first few frames
            if(numFramesIgnored < ignoreLauncherFrames){
                numFramesIgnored++;
            
                if(other.gameObject.GetComponentInParent<GrapplingHookLauncher>() == false){
                    if(hookableLayers.Contains(other.gameObject.layer) && rb.velocity.magnitude >= minHookingSpeed){
                        OnHookSuccess();
                    }
                    else{
                        OnHookFail();
                    }
                }
                
            }

            // Can collide with anything afterwards
            else{
                if(hookableLayers.Contains(other.gameObject.layer) && rb.velocity.magnitude >= minHookingSpeed){
                    OnHookSuccess();
                }
                else{
                    OnHookFail();
                }
            }    
        }
    }


    private void OnHookSuccess(){
        rb.isKinematic = true;
        hookable = false;

        attachedLauncher?.OnHookSuccess();
    }

    private void OnHookFail(){
        hookable = false;

        attachedLauncher?.OnHookFail();
    }


    public void Attach(GrapplingHookLauncher launcher){
        attachedLauncher = launcher;
    }

    public void Detach(){
        attachedLauncher = null;
    }
}
