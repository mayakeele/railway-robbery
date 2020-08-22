using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPartPrefabs : MonoBehaviour
{
    public GameObject basePlatform;
    public GameObject wheels;

    public GameObject verticalWall;


    public GameObject CreateTrainPlatform(float length, float width, float height, float groundOffset){
        // Instantiates a universal train base prefab, scaled to the desired dimensions and raised above the ground
        GameObject parentObject = new GameObject("Universal Base");

        GameObject floorObject = Instantiate(basePlatform);
        floorObject.transform.parent = parentObject.transform;
        //GameObject wheelsObject = Instantiate(wheels);

        Mesh floorMesh = floorObject.GetComponent<MeshFilter>().mesh;
        //Mesh wheelsMesh = wheelsObject.GetComponent<Mesh>();

        Vector3[] newFloorVertices = floorMesh.vertices;

        for (int i = 0; i < newFloorVertices.Length; i++){
            Vector3 vertex = newFloorVertices[i];

            vertex.x *= width;
            vertex.y *= height;
            vertex.z *= length;

            newFloorVertices[i] = vertex;
        }
        floorMesh.vertices = newFloorVertices;
        floorMesh.RecalculateBounds();

        floorObject.GetComponent<BoxCollider>().size = new Vector3(width, height, length);

        floorObject.transform.localPosition = new Vector3(0, groundOffset, 0);

        return parentObject;
    }
}
