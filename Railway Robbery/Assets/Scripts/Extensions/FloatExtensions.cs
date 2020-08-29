using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static float RoundToMultiple(this float input, float multiple, bool allowZero = true){
        float halfwayCutoff = multiple / 2;

        float remainderDown = input % multiple;
        float remainderUp = multiple - remainderDown;

        if(remainderDown < halfwayCutoff){
            if(input - remainderDown == 0 && allowZero == false){
                input += remainderUp;
            }
            else{
                input -= remainderDown;
            }        
        }
        else{
            input += remainderUp;
        }

        return input;
    }
    public static Vector3 RoundToMultiple(this Vector3 inputs, Vector3 multiples){
        Vector3 halfwayCutoffs = multiples / 2;

        Vector3 remainderDown = new Vector3(inputs.x % multiples.x, inputs.y % multiples.y, inputs.z % multiples.z);
        Vector3 remainderUp = multiples - remainderDown;

        if(remainderDown.x < halfwayCutoffs.x){
            inputs.x -= remainderDown.x;
        }
        else{
            inputs.x = inputs.x + remainderUp.x;
        }

        if(remainderDown.y < halfwayCutoffs.y){
            inputs.y -= remainderDown.y;
        }
        else{
            inputs.y = inputs.y + remainderUp.y;
        }

        if(remainderDown.z < halfwayCutoffs.z){
            inputs.z -= remainderDown.z;
        }
        else{
            inputs.z = inputs.z + remainderUp.z;
        }

        return inputs;
    }

}
