using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomExtensions
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

    public static T RandomChoice<T>(this T[] inputArray){
        // Chooses and returns a random value from a given array of any type
        int index = Random.Range(0, inputArray.Length);

        return inputArray[index];
    }

    public static bool RandomBool(){
        int rand = Random.Range(0, 2);
        if (rand == 0){
            return false;
        }
        else{
            return true;
        }
    }

    public static bool RandomChance(float chance){
        float rand = Random.Range(0f, 1f);

        if (rand < chance){
            return true;
        }
        else{
            return false;
        }   
    }
}
