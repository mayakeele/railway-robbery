using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainPartPrefabs : MonoBehaviour
{
    public GameObject basePlatform;
    public GameObject wheels;

    public GameObject straightWall;


    public GameObject CreateBasePlatform(float length, float width, float thickness, float groundOffset){
        // Instantiates a universal train base prefab, scaled to the desired dimensions and raised above the ground
        GameObject parentObject = new GameObject("Universal Base");
        Transform parentTransform = parentObject.transform;

        GameObject floorObject = Instantiate(basePlatform);
        floorObject.transform.parent = parentTransform;
        //GameObject wheelsObject = Instantiate(wheels);

        Mesh floorMesh = floorObject.GetComponent<MeshFilter>().mesh;
        //Mesh wheelsMesh = wheelsObject.GetComponent<Mesh>();

        Vector3 floorDimensions = new Vector3(width, thickness, length);

        floorMesh = ScaleMesh(floorMesh, floorDimensions);
        floorObject.GetComponent<BoxCollider>().size = floorDimensions;

        floorObject.transform.localPosition = new Vector3(0, groundOffset, 0);

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

        wallMesh = ScaleMesh(wallMesh, wallDimensions);
        wallObject.GetComponent<BoxCollider>().size = wallDimensions;

        return parentObject;
    }


    public Mesh ScaleMesh(Mesh inputMesh, float x, float y, float z){
        Vector3[] newVertices = inputMesh.vertices;

        for (int i = 0; i < newVertices.Length; i++){
            Vector3 vertex = newVertices[i];

            vertex.x *= x;
            vertex.y *= y;
            vertex.z *= z;

            newVertices[i] = vertex;
        }
        inputMesh.vertices = newVertices;
        inputMesh.RecalculateBounds();

        return inputMesh;
    }
    public Mesh ScaleMesh(Mesh inputMesh, Vector3 dimensions){
        Vector3[] newVertices = inputMesh.vertices;

        for (int i = 0; i < newVertices.Length; i++){
            Vector3 vertex = newVertices[i];

            vertex.Scale(dimensions);

            newVertices[i] = vertex;
        }
        inputMesh.vertices = newVertices;
        inputMesh.RecalculateBounds();

        return inputMesh;
    }
}
