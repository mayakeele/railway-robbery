using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{
    private BodyPartReferences bodyParts;

    [SerializeField] private float maxLegLiftPercent;
    [SerializeField] private float legLiftTime;
    
    private float currHeadsetHeight;
    private float currLegLiftPercent;
    private bool areLegsMoving;


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
            if (!areLegsMoving){
                StartCoroutine(InterpolateLegLift(maxLegLiftPercent, legLiftTime));
            }
        }
        else{
            // Scale body capsule collider to match the current height of the player's headset
            if (!areLegsMoving){
                StartCoroutine(InterpolateLegLift(0, legLiftTime));
            }
        }

        bodyParts.bodyCollider.height = currHeadsetHeight * (1 - currLegLiftPercent);

        bodyParts.bodyTransform.position = new Vector3(
            bodyParts.cameraTransform.position.x, 
            bodyParts.cameraTransform.position.y - (bodyParts.bodyCollider.height / 2),
            bodyParts.cameraTransform.position.z
        );
    }

    IEnumerator InterpolateLegLift(float targetPercent, float moveTime){
        // Lerps body collider from its current percent to its final percent
        areLegsMoving = true;
        float moveRate = (targetPercent - currLegLiftPercent) / moveTime;
        
        if(moveRate > 0){
            while (currLegLiftPercent < targetPercent){
                currLegLiftPercent += moveRate * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
        else if(moveRate < 0){
            while (currLegLiftPercent > targetPercent){
                currLegLiftPercent += moveRate * Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }

        currLegLiftPercent = targetPercent;
        areLegsMoving = false;
        yield break;
    }
}
