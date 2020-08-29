using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCar : MonoBehaviour
{
    private TrainPartFactory trainPartFactory;
    void Awake() {
        trainPartFactory = GameObject.FindObjectOfType<TrainPartFactory>();
    }


    public GameObject GenerateCar(int seed, float carLength, float carWidth, float carHeight, float groundOffset){

        float halfLength = carLength / 2;
        float halfWidth = carWidth / 2;
        float halfHeight = carHeight / 2;

        GameObject parentObject = new GameObject("Boxcar");
        Transform parentTransform = parentObject.transform;

        // Create a base platform to build upon
        GameObject platform = trainPartFactory.CreateBasePlatform(carLength, carWidth, 0.15f, groundOffset);
        platform.transform.SetParent(parentObject.transform);

        
        // Walls
        float sidePanelLength = 1.5f;
        int numPanelsLong = Mathf.RoundToInt(carLength / sidePanelLength);
        int doorSlot = numPanelsLong % 2 == 0 ? (numPanelsLong / 2) - 1 : (numPanelsLong / 2);
        Debug.Log(doorSlot);
        for (int i = 0; i < numPanelsLong; i++) {
            if (i == doorSlot){
                GameObject leftDoor = Instantiate(trainPartFactory.boxcarSlidingDoorLeft);
                leftDoor.transform.SetParent(parentObject.transform);
                leftDoor.transform.position = new Vector3(-halfWidth, groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));

                GameObject rightDoor = Instantiate(trainPartFactory.boxcarSlidingDoorRight);
                rightDoor.transform.SetParent(parentObject.transform);
                rightDoor.transform.position = new Vector3(halfWidth, groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));
            }
            else{
                GameObject leftWall = Instantiate(trainPartFactory.boxcarSidePanelStandard);
                leftWall.transform.SetParent(parentObject.transform);
                leftWall.transform.position = new Vector3(-halfWidth, groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));

                GameObject rightWall = Instantiate(trainPartFactory.boxcarSidePanelStandard);
                rightWall.transform.SetParent(parentObject.transform);
                rightWall.transform.position = new Vector3(halfWidth, groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));
            }
            
                

            
        }

        GameObject backWall = Instantiate(trainPartFactory.boxcarBackPanelStandard);
        backWall.transform.SetParent(parentObject.transform);
        backWall.GetComponent<BoxcarBackPanel>().Initialize();
        backWall.transform.position = new Vector3(0, groundOffset, -halfLength);

        GameObject frontWall = Instantiate(trainPartFactory.boxcarBackPanelStandard);
        frontWall.transform.SetParent(parentObject.transform);
        frontWall.GetComponent<BoxcarBackPanel>().Initialize();
        frontWall.transform.position = new Vector3(0, groundOffset, halfLength);
        frontWall.transform.eulerAngles = new Vector3(0, 180, 0);
        

        // Roof
        GameObject roof = Instantiate(trainPartFactory.slantedBoxcarRoof);
        roof.transform.SetParent(parentTransform);
        roof.transform.position = new Vector3(0, groundOffset + carHeight, 0);
        
        Mesh roofMesh = roof.GetComponent<MeshFilter>().mesh;
        roofMesh.ScaleVerticesNonUniform(carWidth + 0.1f + 0.1f, 0.6f, carLength + 0.1f + 0.05f);
        roof.GetComponent<MeshCollider>().sharedMesh = roofMesh;


        // Place ladder(s) in random precalculated spots
        float sideInset = 0.6f;
        float distanceOut = 0.1f + 0.05f;

        Vector3[] possibleLadderPositions = new Vector3[] {
            // Back
            //new Vector3(halfWidth - sideInset, groundOffset, -halfLength - distanceOut),
            //new Vector3(-halfWidth + sideInset, groundOffset, -halfLength - distanceOut),

            // Left
            new Vector3(-halfWidth - distanceOut, groundOffset, -halfLength + sideInset),
            new Vector3(-halfWidth - distanceOut, groundOffset, halfLength - sideInset),

            // Front
            //new Vector3(-halfWidth + sideInset, groundOffset, halfLength + distanceOut),
            //new Vector3(halfWidth - sideInset, groundOffset, halfLength + distanceOut),

            // Right
            new Vector3(halfWidth + distanceOut, groundOffset, halfLength - sideInset),
            new Vector3(halfWidth + distanceOut, groundOffset, -halfLength + sideInset)
            
        };
        int[] possibleLadderYRotations = new int[] {
            //0,
            //0,

            90,
            90,

            //180,
            //180,
            
            270,
            270
        };

        int[] ladderPosIndices = RandomExtensions.ChooseRandomIndices(4, possibleLadderPositions.Length, false);
        for (int i = 0; i < ladderPosIndices.Length; i++){
            int index = ladderPosIndices[i];

            Vector3 ladderPos = possibleLadderPositions[index];
            float ladderYRot = possibleLadderYRotations[index];

            float ladderHeight = Random.Range(1.5f, carHeight);

            GameObject ladderObject = trainPartFactory.CreateLadder(ladderHeight);
            ladderObject.transform.parent = parentTransform;

            ladderObject.transform.position = ladderPos;
            ladderObject.transform.eulerAngles = new Vector3(0, ladderYRot, 0);
        }


        return parentObject;
    }
}
