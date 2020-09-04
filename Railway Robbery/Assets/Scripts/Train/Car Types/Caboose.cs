using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Caboose : MonoBehaviour
{
    private TrainPartFactory trainPartFactory;
    void Awake() {
        trainPartFactory = GameObject.FindGameObjectWithTag("TrainPartPrefabsContainer").GetComponent<TrainPartFactory>();
    }


    public GameObject GenerateCar(int seed, float carLength, float carWidth, float carHeight, float groundOffset){
        
        float halfLength = carLength / 2;
        float halfWidth = carWidth / 2;
        float halfHeight = carHeight / 2;

        GameObject parentObject = new GameObject("Caboose");
        Transform parentTransform = parentObject.transform;


        // Create a base platform to build upon
        GameObject platform = trainPartFactory.CreateBasePlatform(carLength, carWidth, 0.15f, groundOffset);
        platform.transform.SetParent(parentObject.transform);


        // Side Walls
        float sidePanelLength = 1f;
        float sidePanelThickness = 0.1f;
        float sidePanelHeight = 3f;

        int numPanelsLong = Mathf.RoundToInt(carLength / sidePanelLength);

        for (int i = 1; i < numPanelsLong - 1; i++) {

            // Walls if anywhere else
            GameObject leftWall = Instantiate(trainPartFactory.cabooseWallLeft, parentTransform);
            leftWall.transform.position = new Vector3(-(halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));

            GameObject rightWall = Instantiate(trainPartFactory.cabooseWallRight, parentTransform);
            rightWall.transform.position = new Vector3((halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));    
        }
        

        // Front and back doorways and porches
        float doorwayThickness = 0.1f;

        GameObject frontDoorway = Instantiate(trainPartFactory.cabooseDoorwayFront, parentTransform);
        frontDoorway.transform.position = new Vector3(0, groundOffset, halfLength - sidePanelLength - (doorwayThickness / 2));

        GameObject backDoorway = Instantiate(trainPartFactory.cabooseDoorwayFront, parentTransform);
        backDoorway.transform.eulerAngles = new Vector3(0, 180, 0);
        backDoorway.transform.position = new Vector3(0, groundOffset, -(halfLength - sidePanelLength - (doorwayThickness / 2)));


        GameObject frontPorch = Instantiate(trainPartFactory.caboosePorchFront, parentTransform);
        frontPorch.transform.position = new Vector3(0, groundOffset, halfLength - (sidePanelLength/2));

        GameObject backPorch = Instantiate(trainPartFactory.caboosePorchFront, parentTransform);
        backPorch.transform.eulerAngles = new Vector3(0, 180, 0);
        backPorch.transform.position = new Vector3(0, groundOffset, -(halfLength - (sidePanelLength/2)));


        // Cupola and roof segments
        GameObject cupola = Instantiate(trainPartFactory.cabooseCupola, parentTransform);
        cupola.transform.position = new Vector3(0, groundOffset + sidePanelHeight, 0);

        int middleIndex = (numPanelsLong-1) / 2;
        int[] cupolaRoofIndicesOccupied = { middleIndex - 1, middleIndex, middleIndex + 1 };

        for (int i = 0; i < numPanelsLong; i++){
            // Only place a roof if the cupola does not occupy this space
            if (System.Array.IndexOf(cupolaRoofIndicesOccupied, i) == -1){
                GameObject roofPanel = Instantiate(trainPartFactory.cabooseRoof, parentTransform);

                roofPanel.transform.position = new Vector3(0, groundOffset + sidePanelHeight, halfLength - (sidePanelLength/2) - (i * sidePanelLength));
            }
        }


        return parentObject;
    }
}
