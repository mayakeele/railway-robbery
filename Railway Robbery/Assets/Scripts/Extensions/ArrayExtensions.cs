using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static int Sum(this int[] array){
        int sum = 0;
        for (int i = 0; i < array.Length; i++){
            sum += array[i];
        }
        return sum;
    }

    public static float Sum(this float[] array){
        float sum = 0;
        for (int i = 0; i < array.Length; i++){
            sum += array[i];
        }
        return sum;
    }

    public static int Max(this int[] array){
        int max = array[0];
        foreach (int num in array){
            if (num > max){
                max = num;
            }
        }
        return max;
    }

    public static int Min(this int[] array){
        int min = array[0];
        foreach (int num in array){
            if (num < min){
                min = num;
            }
        }
        return min;
    }

    public static List<int> FindIndices<T>(this T[] array, T target){
        List<int> indices = new List<int>();

        for(int i = 0; i < array.Length; i++){
            if (array[i].Equals(target)){
                indices.Add(i);
            }
        }
        
        return indices;
    }

    public static List<int> FindIndices<T>(this List<T> list, T target){
        List<int> indices = new List<int>();

        for(int i = 0; i < list.Count; i++){
            if (list[i].Equals(target)){
                indices.Add(i);
            }
        }
        
        return indices;
    }

    public static void SetAllValues<T>(this List<T> list, T value){
        for(int i = 0; i < list.Count; i++){
            list[i] = value;
        }
    }
}
