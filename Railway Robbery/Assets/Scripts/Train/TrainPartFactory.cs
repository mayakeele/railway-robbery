using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPartFactory : MonoBehaviour
{
    public GameObject basePlatform;
    public GameObject wheelSet;
    public GameObject trainConnector;

    public GameObject ladderRung;
    public GameObject ladderBars;
    public GameObject ladderConnector;

    public GameObject straightWall;
    public GameObject slantedBoxcarRoof;


    public GameObject CreateBasePlatform(float length, float width, float thickness, float groundOffset){
        // Instantiates a universal train base prefab, scaled to the desired dimensions and raised above the ground
        GameObject parentObject = new GameObject("Universal Base");
        Transform parentTransform = parentObject.transform;

        // Base platform
        GameObject floorObject = Instantiate(basePlatform);
        floorObject.transform.parent = parentTransform;

        Mesh floorMesh = floorObject.GetComponent<MeshFilter>().mesh;
        Vector3 floorDimensions = new Vector3(width, thickness, length);
        floorMesh.ScaleVerticesNonUniform(floorDimensions);
        floorObject.GetComponent<BoxCollider>().size = floorDimensions;

        floorObject.transform.localPosition = new Vector3(0, groundOffset, 0);

        // Back train connector
        GameObject connectorObject = Instantiate(trainConnector);
        connectorObject.transform.parent = parentTransform;

        Mesh connectorMesh = connectorObject.GetComponent<MeshFilter>().mesh;
        connectorMesh.ScaleVerticesNonUniform(width, thickness, 1);
        connectorObject.GetComponent<MeshCollider>().sharedMesh = connectorMesh;

        connectorObject.transform.position = new Vector3(0, groundOffset, -length / 2);

        // Front train connector
        connectorObject = Instantiate(connectorObject);
        connectorObject.transform.SetParent(parentTransform);
        connectorObject.transform.position = new Vector3(0, groundOffset, length / 2);
        connectorObject.transform.eulerAngles = new Vector3(0, 180, 0);

        // Front wheel truck
        GameObject wheelsObject = Instantiate(wheelSet);
        wheelsObject.transform.SetParent(parentTransform);

        MeshFilter[] wheelsMeshes = wheelsObject.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in wheelsMeshes){
            mf.mesh.ScaleVerticesNonUniform(width * 0.8f, groundOffset, groundOffset);
        }
        
        float truckLength = 2 * groundOffset;
        wheelsObject.transform.position = new Vector3(0, groundOffset, (length / 2) - truckLength);

        // Back wheel truck
        wheelsObject = Instantiate(wheelsObject);
        wheelsObject.transform.SetParent(parentTransform);

        wheelsObject.transform.position = new Vector3(0, groundOffset, -((length / 2) - truckLength));


        return parentObject;
    }

    public GameObject CreateStraightWall(float length, float height, float thickness, bool isFrontToBack = true){
        // Instantiates a wall prefab, scaled to the desired dimensions
        GameObject parentObject = new GameObject("Straight Wall");
        Transform parentTransform = parentObject.transform;

        GameObject wallObject = Instantiate(straightWall);
        wallObject.transform.parent = parentTransform;

        Vector3 wallDimensions = isFrontToBack ? new Vector3(thickness, height, length) : new Vector3(length, height, thickness);

        Mesh wallMesh = wallObject.GetComponent<MeshFilter>().mesh;

        wallMesh.ScaleVerticesNonUniform(wallDimensions);
        wallObject.GetComponent<BoxCollider>().size = wallDimensions;

        return parentObject;
    }


    public GameObject CreateLadder(float inputHeight, float inputRungDistance = 0.5f){
        // Instantiates ladder prefabs, scales them to required size and number of rungs
        GameObject parentObject = new GameObject("Ladder");
        Transform parentTransform = parentObject.transform;

        Ladder ladderScript = parentObject.AddComponent<Ladder>();
        ladderScript.height = inputHeight;
        ladderScript.rungDistance = inputRungDistance;

        // Side bars
        GameObject bars = Instantiate(ladderBars);
        bars.transform.parent = parentTransform;

        MeshFilter[] barMeshes = bars.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in barMeshes){
            Mesh m = mf.mesh;
            m.ScaleVerticesNonUniform(1, inputHeight, 1);

            CapsuleCollider coll = mf.gameObject.GetComponent<CapsuleCollider>();
            coll.height = inputHeight;
            coll.center = new Vector3(coll.center.x, inputHeight / 2, coll.center.z);
        }

        // Connectors
        GameObject connectors = Instantiate(ladderConnector);
        connectors.transform.parent = parentTransform;
        connectors.transform.position = new Vector3(0, inputHeight, 0);

        // Rungs
        GameObject rungs = new GameObject("Ladder Rungs");
        rungs.transform.parent = parentTransform;

        float currRungHeight = inputHeight;
        while (currRungHeight > 0){
            GameObject thisRung = Instantiate(ladderRung);
            thisRung.transform.parent = rungs.transform;

            thisRung.transform.position = new Vector3(0, currRungHeight, 0);

            currRungHeight -= inputRungDistance;
        }


        return parentObject;
    }
}
