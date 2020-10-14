﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPartFactory : MonoBehaviour
{

    public PartVariantGroup basePlatform;
    public PartVariantGroup wheelSet;
    public PartVariantGroup trainConnector;

    public PartVariantGroup ladderRung;
    public PartVariantGroup ladderBars;
    public PartVariantGroup ladderConnector;

    public PartVariantGroup slantedBoxcarRoof;
    public PartVariantGroup boxcarDoorHandle;
    public PartVariantGroup boxcarSidePanelLB;
    public PartVariantGroup boxcarSidePanelLF;
    public PartVariantGroup boxcarSidePanelRB;
    public PartVariantGroup boxcarSidePanelRF;
    public PartVariantGroup boxcarBackPanelStandard;
    public PartVariantGroup boxcarSlidingDoorLeft;
    public PartVariantGroup boxcarSlidingDoorRight;

    public PartVariantGroup cabooseWallLeft;
    public PartVariantGroup cabooseWallRight;
    public PartVariantGroup cabooseRoof;
    public PartVariantGroup cabooseCupola;
    public PartVariantGroup caboosePorchFront;
    public PartVariantGroup cabooseDoorwayFront;

    public PartVariantGroup passengerCarWallLeft;
    public PartVariantGroup passengerCarWallRight;
    public PartVariantGroup passengerCarRoof;
    public PartVariantGroup passengerCarInteriorLeft;
    public PartVariantGroup passengerCarInteriorRight;
    public PartVariantGroup passengerCarWalkway;



    public GameObject CreateBasePlatform(float length, float width, float thickness, float groundOffset){
        // Instantiates a universal train base prefab, scaled to the desired dimensions and raised above the ground
        GameObject parentObject = new GameObject("Universal Base");
        Transform parentTransform = parentObject.transform;

        // Base platform
        GameObject floorObject = Instantiate(basePlatform.ChooseVariant(), parentTransform);
        //floorObject.transform.parent = parentTransform;

        Mesh floorMesh = floorObject.GetComponent<MeshFilter>().mesh;
        Vector3 floorDimensions = new Vector3(width, thickness, length);
        floorMesh.ScaleVerticesNonUniform(floorDimensions);
        floorObject.GetComponent<BoxCollider>().size = floorDimensions;

        floorObject.transform.localPosition = new Vector3(0, groundOffset - (floorDimensions.y/2), 0);

        // Back train connector
        GameObject connectorObject = Instantiate(trainConnector.ChooseVariant(), parentTransform);

        Mesh connectorMesh = connectorObject.GetComponent<MeshFilter>().mesh;
        connectorMesh.ScaleVerticesNonUniform(width, thickness, 1);
        connectorObject.GetComponent<MeshCollider>().sharedMesh = connectorMesh;

        connectorObject.transform.position = new Vector3(0, groundOffset - (floorDimensions.y/2), -length / 2);

        // Front train connector
        connectorObject = Instantiate(connectorObject);
        connectorObject.transform.SetParent(parentTransform);
        connectorObject.transform.position = new Vector3(0, groundOffset - (floorDimensions.y/2), length / 2);
        connectorObject.transform.eulerAngles = new Vector3(0, 180, 0);

        // Front wheel truck
        GameObject wheelsObject = Instantiate(wheelSet.ChooseVariant(), parentTransform);

        MeshFilter[] wheelsMeshes = wheelsObject.GetComponentsInChildren<MeshFilter>();
        float wheelsHeight = groundOffset - floorDimensions.y;
        foreach (MeshFilter mf in wheelsMeshes){
            mf.mesh.ScaleVerticesNonUniform(width * 0.8f, wheelsHeight, wheelsHeight);
        }
        
        float truckLength = 2 * wheelsHeight;
        wheelsObject.transform.position = new Vector3(0, wheelsHeight, (length / 2) - truckLength);

        // Back wheel truck
        wheelsObject = Instantiate(wheelsObject);
        wheelsObject.transform.SetParent(parentTransform);

        wheelsObject.transform.position = new Vector3(0, wheelsHeight, -((length / 2) - truckLength));


        return parentObject;
    }

    /*public GameObject CreateStraightWall(float length, float height, float thickness, bool isFrontToBack = true){
        // Instantiates a wall prefab, scaled to the desired dimensions
        GameObject parentObject = new GameObject("Straight Wall");
        Transform parentTransform = parentObject.transform;

        GameObject wallObject = Instantiate(straightWall.ChooseVariant(), parentTransform);

        Vector3 wallDimensions = isFrontToBack ? new Vector3(thickness, height, length) : new Vector3(length, height, thickness);

        Mesh wallMesh = wallObject.GetComponent<MeshFilter>().mesh;

        wallMesh.ScaleVerticesNonUniform(wallDimensions);
        wallObject.GetComponent<BoxCollider>().size = wallDimensions;

        return parentObject;
    }*/


}
