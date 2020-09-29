using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonFieldRenderer : MonoBehaviour
{
    [SerializeField] private PolygonField polygonField;
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private GameObject planePrefab;
    

    void Start()
    {
        GameObject plane = Instantiate(planePrefab, new Vector3(0, -0.1f, 0), Quaternion.identity);
        plane.transform.localScale = new Vector3(polygonField.fieldWidth / 10, 1, polygonField.fieldLength / 10);
    }

    void Update()
    {
        List<Vector3> allPoints = new List<Vector3>();

        foreach (Polygon polygon in polygonField.polygons){
            foreach (Polygon.Edge edge in polygon.GetWorldEdges()){
                allPoints.Add(new Vector3(edge.pointA.x, 0, edge.pointA.y));
                allPoints.Add(new Vector3(edge.pointB.x, 0, edge.pointB.y));
            }
        }

        lineRenderer.positionCount = allPoints.Count;
        lineRenderer.SetPositions(allPoints.ToArray());
    }
}
