using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonFieldRenderer : MonoBehaviour
{
    [SerializeField] private PolygonField polygonField;

    [SerializeField] private GameObject planePrefab;
    

    void Awake()
    {
        GameObject plane = Instantiate(planePrefab, new Vector3(0, -0.1f, 0), Quaternion.identity);
        plane.transform.localScale = new Vector3(polygonField.fieldWidth / 10, 1, polygonField.fieldLength / 10);
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos() {
        foreach (Polygon polygon in polygonField.polygons){
            foreach (Polygon.Edge edge in polygon.CalculateWorldEdges()){
                Gizmos.color = Color.green;
                float radius = 0.08f;

                Vector3 worldA = new Vector3(edge.pointA.x, 0, edge.pointA.y);
                Vector3 worldB = new Vector3(edge.pointB.x, 0, edge.pointB.y);

                Gizmos.DrawWireSphere(worldA, radius);
                Gizmos.DrawWireSphere(worldB, radius);
                Gizmos.DrawLine(worldA, worldB);
            }
        }
    }
}
