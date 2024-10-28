using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BallData
{
    public int Number { get; set; }

    public BallData(int number = 2)
    {
        this.Number = number;
    }
}
