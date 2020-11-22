using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeManager : MonoBehaviour
{
    [Header("Object/Component References")]
    [SerializeField] private Transform eyeTransform;
    [SerializeField] private Light eyeLight;
    private NPC npc;
    private MeshRenderer eyeRenderer;

    [Header("Materials")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material suspiciousMaterial;
    [SerializeField] private Material alertedMaterial;

    [Header("Properties")]
    [SerializeField] private float defaultLightIntensity;


    void Start()
    {
        npc = GetComponent<NPC>();
        eyeRenderer = eyeTransform.GetComponent<MeshRenderer>();

        eyeRenderer.material = defaultMaterial;
        eyeLight.color = defaultMaterial.GetColor("_EmissionColor");

        eyeLight.spotAngle = npc.visionConeAngle;
        eyeLight.range = npc.visionRange;
        eyeLight.intensity = defaultLightIntensity;
    }

    void LateUpdate()
    {        
        if(npc.behaviorStateChanged){
            Material currentMaterial = defaultMaterial;

            switch(npc.currentAlertnessLevel){
                case NPC.AlertnessLevel.Unwary:
                    currentMaterial = defaultMaterial;
                break;

                case NPC.AlertnessLevel.Suspicious:
                    currentMaterial = suspiciousMaterial;
                break;

                case NPC.AlertnessLevel.Alerted:
                    currentMaterial = alertedMaterial;
                    StartCoroutine(FlashLightWave(60, 2, 0.8f));
                break;
            }

            eyeRenderer.material = currentMaterial;
            eyeLight.color = currentMaterial.GetColor("_EmissionColor");
        }    
    }


    public IEnumerator IntensifyLight(float duration, float intensityChange){
        // Multiplies light intensity for a given amount of time, then returns to normal
        eyeLight.intensity = defaultLightIntensity + intensityChange;
        yield return new WaitForSeconds(duration);
        eyeLight.intensity = defaultLightIntensity;
        yield break;
    }


    public IEnumerator FlashLightWave(float duration, float intensityChangeAmplitude, float wavelength){
        // Varies the intensity of the light as a wave function
        yield return null;

        float angularFrequency = (2 * Mathf.PI / wavelength);

        float currentTime = 0;
        while(currentTime < duration && !npc.behaviorStateChanged){
            currentTime += Time.deltaTime;

            float additionalIntensity = intensityChangeAmplitude * (-Mathf.Cos(currentTime * angularFrequency) + 1) / 2;
            eyeLight.intensity = defaultLightIntensity + additionalIntensity;

            yield return null;
        }

        eyeLight.intensity = defaultLightIntensity;

        yield break;
    }
}
