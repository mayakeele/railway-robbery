using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableGeometry : MonoBehaviour
{

    public Mesh mesh;
    public PhysicMaterial physicMaterial;


    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Update()
    {
        
    }
}
