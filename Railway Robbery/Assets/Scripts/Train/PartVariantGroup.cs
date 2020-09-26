﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartVariantGroup : MonoBehaviour
{
    public GameObject[] variants;
    public int[] weights;

    public GameObject ChooseVariant(){
        GameObject obj = variants.WeightedRandomChoice(weights);
        return obj;
    }
}
