using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMaterials : MonoBehaviour
{
    public GameObject layerPaletteContainer;

    public Material[] layerMaterials = new Material[MaterialLayers.GetLayerCount()];
    public Color[] layerColors = new Color[MaterialLayers.GetLayerCount()];
    

    void Start()
    {
        layerPaletteContainer = GameObject.FindGameObjectWithTag("MaterialLayerPaletteContainer");

        // For each child of the palette container, get its Material layer, match it to an index of the MaterialLayer enum,
        // and add that to this train car's colors.

        MaterialLayerPalette[] layerPalettes = layerPaletteContainer.GetComponentsInChildren<MaterialLayerPalette>();
        foreach (MaterialLayerPalette thisPalette in layerPalettes){
            
            Material chosenMaterial = RandomExtensions.RandomChoice(thisPalette.materialPalette);
            Color chosenColor = RandomExtensions.RandomChoice(thisPalette.colorPalette); 

            int layer = (int) thisPalette.materialLayer;

            layerMaterials[layer] = new Material(chosenMaterial);
            layerColors[layer] = chosenColor; 
        }

        ApplyMaterialToChildren();
    }

    
    public void ApplyMaterialToChildren(){
        // Finds each part of this car with a MaterialdObject attached, gets its layer, and applies that layer's color to it

        HasMaterialLayer[] objectsWithMaterial = GetComponentsInChildren<HasMaterialLayer>();

        foreach (HasMaterialLayer thisObject in objectsWithMaterial){

            int layerIndex = (int) thisObject.materialLayer;

            Material layerMaterial = layerMaterials[layerIndex];
            Color layerColor = layerColors[layerIndex];

            Renderer meshRenderer = thisObject.gameObject.GetComponent<Renderer>();
            meshRenderer.material = layerMaterial;
            meshRenderer.material.SetColor("_Color", layerColor);
        }
    }
}
