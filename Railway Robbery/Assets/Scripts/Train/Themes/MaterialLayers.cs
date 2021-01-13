using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MaterialLayers
{
    public enum MaterialLayer{
        Wall_Wood_Exterior_Primary,
        Wall_Wood_Exterior_Detail,
        Roof_Primary,
        Roof_Detail,
        Floor_Wood,
        Floor_Carpet,
        Glass,
        Metal_Exterior,
        Metal_Interior,
        Upholstery_Primary,
        Upholstery_Detail,
        Furniture_Wood,
        Furniture_Fabric,
        Door_Wood,
        Door_Metal
    }

    public static int GetLayerCount(){
        int numLayers = System.Enum.GetNames(typeof(MaterialLayer)).Length;
        return numLayers;
    }
}
