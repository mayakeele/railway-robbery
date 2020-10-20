using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoGenerator : MonoBehaviour
{
    private PartVariantGroup cargoPrefabs;
    private PolygonField polygonField;

    private List<GameObject> initialCargo = new List<GameObject>();

    private float width, length;
    private float fillPercent;
 

    public GameObject GenerateCargoRoom(float roomWidth, float roomLength, float roomFillPercent){
        cargoPrefabs = GameObject.FindGameObjectWithTag("CargoPrefabContainer").GetComponent<PartVariantGroup>();

        this.width = roomWidth;
        this.length = roomLength;
        this.fillPercent = roomFillPercent;

        polygonField = new PolygonField(width, length);

        //PolygonFieldRenderer fieldRenderer = gameObject.AddComponent<PolygonFieldRenderer>();
        //fieldRenderer.polygonField = this.polygonField;

        InitializeCargo();

        while (polygonField.isSimulationFinished == false){
            polygonField.RunSimulationStepCollisionOnly();
        }


        // Instantiate cargo prefabs to match final polygon configuration
        GameObject cargoParent = new GameObject("Cargo Parent");
        foreach (Polygon polygon in polygonField.polygons){
            GameObject prefab = initialCargo[polygon.id];

            Vector3 cargoPosition = new Vector3(polygon.position.x, 0, polygon.position.y);
            Quaternion cargoRotation = Quaternion.Euler(0, -polygon.rotation * Mathf.Rad2Deg, 0);

            GameObject thisCargo = Instantiate(prefab, cargoPosition, cargoRotation, cargoParent.transform);
        }

        return cargoParent;
    }

    private void InitializeCargo(){
        float roomArea = length * width;

        int currID = 0;
        float totalCargoArea = 0;
        while (totalCargoArea < roomArea * fillPercent){

            Vector2 position = new Vector2(Random.Range(-width/2, width/2), Random.Range(-length/2, length/2));
            float rotation = Random.Range(0f, 2 * Mathf.PI);

            GameObject cargoObject = cargoPrefabs.ChooseVariant();

            List<Vector2> polygonPoints = MeshToPolygon.GeneratePolygonPointsFromMesh(cargoObject.GetComponent<MeshFilter>().sharedMesh);
            Polygon cargoPolygon = new Polygon(polygonPoints.ToArray(), currID, position, rotation);

            initialCargo.Add(cargoObject);
            polygonField.polygons.Add(cargoPolygon);

            currID++;
            totalCargoArea += cargoPolygon.area;
        }

    }
}
