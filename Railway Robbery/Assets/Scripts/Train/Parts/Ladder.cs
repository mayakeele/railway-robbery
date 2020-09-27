using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{

    // Set autoGenerate to true if you want to make a prefab with a self-generating ladder
    [SerializeField] private bool autoGenerate = false;

    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private float rungDistance;

    [SerializeField] private Vector3 initialPos;
    [SerializeField] private Vector3 initialAngles;

    public bool isMoveable = false;


    private TrainPartFactory trainPartFactory;

    void Awake() {
        trainPartFactory = GameObject.FindObjectOfType<TrainPartFactory>();

        if (autoGenerate == true){
            float height = Random.Range(minHeight, maxHeight);

            GameObject ladderObject = GenerateLadder(height, rungDistance);

            ladderObject.transform.parent = this.transform;
            ladderObject.transform.localPosition = initialPos;
            ladderObject.transform.eulerAngles = initialAngles;
        }
    }



    public GameObject GenerateLadder(float inputHeight, float inputRungDistance){
        // Instantiates ladder prefabs, scales them to required size and number of rungs
        GameObject parentObject = new GameObject("Ladder");
        Transform parentTransform = parentObject.transform;

        //Ladder ladderScript = parentObject.AddComponent<Ladder>(); ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //ladderScript.Initialize(inputHeight, inputRungDistance); ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        // Side bars
        GameObject bars = Instantiate(trainPartFactory.ladderBars.ChooseVariant(), parentTransform);

        MeshFilter[] barMeshes = bars.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter mf in barMeshes){
            Mesh m = mf.mesh;
            m.ScaleVerticesNonUniform(1, inputHeight, 1);

            CapsuleCollider coll = mf.gameObject.GetComponent<CapsuleCollider>();
            coll.height = inputHeight;
            coll.center = new Vector3(coll.center.x, inputHeight / 2, coll.center.z);
        }

        // Connectors
        GameObject connectors = Instantiate(trainPartFactory.ladderConnector.ChooseVariant(), parentTransform);
        connectors.transform.position = new Vector3(0, inputHeight, 0);

        // Rungs
        GameObject rungs = new GameObject("Ladder Rungs");
        rungs.transform.parent = parentTransform;

        GameObject rungVariant = trainPartFactory.ladderRung.ChooseVariant();

        float currRungHeight = inputHeight;
        while (currRungHeight > 0){
            GameObject thisRung = Instantiate(rungVariant, rungs.transform);

            thisRung.transform.position = new Vector3(0, currRungHeight, 0);

            currRungHeight -= inputRungDistance;
        }


        return parentObject;
    }  

}
