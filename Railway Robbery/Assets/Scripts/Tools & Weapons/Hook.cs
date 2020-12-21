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

            Vector3 hitNormal = other.GetContact(0).normal;

             // Ignore collision with the launcher in the first few frames
            if(numFramesIgnored < ignoreLauncherFrames){
                numFramesIgnored++;
            
                if(other.gameObject.GetComponentInParent<GrapplingHookLauncher>() == false){
                    if(hookableLayers.Contains(other.gameObject.layer) && Vector3.Angle(hitNormal, -transform.forward) <= maxHookableAngle){
                        OnHookSuccess();
                    }
                    else{
                        OnHookFail();
                    }
                }
                
            }

            // Can collide with anything afterwards
            else{
                if(hookableLayers.Contains(other.gameObject.layer) && Vector3.Angle(hitNormal, -rb.velocity) <= maxHookableAngle){
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
