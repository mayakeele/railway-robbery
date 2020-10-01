using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 Sum(this Vector2[] array){
        Vector2 sum = Vector2.zero;
        foreach (Vector2 vector in array){
            sum += vector;
        }

        return sum;
    }

    public static Vector2 Sum(this List<Vector2> list){
        Vector2 sum = Vector2.zero;
        foreach (Vector2 vector in list){
            sum += vector;
        }
        
        return sum;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) {
         float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
         float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
         float tx = v.x;
         float ty = v.y;

         Vector2 rotatedVector = new Vector2();
         rotatedVector.x = (cos * tx) - (sin * ty);
         rotatedVector.y = (sin * tx) + (cos * ty);
         return rotatedVector;
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

    public static Vector2 Clamp(this Vector2 vector, Vector2 min, Vector2 max){
        float x = Mathf.Clamp(vector.x, min.x, max.x);
        float y = Mathf.Clamp(vector.y, min.y, max.y);

        return new Vector2(x, y);
    }
}
