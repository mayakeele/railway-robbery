using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static int Sum(this int[] inputArray){
        int sum = 0;
        for (int i = 0; i < inputArray.Length; i++){
            sum += inputArray[i];
        }
        return sum;
    }

    public static float Sum(this float[] inputArray){
        float sum = 0;
        for (int i = 0; i < inputArray.Length; i++){
            sum += inputArray[i];
        }
        return sum;
    }
}
