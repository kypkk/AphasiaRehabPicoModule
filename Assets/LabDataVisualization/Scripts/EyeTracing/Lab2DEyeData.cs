using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataSync;

public class Lab2DEyeData : LabDataBase
{
    public Vector2 position;

    public Lab2DEyeData(float x, float y)
    {
        position = new Vector2(x, y);
    }
}
