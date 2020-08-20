using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableGeometry : MonoBehaviour
{

    public Mesh mesh;
    public PhysicMaterial physicMaterial;

    private Collider coll;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        coll.tag = "Climbable";
    }

    void Update()
    {
        
    }
}
