using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


        // Walls
        float sidePanelLength = 1f;
        float sidePanelThickness = 0.1f;
        float sidePanelHeight = 3f;

        int numPanelsLong = Mathf.RoundToInt(carLength / sidePanelLength);

        for (int i = 0; i < numPanelsLong; i++) {
            // Front end of car
            GameObject leftWall = Instantiate(trainPartFactory.cabooseWallLeft, parentTransform);
            leftWall.transform.position = new Vector3(-(halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));

            GameObject rightWall = Instantiate(trainPartFactory.cabooseWallRight, parentTransform);
            rightWall.transform.position = new Vector3((halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));    
        }
        

        // Cupola and roof segments
        GameObject cupola = Instantiate(trainPartFactory.cabooseCupola, parentTransform);
        cupola.transform.position = new Vector3(0, groundOffset + sidePanelHeight, 0);


        return parentObject;
    }
}
