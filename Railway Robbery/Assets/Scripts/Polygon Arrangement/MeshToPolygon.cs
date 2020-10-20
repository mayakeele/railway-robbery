using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshToPolygon
{
    public static List<Vector2> ProjectMeshToXZPlane(Mesh mesh){
        // Projects the vertices of a given mesh to the xz plane, returning a Vector2[] of these points

        List<Vector2> pointCloud = new List<Vector2>();

        foreach(Vector3 vertex in mesh.vertices){
            Vector2 projection = new Vector2(vertex.x, vertex.z);
            pointCloud.Add(projection);
        }

        return pointCloud;
    }

    public static List<Vector2> GeneratePolygonPointsFromMesh(Mesh mesh){
        List<Vector2> pointCloud = ProjectMeshToXZPlane(mesh);
        List<Vector2> polygonPoints = ConvexHull.ComputeConvexHull(pointCloud);

        return polygonPoints;
    }
}
