using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshExtensions
{
    public static Mesh ScaleVerticesNonUniform(this Mesh inputMesh, float x, float y, float z){
        Vector3[] newVertices = inputMesh.vertices;

        for (int i = 0; i < newVertices.Length; i++){
            Vector3 vertex = newVertices[i];

            vertex.x *= x;
            vertex.y *= y;
            vertex.z *= z;

            newVertices[i] = vertex;
        }
        inputMesh.vertices = newVertices;

        inputMesh.RecalculateNormals();
        inputMesh.RecalculateBounds();

        return inputMesh;
    }
    public static Mesh ScaleVerticesNonUniform(this Mesh inputMesh, Vector3 dimensions){
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
