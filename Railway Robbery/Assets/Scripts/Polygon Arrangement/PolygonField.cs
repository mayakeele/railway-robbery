using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonField
{
    public float fieldWidth;
    public float fieldLength;

    public float polygonRepulsion = 0.5f;
    public float wallRepulsion = 0.5f;

    public float stepSize = 0.08f;
    public float rotationConstant = 2;
    public int maxCycles = 20;

    public float sleepThreshold = 0.01f;
    public float percentSleepingToContinue = 0.2f;

    private float currCycle = 0;
    private int numSleeping = 0;

    public bool isSimulationFinished = false;
    

    public List<Polygon> polygons = new List<Polygon>();
    //public List<Polygon> exclusionZones = new List<Polygon>();
    

    public PolygonField(float width, float length) {
        this.fieldWidth = width;
        this.fieldLength = length;
    }


    public void RunSimulationStep(){
        // Runs a simulation of this polygon field until end conditions are met

        List<Vector2> forcePerPoly = new List<Vector2>();
        List<float> torquePerPoly = new List<float>();

        // Calculate forces and torques on each polygon in the simulation
        for (int p = 0; p < polygons.Count; p++){
            Polygon thisPoly = polygons[p];
            List<Vector2> forcePerPoint = CalculateForcePerPoint(thisPoly);

            Vector2 netForce = forcePerPoint.Sum();
            forcePerPoly.Add(netForce);

            float numPoints = forcePerPoint.Count;
            Vector2[] rotatedPoints = thisPoly.CalculateRotatedPoints();
            float netTorque = 0;
            for (int i = 0; i < numPoints; i++){
                Vector2 position = rotatedPoints[i];
                Vector2 force = forcePerPoint[i];
                float sinTheta = Mathf.Sin(Mathf.Deg2Rad * Vector2.SignedAngle(position, force));

                float thisTorque = position.magnitude * force.magnitude * sinTheta;
                netTorque += thisTorque;
            }

            torquePerPoly.Add(netTorque);
        }

        // Apply movement to all polygons and count how many fall below the sleep threshold
        numSleeping = 0;
        for (int p = 0; p < polygons.Count; p++){

            Vector2 displacement = forcePerPoly[p] * stepSize / (polygons[p].localPoints.Length * polygons[p].area);
            //float angularDisplacement = torquePerPoly[p] * stepSize * rotationConstant / polygons[p].localPoints.Length;

            polygons[p].position += displacement;
            //polygons[p].rotation += angularDisplacement;

            polygons[p].position = polygons[p].position.Clamp(new Vector2(-fieldWidth/2, -fieldLength/2), new Vector2(fieldWidth/2, fieldLength/2));

            if (displacement.magnitude <= sleepThreshold){
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

                Vector2 maxPoint = polygons[p].CalculateWorldPoints().MaxValues();
                Vector2 minPoint = polygons[p].CalculateWorldPoints().MinValues();

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
                isSimulationFinished = true;
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


    public List<Vector2> CalculateForcePerPoint(Polygon thisPoly){
        List<Vector2> forces = new List<Vector2>();

        foreach (Vector2 thisPoint in thisPoly.CalculateWorldPoints()){
            Vector2 netForce = Vector2.zero;

            // Calculate net force from all other polygon points
            foreach (Polygon otherPoly in polygons){
                if (otherPoly != thisPoly){
                    foreach (Vector2 otherPoint in otherPoly.CalculateWorldPoints()){
                        Vector2 displacement = thisPoint - otherPoint;
                        Vector2 forceDirection = displacement.normalized;
                        float distSquared = displacement.sqrMagnitude;
                        if (distSquared == 0){ distSquared = 0.0001f; }

                        netForce += forceDirection * (polygonRepulsion/distSquared) * otherPoly.area;
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

            forces.Add(netForce);
        }

        return forces;
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
