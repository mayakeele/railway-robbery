using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerCar : MonoBehaviour
{

    float interiorSectionLength = 3f;
    float interiorSectionWidth = 1.5f;
    float middleWalkwayLength = 1f;

    float sidePanelLength = 1.5f;
    float sidePanelThickness = 0.1f;

    float backPanelThickness = 0.1f;


    private TrainPartFactory trainPartFactory;
    void Awake() {
        trainPartFactory = GameObject.FindObjectOfType<TrainPartFactory>();
    }


    public GameObject GenerateCar(int seed, float carLength, float carWidth, float carHeight, float groundOffset){

        float halfLength = carLength / 2;
        float halfWidth = carWidth / 2;
        float halfHeight = carHeight / 2;

        float floorHeight = groundOffset + 0.05f;


        GameObject parentObject = new GameObject("Passenger Car");
        Transform parentTransform = parentObject.transform;

        // Create a base platform to build upon
        GameObject platform = trainPartFactory.CreateBasePlatform(carLength, carWidth, 0.15f, groundOffset);
        platform.transform.SetParent(parentObject.transform);

        
        // Inside seating and external walls
        int numSectionsLong = Mathf.RoundToInt(carLength / interiorSectionLength);
        for (int i = 0; i < numSectionsLong; i++) {

            // Choose an interior section variant for each side
            //GameObject leftSection = Instantiate(trainPartFactory.passengerWallLeft.ChooseVariant(), parentTransform);
            //leftSection.transform.position = new Vector3(-(halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (interiorSectionLength/2) - (i * interiorSectionLength));

            //GameObject rightSection = Instantiate(trainPartFactory.passengerWallRight.ChooseVariant(), parentTransform);
            //rightSection.transform.position = new Vector3((halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (interiorSectionLength/2) - (i * interiorSectionLength));  


            // Since each interior section is twice as long as each external wall panel, create 2 wall panels for each side
            GameObject leftWallA = Instantiate(trainPartFactory.passengerWallLeft.ChooseVariant(), parentTransform);
            leftWallA.transform.position = new Vector3(-(halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (2 * i * sidePanelLength));

            GameObject rightWallA = Instantiate(trainPartFactory.passengerWallRight.ChooseVariant(), parentTransform);
            rightWallA.transform.position = new Vector3((halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (2 * i * sidePanelLength));

            GameObject leftWallB = Instantiate(trainPartFactory.passengerWallLeft.ChooseVariant(), parentTransform);
            leftWallB.transform.position = new Vector3(-(halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (2 * i * sidePanelLength) - sidePanelLength);

            GameObject rightWallB = Instantiate(trainPartFactory.passengerWallRight.ChooseVariant(), parentTransform);
            rightWallB.transform.position = new Vector3((halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (2 * i * sidePanelLength) - sidePanelLength);
        }


        /*GameObject backWall = Instantiate(trainPartFactory.boxcarBackPanelStandard.ChooseVariant(), parentTransform);
        backWall.GetComponent<BoxcarBackPanel>().Initialize();
        backWall.transform.position = new Vector3(0, groundOffset, -(halfLength - (backPanelThickness/2)));

        GameObject frontWall = Instantiate(trainPartFactory.boxcarBackPanelStandard.ChooseVariant(), parentTransform);
        frontWall.GetComponent<BoxcarBackPanel>().Initialize();
        frontWall.transform.position = new Vector3(0, groundOffset, (halfLength - (backPanelThickness/2)));
        frontWall.transform.eulerAngles = new Vector3(0, 180, 0);
        

        // Roof
        GameObject roof = Instantiate(trainPartFactory.slantedBoxcarRoof.ChooseVariant(), parentTransform);
        roof.transform.position = new Vector3(0, groundOffset + carHeight, 0);
        
        Mesh roofMesh = roof.GetComponent<MeshFilter>().mesh;
        roofMesh.ScaleVerticesNonUniform(carWidth + 0.1f + 0.1f, 0.6f, carLength + 0.1f + 0.05f);
        roof.GetComponent<MeshCollider>().sharedMesh = roofMesh;*/


        return parentObject;
    }
}
