using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{
    private BodyPartReferences bodyParts;

    private float currHeadsetHeight;
    [SerializeField] private float legLiftPercentage;


    void Awake() {
        bodyParts = GetComponent<BodyPartReferences>();
    }


    void FixedUpdate()
    {
        bodyParts.feetTransform.position = transform.position;
        bodyParts.headCollider.transform.position = bodyParts.cameraTransform.position;

        currHeadsetHeight = bodyParts.cameraTransform.position.y - bodyParts.feetTransform.position.y; //bodyParts.cameraTransform.localPosition.y;

        if (bodyParts.leftClimbingHand.isClimbing || bodyParts.rightClimbingHand.isClimbing){
            // Scale body capsule collider to match the current height of the player's headset and the height of the player's feet

            bodyParts.bodyCollider.height = currHeadsetHeight * (1 - legLiftPercentage);

            bodyParts.bodyTransform.position = new Vector3(
                bodyParts.cameraTransform.position.x, 
                bodyParts.cameraTransform.position.y - (bodyParts.bodyCollider.height / 2),
                bodyParts.cameraTransform.position.z
            );
            
        }
        else{
            // Scale body capsule collider to match the current height of the player's headset

            bodyParts.bodyCollider.height = currHeadsetHeight;

            bodyParts.bodyTransform.position = new Vector3(
                bodyParts.cameraTransform.position.x, 
                bodyParts.cameraTransform.position.y - (bodyParts.bodyCollider.height / 2),
                bodyParts.cameraTransform.position.z
            );
        }  
    }
}
