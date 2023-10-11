/// Modded by JCxYIS
/// 20220430

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabData;
using DataSync;
#if USE_SRANIPAL
using ViveSR.anipal.Eye;
#endif

public class HeadPosData : LabDataBase
{

    public Vector4 c1 { get; set; }
    public Vector4 c2 { get; set; }
    public Vector4 c3 { get; set; }
    public Vector4 c4 { get; set; }

    public HeadPosData(
        Vector4 c1,
        Vector4 c2,
        Vector4 c3,
        Vector4 c4)
    {
        this.c1 = c1;
        this.c2 = c2;
        this.c3 = c3;
        this.c4 = c4;
    }
}
