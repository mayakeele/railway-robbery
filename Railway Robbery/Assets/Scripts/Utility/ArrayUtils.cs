using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ArrayUtils
{
    public static int[] ChooseRandomIndices(int numToChoose, int arrayLength, bool allowRepeats = false){
        numToChoose = Mathf.Clamp(numToChoose, 0, arrayLength-1);

        int numChosen = 0;
        int[] choices = new int[numToChoose];

        while (numChosen < numToChoose){
            int thisNum = UnityEngine.Random.Range(0, arrayLength);

            if (allowRepeats){
                choices[numChosen] = thisNum;
                numChosen++;
            }
            else{
                if(System.Array.IndexOf(choices, thisNum) == -1){
                    choices[numChosen] = thisNum;
                    numChosen++;
                }
            }
        }

        return choices;
    }
}
