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
        float sidePanelThickness = 0.1f;

        int numPanelsLong = Mathf.RoundToInt(carLength / sidePanelLength);
        int doorSlot = numPanelsLong % 2 == 0 ? (numPanelsLong / 2) - 1 : (numPanelsLong / 2);

        for (int i = 0; i < numPanelsLong; i++) {

            if (i == doorSlot){
                GameObject leftDoor = Instantiate(trainPartFactory.boxcarSlidingDoorLeft.ChooseVariant(), parentTransform);
                leftDoor.transform.position = new Vector3(-(halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));

                GameObject rightDoor = Instantiate(trainPartFactory.boxcarSlidingDoorRight.ChooseVariant(), parentTransform);
                rightDoor.transform.position = new Vector3((halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));
            }
            else if (i == doorSlot + 1){
                GameObject leftWall = Instantiate(trainPartFactory.boxcarSidePanelLB.variants[0], parentTransform);
                leftWall.transform.position = new Vector3(-(halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));

                GameObject rightWall = Instantiate(trainPartFactory.boxcarSidePanelRB.variants[0], parentTransform);
                rightWall.transform.position = new Vector3((halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));
            }
            else if (i < doorSlot){
                // Front end of car
                GameObject leftWall = Instantiate(trainPartFactory.boxcarSidePanelLF.ChooseVariant(), parentTransform);
                leftWall.transform.position = new Vector3(-(halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));

                GameObject rightWall = Instantiate(trainPartFactory.boxcarSidePanelRF.ChooseVariant(), parentTransform);
                rightWall.transform.position = new Vector3((halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));
            }
            else {
                // Back end of car
                GameObject leftWall = Instantiate(trainPartFactory.boxcarSidePanelLB.ChooseVariant(), parentTransform);
                leftWall.transform.position = new Vector3(-(halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));

                GameObject rightWall = Instantiate(trainPartFactory.boxcarSidePanelRB.ChooseVariant(), parentTransform);
                rightWall.transform.position = new Vector3((halfWidth - (sidePanelThickness/2)), groundOffset, halfLength - (sidePanelLength/2) - (i * sidePanelLength));
            }

        }

        float backPanelThickness = 0.1f;

        GameObject backWall = Instantiate(trainPartFactory.boxcarBackPanelStandard.ChooseVariant(), parentTransform);
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
        roof.GetComponent<MeshCollider>().sharedMesh = roofMesh;


        // Generate cargo on the deck of this car
        CargoGenerator cargoGenerator = gameObject.AddComponent<CargoGenerator>();

        GameObject cargo = cargoGenerator.GenerateCargoRoom(carWidth - (2*sidePanelThickness), carLength - (2*backPanelThickness));

        cargo.transform.parent = parentTransform;
        cargo.transform.position = new Vector3(0, groundOffset, 0);


        return parentObject;
    }
}
