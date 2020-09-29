using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonField : MonoBehaviour
{
    public float fieldWidth;
    public float fieldLength;

    public float polygonRepulsion;
    public float wallRepulsion;

    public float stepSize;
    public float maxCycles;

    public float sleepThreshold;
    public float percentSleepingToContinue;

    public float currCycle = 0;
    public int numSleeping = 0;
    

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
        RunSimulationFrame();
    }


    public void RunSimulationFrame(){
        // Runs a simulation of this polygon field until end conditions are met

        List<Vector2> forces = new List<Vector2>();

        // Calculate forces on each polygon in the simulation
        for (int p = 0; p < polygons.Count; p++){
            forces.Add(CalculateRepulsion(polygons[p]));
        }

        // Apply movement to all polygons and count how many fall below the sleep threshold
        numSleeping = 0;
        for (int p = 0; p < polygons.Count; p++){
            Vector2 displacement = forces[p] * stepSize;

            if (displacement.magnitude > sleepThreshold){
                polygons[p].position += displacement;
            }
            else{
                numSleeping++;
            }
        }

        // If the simulation has settled down (enough polygons are sleeping) or has reached a set number of steps, perform culling step
        if ((numSleeping / polygons.Count >= percentSleepingToContinue || currCycle > maxCycles) && currCycle > 0){
            currCycle = 0;

            // Calculate how many collisions each polygon has with other polygons or walls
            List<int> numCollisions = new List<int>();
            for (int p = 0; p < polygons.Count; p++){
                numCollisions.Add(CalculateNumCollisions(polygons[p]));

                Vector2 maxPoint = polygons[p].GetWorldPoints().MaxValues();
                Vector2 minPoint = polygons[p].GetWorldPoints().MinValues();

                if (maxPoint.x >= fieldWidth/2 || minPoint.x <= -fieldWidth/2){
                    numCollisions[p]++;
                }
                if (maxPoint.y >= fieldLength/2 || minPoint.y <= -fieldLength/2){
                    numCollisions[p]++;
                }
            }

            // Get the polygon with the maximum number of collisions
            int maxCollisions = numCollisions.ToArray().Max();
            if (maxCollisions == 0){
                // No collisions means the system is in equilibrium, and the simulation is done
                Debug.Log("Simulation finished.");
                return; 
            }
            else{
                // Out of the polygon(s) with the most collisions, choose 1 to remove
                List<int> choppingBlockIndices = numCollisions.FindIndices<int>(maxCollisions);
                int indexToDelete = RandomExtensions.RandomChoice<int>(choppingBlockIndices);

                polygons.RemoveAt(indexToDelete);
            }
        }

        currCycle++;
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
                        if (distSquared == 0){ distSquared = 0.0001f; }

                        netForce += forceDirection * (polygonRepulsion/distSquared);
                    }
                }
            }

            // Calculate net force from walls
            float halfWidth = fieldWidth / 2;
            float halfLength = fieldLength / 2;

            float wallForceRight = (wallRepulsion / Mathf.Pow(halfWidth - thisPoint.x, 2));
            float wallForceLeft = (wallRepulsion / Mathf.Pow(-halfWidth - thisPoint.x, 2));
            float wallForceFront = (wallRepulsion / Mathf.Pow(halfLength - thisPoint.y, 2));
            float wallForceBack = (wallRepulsion / Mathf.Pow(-halfLength - thisPoint.y, 2));

            netForce += new Vector2(wallForceLeft - wallForceRight, wallForceBack - wallForceFront);
        }

        return netForce;
    }

    public int CalculateNumCollisions(Polygon thisPoly){
        // Calculate how many other polygons this polygon is colliding with
        int num = 0;
        foreach (Polygon otherPoly in polygons){
            if (otherPoly != thisPoly){
                if (thisPoly.IsColliding(otherPoly)){
                    num++;
                }
            }
        }

        return num;
    }
}
