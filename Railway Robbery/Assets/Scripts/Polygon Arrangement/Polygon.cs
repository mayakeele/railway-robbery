using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    public struct Edge{
        public Vector2 pointA;
        public Vector2 pointB;
        public Vector2 directionAtoB;

        public Edge(Vector2 pointA, Vector2 pointB){
            this.pointA = pointA;
            this.pointB = pointB;
            this.directionAtoB = (pointB - pointA).normalized;
        }
    }


    public Vector2[] localPoints;
    public int id;
    public Vector2 position;
    public float rotation;

    public Edge[] localEdges;
    public float area;

    
    public Polygon(Vector2[] localPoints, int id, Vector2 position, float rotation){
        this.localPoints = localPoints;
        this.id = id;
        this.position = position;
        this.rotation = rotation;

        this.localEdges = CalculateLocalEdges();
        this.area = CalculateArea();
    }


    public Vector2[] CalculateRotatedPoints(){
        // Rotates the points in this polygon by its rotation and returns a new array of rotated points
        Vector2[] rotatedPoints = new Vector2[localPoints.Length];

        for(int i = 0; i < localPoints.Length; i++){
            Vector2 thisPoint = localPoints[i];
            rotatedPoints[i] = Vector2Extensions.Rotate(thisPoint, rotation);
        }

        return rotatedPoints;
    }

    public Vector2[] CalculateWorldPoints(){
        // Returns an array of points rotated and shifted by this polygon's position and rotation

        Vector2[] translatedPoints = new Vector2[localPoints.Length];
        Vector2[] rotatedPoints = CalculateRotatedPoints();

        for (int i = 0; i < localPoints.Length; i++){
            translatedPoints[i] = rotatedPoints[i] + position;
        }
        return translatedPoints;
    }

    public Edge[] CalculateLocalEdges(){
        int numEdges = localPoints.Length;
        Edge[] newEdges = new Edge[numEdges];

        for (int i = 0; i < numEdges - 1; i++){
            newEdges[i] = new Edge(localPoints[i], localPoints[i+1]);
        }
        newEdges[numEdges-1] = new Edge(localPoints[numEdges-1], localPoints[0]);

        return newEdges;
    }

    public Edge[] CalculateWorldEdges(){
        int numEdges = localPoints.Length;
        Edge[] newEdges = new Edge[numEdges];

        Vector2[] worldPoints = CalculateWorldPoints();

        for (int i = 0; i < numEdges - 1; i++){
            newEdges[i] = new Edge(worldPoints[i], worldPoints[i+1]);
        }
        newEdges[numEdges-1] = new Edge(worldPoints[numEdges-1], worldPoints[0]);

        return newEdges;
    }

    private float[] ProjectToAxis(Vector2 axis){
        // Projects each point of this polygon to a given axis, and returns [min, max] in a 2-part float array.

        Vector2[] worldPoints = CalculateWorldPoints();

        float dot = Vector2.Dot(axis, worldPoints[0]);
        float min = dot;
        float max = dot;

        for(int i = 0; i < worldPoints.Length; i++){
            dot = Vector2.Dot(axis, worldPoints[i]);

            if (dot < min){
                min = dot;
            }
            else if (dot > max){
                max = dot;
            }
        }

        return new float[] {min, max};
    }

    private float IntervalDistance(float minA, float maxA, float minB, float maxB) {
        // Calculate the distance between [minA, maxA] and [minB, maxB]. The distance will be negative if the intervals overlap

        if (minA < minB) {
            return minB - maxA;
        } else {
            return minA - maxB;
        }
    }

    public bool IsColliding(Polygon polygonB) {
        // Check if polygon A is colliding with polygon B.

        Polygon polygonA = this;

        Edge[] edgesA = polygonA.CalculateWorldEdges();
        Edge[] edgesB = polygonB.CalculateWorldEdges();

        int edgeCountA = edgesA.Length;
        int edgeCountB = edgesB.Length;

        Vector2 edge;

        // Loop through all the edges of both polygons
        for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++) {
            if (edgeIndex < edgeCountA) {
                edge = edgesA[edgeIndex].directionAtoB;
            } else {
                edge = edgesB[edgeIndex - edgeCountA].directionAtoB;
            }

            // Find the axis perpendicular to the current edge
            Vector2 axis = new Vector2(-edge.y, edge.x).normalized;

            // Find the projection of the polygon on the current axis
            float[] rangeA = polygonA.ProjectToAxis(axis);
            float[] rangeB = polygonB.ProjectToAxis(axis);

            float minA = rangeA[0]; float maxA = rangeA[1];
            float minB = rangeB[0]; float maxB = rangeB[1];

            // If the polygon projections do not intersect on this projection, they are not colliding
            if (IntervalDistance(minA, maxA, minB, maxB) > 0){
                return false;
            }
        }

        // If polygon projections intersect across ALL axes, the polygons are colliding
        return true;
    }

    public float CalculateArea(){
        // Calculate the area of this polygon, assuming it does not self-overlap

        float area = 0;
        int j = localPoints.Length - 1;

        for (int i = 0; i < localPoints.Length; i++){
            area += (localPoints[j].x + localPoints[i].x) * (localPoints[j].y - localPoints[i].y);
            j = i;
        }

        return Mathf.Abs(area / 2);
    }
}
