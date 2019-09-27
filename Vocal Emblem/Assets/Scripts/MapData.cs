﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public int[,] mapTest = {
        { 0,0,  0,0,  0,  0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 1,1,  1,1,  1,  0,1,1,1,1,1,1,1,1,1,1,1,1 },
        { 1,1,  1,1,  1,  1,1,1,1,1,1,1,1,20,20,20,20,1 },
        { 1,1,  1,1,  1,  0,1,1,1,1,1,1,1,20,20,20,20,1 },
        { 0,0,  0,0,  0,  0,1,1,1,1,1,1,1,20,20,20,20,1 },
        { 1,1,  1,1,  1,  1,1,1,1,1,1,1,1,20,20,20,20,1 },
        { 1,1,-20,1,-20,-20,1,1,1,1,1,1,1,20,20,20,20,1 },
        { 1,1,-20,1,  1,-20,1,1,1,1,1,1,1,20,20,20,20,1 },
        { 1,1,-20,1,  1,-20,1,1,1,1,1,1,1,20,20,20,20,1 },
        { 1,1,-20,1,  1,-20,1,1,1,1,1,1,1,1,1,1,1,1 }
    };

    public int[,] mapTestCharacterPos = {
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,2,0,2,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
        { 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }
    };
}
