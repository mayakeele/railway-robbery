using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerCar : MonoBehaviour
{

    float interiorSectionLength = 3f;
    float interiorSectionWidth = 1.5f;
    float middleWalkwayLength = 1f;

    float floorThickness = 0.05f;

    float sidePanelLength = 1.5f;
    float sidePanelThickness = 0.1f;
    float sidePanelHeight = 3f;

    float roofHeight = 1f;

    float backPanelThickness = 0.1f;


    private TrainPartFactory trainPartFactory;
    void Awake() {
        trainPartFactory = GameObject.FindObjectOfType<TrainPartFactory>();
    }


    public GameObject GenerateCar(int seed, float carLength, float carWidth, float carHeight, float groundOffset){

        float halfLength = carLength / 2;
        float halfWidth = carWidth / 2;
        float halfHeight = carHeight / 2;

        float floorHeight = groundOffset + floorThickness;

        int numSectionsLong = Mathf.RoundToInt(carLength / interiorSectionLength) - 1;
        int numPanelsLong = Mathf.RoundToInt(carLength / sidePanelLength) - 2;


        GameObject parentObject = new GameObject("Passenger Car");
        Transform parentTransform = parentObject.transform;

        // Create a base platform to build upon
        GameObject platform = trainPartFactory.CreateBasePlatform(carLength, carWidth + (2 * sidePanelThickness), 0.15f, groundOffset);
        platform.transform.SetParent(parentObject.transform);

        
        // Inside seating and external walls
        for (int i = 0; i < numSectionsLong; i++) {
            float sectionX = halfWidth - (interiorSectionWidth/2);
            float sectionZ = halfLength - sidePanelLength - (interiorSectionLength/2) - (i * interiorSectionLength);

            float panelX = (halfWidth + (sidePanelThickness/2));
            float panelZ = halfLength - sidePanelLength - (sidePanelLength/2) - (2 * i * sidePanelLength);

            // Choose an interior section variant for each side
            GameObject leftSection = Instantiate(trainPartFactory.passengerCarInteriorLeft.ChooseVariant(), parentTransform);
            leftSection.transform.position = new Vector3(-sectionX, groundOffset, sectionZ);

            GameObject rightSection = Instantiate(trainPartFactory.passengerCarInteriorRight.ChooseVariant(), parentTransform);
            rightSection.transform.position = new Vector3(sectionX, groundOffset, sectionZ);


            // Put floor panel in the middle
            GameObject floorPanel = Instantiate(trainPartFactory.passengerCarFloor.ChooseVariant(), parentTransform);
            floorPanel.transform.position = new Vector3(0, groundOffset, sectionZ);


            // Since each interior section is twice as long as each external wall panel, create 2 wall panels for each side
            GameObject leftWallA = Instantiate(trainPartFactory.passengerCarWallLeft.ChooseVariant(), parentTransform);
            leftWallA.transform.position = new Vector3(-panelX, groundOffset, panelZ);

            GameObject rightWallA = Instantiate(trainPartFactory.passengerCarWallRight.ChooseVariant(), parentTransform);
            rightWallA.transform.position = new Vector3(panelX, groundOffset, panelZ);

            GameObject leftWallB = Instantiate(trainPartFactory.passengerCarWallLeft.ChooseVariant(), parentTransform);
            leftWallB.transform.position = new Vector3(-panelX, groundOffset, panelZ - sidePanelLength);

            GameObject rightWallB = Instantiate(trainPartFactory.passengerCarWallRight.ChooseVariant(), parentTransform);
            rightWallB.transform.position = new Vector3(panelX, groundOffset, panelZ - sidePanelLength);
        }


        // Front and back porches
        GameObject frontPorch = Instantiate(trainPartFactory.passengerCarPorchFront.ChooseVariant(), parentTransform);
        frontPorch.transform.position = new Vector3(0, groundOffset, halfLength - (sidePanelLength / 2));

        GameObject backPorch = Instantiate(trainPartFactory.passengerCarPorchBack.ChooseVariant(), parentTransform);
        backPorch.transform.position = new Vector3(0, groundOffset, -(halfLength - (sidePanelLength / 2)));
        //backPorch.transform.eulerAngles = new Vector3(0, 180, 0);
        

        // Roof
        for (int i = 1; i < numPanelsLong + 1; i++){
            GameObject roofPanel = Instantiate(trainPartFactory.passengerCarRoof.ChooseVariant(), parentTransform);

            roofPanel.transform.position = new Vector3(0, groundOffset + sidePanelHeight, halfLength - (sidePanelLength/2) - (i * sidePanelLength));
        }


        return parentObject;
    }
}
