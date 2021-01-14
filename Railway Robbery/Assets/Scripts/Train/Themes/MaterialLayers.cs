using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialLayers
{
    public enum MaterialLayer{
        Wall_PaintedWood_Exterior,
        Wall_PaintedMetal_Exterior,
        Roof_Wood,
        Roof_Metal,
        Floor_Wood,
        Floor_Carpet,
        Floor_Metal,
        Glass,
        Metal_Exterior,
        Metal_Interior,
        Upholstery_Primary,
        Upholstery_Detail,
        Furniture_Wood,
        Furniture_Fabric,
        Door_Wood,
        Door_Metal,
        Wallpaper,
        Ceiling_Wood,
        Ceiling_Metal,
        Wall_Wood_Interior,
        Wall_Metal_Interior,
        Caboose_Wall_Wood_Exterior
    }

    public static int GetLayerCount(){
        int numLayers = System.Enum.GetNames(typeof(MaterialLayer)).Length;
        return numLayers;
    }
}
