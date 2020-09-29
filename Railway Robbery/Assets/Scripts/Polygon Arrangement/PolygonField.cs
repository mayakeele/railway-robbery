using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonField : MonoBehaviour
{
    public float fieldWidth;
    public float fieldLength;

    public float repulsionConstant;
    public float stepSize;

    public List<Polygon> polygons = new List<Polygon>();
    //public List<Polygon> exclusionZones = new List<Polygon>();
    


    public Vector2[] pointsA;
    public Vector2 posA;

    public Vector2[] pointsB;
    public Vector2 posB;

    Polygon polyA;
    Polygon polyB;


    void Start()
    {
        polyA = new Polygon(pointsA, posA);
        polyB = new Polygon(pointsB, posB);

        polygons.Add(polyA);
        polygons.Add(polyB);

        //Debug.Log(CalculateRepulsion(polyB));
    }

    void Update()
    {
        Vector2 forceA = CalculateRepulsion(polyA);
        polyA.position += forceA * stepSize;
    }


    public Vector2 CalculateRepulsion(Polygon thisPoly){
        Vector2 netForce = Vector2.zero;

        foreach (Vector2 thisPoint in thisPoly.GetWorldPoints()){
            // Calculate net force from all other polygon points
            foreach (Polygon otherPoly in polygons){
                if (otherPoly != thisPoly){
                    foreach (Vector2 otherPoint in otherPoly.GetWorldPoints()){
                        Vector2 displacement = thisPoint - otherPoint;
                        Vector2 forceDirection = displacement.normalized;
                        float distSquared = displacement.sqrMagnitude;

                        netForce += forceDirection * (repulsionConstant/distSquared);
                    }
                }
            }

            // Calculate net force from walls

        }

        return netForce;
    }
}
