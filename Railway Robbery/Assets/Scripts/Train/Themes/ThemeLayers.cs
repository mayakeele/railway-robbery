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
        Glass_Primary,
        Metal_Primary_Exterior,
        Metal_Primary_Interior,
    }

    public static int GetLayerCount(){
        int numLayers = System.Enum.GetNames(typeof(ThemeLayer)).Length;
        return numLayers;
    }
}
