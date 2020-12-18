using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("Hook Properties")]
    public float launchSpeed;
    public float minHookingSpeed;
    public LayerMask hookableLayers;

    [Header("References")]
    public Transform buryPoint;
    public Rigidbody rb;
    public GrapplingHookLauncher attachedLauncher;


    bool hookable = true;

 
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * launchSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(hookable){
            Vector3 forwardTarget = transform.position + rb.velocity.normalized;
            transform.LookAt(forwardTarget, Vector3.up);
        }
    }


    private void OnCollisionEnter(Collision other) {
        if(hookable){
            if(hookableLayers.Contains(other.gameObject.layer) && rb.velocity.magnitude >= minHookingSpeed){
                OnHookSuccess();
            }
            else{
                OnHookFail();
            }
        }
    }


    private void OnHookSuccess(){
        rb.isKinematic = true;
        hookable = false;

        attachedLauncher.OnHookSuccess();
    }

    private void OnHookFail(){
        hookable = false;

        attachedLauncher.OnHookFail();
    }
}
