using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonField : MonoBehaviour
{
    public List<Polygon> polygons = new List<Polygon>();

    public float fieldWidth;
    public float fieldLength;


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
    }

    void Update()
    {
        polyA.position = posA;
        polyB.position = posB;

        Debug.Log(polyA.IsColliding(polyB));
    }
}
