using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool Contains(this LayerMask layerMask, int layerNum){
        if(layerMask == (layerMask | (1 << layerNum))){
            return true;
        }
        else{
            return false;
        }
    }

    public static bool Contains(this LayerMask layerMask, string layerName){
        int layerNum = LayerMask.NameToLayer(layerName);
        if(layerMask == (layerMask | (1 << layerNum))){
            return true;
        }
        else{
            return false;
        }
    }
}
