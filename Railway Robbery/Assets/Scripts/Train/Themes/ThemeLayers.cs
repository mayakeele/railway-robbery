using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ThemeLayers
{
    public enum ThemeLayer{
        Wall_Primary,
        Wall_Detail,
        Roof_Primary,
        Roof_Detail,
        Floor_Primary,
        Floor_Detail,
        Glass_Primary,
        Metal_Primary_Exterior,
        Metal_Primary_Interior,
        Interior_Upholstery_Primary,
        Interior_Upholstery_Secondary,
        Interior_Wood
    }

    public static int GetLayerCount(){
        int numLayers = System.Enum.GetNames(typeof(ThemeLayer)).Length;
        return numLayers;
    }
}
