using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [Header("Hook Properties")]
    public float launchSpeed;
    public float maxHookableAngle;
    public int ignoreLauncherFrames;
    public LayerMask hookableLayers;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [Space]
    [SerializeField] private List<AudioClip> successfulHookSounds;
    [SerializeField] [Range(0, 1)] private float successfulHookVolume;
    [Space]
    [SerializeField] private List<AudioClip> unsuccessfulHookSounds;
    [SerializeField] [Range(0, 1)] private float unsuccessfulHookVolume;

    [Header("References")]
    public Transform buryPoint;
    public Rigidbody rb;
    [HideInInspector] public GrapplingHookLauncher attachedLauncher;


    private bool hookable = true;
    private int numFramesIgnored = 0;


    public void Launch(Vector3 parentVelocity){
        rb.velocity = (transform.forward * launchSpeed) + parentVelocity;
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

            Vector3 hitNormal = other.GetContact(0).normal;

             // Ignore collision with the launcher in the first few frames
            if(numFramesIgnored < ignoreLauncherFrames){
                numFramesIgnored++;
            
                if(other.gameObject.GetComponentInParent<GrapplingHookLauncher>() == false){
                    if(hookableLayers.Contains(other.gameObject.layer)){
                        OnHookSuccess();
                    }
                    else{
                        OnHookFail();
                    }
                }
                
            }

            // Can collide with anything afterwards
            else{
                if(hookableLayers.Contains(other.gameObject.layer)){
                    OnHookSuccess();
                }
                else{
                    OnHookFail();
                }
            }    
        }
    }


    private void OnHookSuccess(){
        if(Physics.Raycast(transform.position - rb.velocity * Time.fixedDeltaTime, transform.forward, out RaycastHit hitInfo, 0.3f, hookableLayers)){
            Vector3 buryPointOffset = buryPoint.position - transform.position;
            Vector3 hitPoint = hitInfo.point;

            transform.position = hitPoint - buryPointOffset;
        }
        
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.isKinematic = true;
        hookable = false;   

        gameObject.layer = LayerMask.NameToLayer("StaticStructure");
        gameObject.tag = "Climbable";
        attachedLauncher?.OnHookSuccess();

        audioSource.PlayClip(successfulHookSounds.RandomChoice(), successfulHookVolume);
    }

    private void OnHookFail(){
        hookable = false;

        attachedLauncher?.OnHookFail();

        audioSource.PlayClip(unsuccessfulHookSounds.RandomChoice(), unsuccessfulHookVolume);
    }


    public void Attach(GrapplingHookLauncher launcher){
        attachedLauncher = launcher;
    }

    public void Detach(){
        attachedLauncher = null;
    }
}
