using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class LegManager : MonoBehaviour
{
    public float displacementToChangeTarget;
    public List<FastIKFabric> legs;

    public List<Transform> defaultTargets;

    public List<Transform> currentTargets;
    public List<Transform> potentialTargets;

    void Start()
    {
        for(int i = 0; i < legs.Count; i++){
            FastIKFabric currentLeg = legs[i];

            currentTargets.Add(Instantiate(defaultTargets[i]));

            currentLeg.Target = currentTargets[i];
        }
    }

    void Update()
    {
        for(int i = 0; i < legs.Count; i++){
            FastIKFabric currentLeg = legs[i];
            Transform currentTarget = currentTargets[i];
            Transform defaultTarget = defaultTargets[i];

            float displacementFromDefault = (currentTarget.position - defaultTarget.position).magnitude;
            if(displacementFromDefault >= displacementToChangeTarget){
                currentTarget.position = defaultTarget.position;
                currentLeg.Target = currentTarget;
            }
        }
    }
}
