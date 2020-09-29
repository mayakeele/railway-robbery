using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 Rotate(this Vector2 v, float degrees) {
         float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
         float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
         float tx = v.x;
         float ty = v.y;
         v.x = (cos * tx) - (sin * ty);
         v.y = (sin * tx) + (cos * ty);
         return v;
    }

    public static Vector2 MaxValues(this Vector2[] array){
        Vector2 maxValues = array[0];
        foreach (Vector2 vector in array){
            if (vector.x > maxValues.x){
                maxValues.x = vector.x;
            }
            if (vector.y > maxValues.y){
                maxValues.y = vector.y;
            }
        }

        return maxValues;
    }

    public static Vector2 MinValues(this Vector2[] array){
        Vector2 minValues = array[0];
        foreach (Vector2 vector in array){
            if (vector.x < minValues.x){
                minValues.x = vector.x;
            }
            if (vector.y < minValues.y){
                minValues.y = vector.y;
            }
        }

        return minValues;
    } 
}
