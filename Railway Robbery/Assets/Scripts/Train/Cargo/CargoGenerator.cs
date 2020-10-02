using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoGenerator : MonoBehaviour
{
    private PartVariantGroup cargoPrefabs;
    private PolygonField polygonField;

    private List<GameObject> initialCargo = new List<GameObject>();

    private float width, length;
 

    public GameObject GenerateCargoRoom(float roomWidth, float roomLength){
        cargoPrefabs = GameObject.FindGameObjectWithTag("CargoPrefabContainer").GetComponent<PartVariantGroup>();

        this.width = roomWidth;
        this.length = roomLength;

        polygonField = new PolygonField(width, length);

        InitializeCargo();

        while (polygonField.isSimulationFinished == false){
            polygonField.RunSimulationStep();
        }

        // Shuffle polygons and delete one at a time until none are colliding
        /*int triesPerStep = 5;
        bool continueShuffling = true;
        while (continueShuffling){
            for(int i = 0; i < polygonField.polygons.Count - 1; i++){
                Polygon thisPoly = polygonField.polygons[i];
                for (int j = i + 1; j < polygonField.polygons.Count; j++){
                    Polygon otherPoly = polygonField.polygons[j];

                    if (thisPoly.IsColliding(otherPoly)){
                        // Remove a random polygon, reshuffle all positions and rotations and check collisions again
                        polygonField.polygons.Remove(polygonField.polygons.RandomChoice());

                        foreach (Polygon poly in polygonField.polygons){
                            poly.position = new Vector2(Random.Range(-width/2, width/2), Random.Range(-length/2, length/2));
                            poly.rotation = Random.Range(0f, 360f);
                        }

                        i = 0;
                    }
                }
            }
            continueShuffling = false;
        }*/


        // Instantiate cargo prefabs to match final polygon configuration
        GameObject cargoParent = new GameObject("Cargo Parent");
        foreach (Polygon polygon in polygonField.polygons){
            GameObject prefab = initialCargo[polygon.id];

            Vector3 cargoPosition = new Vector3(polygon.position.x, 0, polygon.position.y);
            Quaternion cargoRotation = Quaternion.Euler(0, polygon.rotation, 0);

            GameObject thisCargo = Instantiate(prefab, cargoPosition, cargoRotation, cargoParent.transform);
        }

        return cargoParent;
    }

    private void InitializeCargo(){
        float roomArea = length * width;

        int currID = 0;
        float totalCargoArea = 0;
        while (totalCargoArea < roomArea){// && currID < 16){

            Vector2 position = new Vector2(Random.Range(-width/2, width/2), Random.Range(-length/2, length/2));
            float rotation = Random.Range(0f, 360f);

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
