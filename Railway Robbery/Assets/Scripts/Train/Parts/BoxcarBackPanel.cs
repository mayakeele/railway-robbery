using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxcarBackPanel : MonoBehaviour
{

    private TrainPartFactory trainPartFactory;
    void Awake() {
        trainPartFactory = GameObject.FindObjectOfType<TrainPartFactory>();
    }

    void Update()
    {
        
    }


    public void Initialize(){
        // Always generate a tall ladder, and optionally create a scaleable ladder as well

        Vector3[] potentialLadderPositions = GetPotentialLadderPositions(3, 3, 0.1f);
        float primaryLadderHeight = 3f;
        float secondaryLadderHeight = Random.Range(0.75f, 2.5f);
        float secondaryLadderChance = 0.6f;

        bool leftIsPrimary = RandomExtensions.RandomBool();

        GameObject primaryLadder = trainPartFactory.CreateLadder(primaryLadderHeight);
        primaryLadder.transform.parent = this.transform;
        primaryLadder.transform.localPosition = leftIsPrimary ? potentialLadderPositions[0] : potentialLadderPositions[1];

        if (RandomExtensions.RandomChance(secondaryLadderChance)){
            GameObject secondaryLadder = trainPartFactory.CreateLadder(secondaryLadderHeight);
            secondaryLadder.transform.parent = this.transform;
            secondaryLadder.transform.localPosition = leftIsPrimary ? potentialLadderPositions[1] : potentialLadderPositions[0];
        }
    }


    private Vector3[] GetPotentialLadderPositions(float panelWidth, float panelHeight, float panelThickness){
        float ladderWidth = 0.5f;
        float ladderOutwardDist = (panelThickness / 2) + 0.1f;
        float inset = 0.2f;

        Vector3 left = new Vector3( -((panelWidth/2) - (ladderWidth/2) - inset) , 0, -ladderOutwardDist);
        Vector3 right = new Vector3( ((panelWidth/2) - (ladderWidth/2) - inset) , 0, -ladderOutwardDist);

        return new Vector3[] {left, right};
    }
}
