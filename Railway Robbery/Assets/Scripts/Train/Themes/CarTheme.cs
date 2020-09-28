﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTheme : MonoBehaviour
{
    public GameObject themeLayerPaletteContainer;

    public Color[] themeLayerColors = new Color[ThemeLayers.GetLayerCount()];

    void Start()
    {
        themeLayerPaletteContainer = GameObject.FindGameObjectWithTag("ThemeLayerPaletteContainer");

        // For each child of the palette container, get its theme layer, match it to an index of the ThemeLayer enum,
        // and add that to this train car's colors.

        ThemeLayerPalette[] layerPalettes = themeLayerPaletteContainer.GetComponentsInChildren<ThemeLayerPalette>();
        foreach (ThemeLayerPalette thisPalette in layerPalettes){
            
            Color chosenColor = RandomExtensions.RandomChoice(thisPalette.colorPalette); 
            int layerIndex = (int) thisPalette.themeLayer;

            themeLayerColors[layerIndex] = chosenColor;
        }

        ApplyThemeToChildren();
    }

    
    public void ApplyThemeToChildren(){
        // Finds each part of this car with a ThemedObject attached, gets its layer, and applies that layer's color to it

        ThemedObject[] themedObjects = GetComponentsInChildren<ThemedObject>();

        foreach (ThemedObject thisObject in themedObjects){

            int layerIndex = (int) thisObject.themeLayer;
            Color layerColor = themeLayerColors[layerIndex];

            Renderer meshRenderer = thisObject.gameObject.GetComponent<Renderer>();
            meshRenderer.material.SetColor("_Color", layerColor);
        }
    }
}
