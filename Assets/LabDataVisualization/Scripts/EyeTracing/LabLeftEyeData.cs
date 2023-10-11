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

public class LabLeftEyeData : LabDataBase
{
    /** 眼睛凝视点，基于右手坐标系*/
    public Pos gaze_origin_mm { get; set; }
    /** 眼睛归一化后的注视方向右手坐标系*/
    public Pos gaze_direction_normalized { get; set; }
    /** 瞳孔直径*/
    public float pupil_diameter_mm { get; set; }
    /** 眼睛开合程度.*/
    public float eye_openness { get; set; }
    /** The normalized position of a pupil in [0,1]*/
    public Vector2 pupil_position_in_sensor_area { get; set; }

#if USE_SRANIPAL
    public LabLeftEyeData(SingleEyeData data)
    {
        gaze_origin_mm = data.gaze_origin_mm.ToPos();
        gaze_direction_normalized = data.gaze_direction_normalized.ToPos();
        pupil_diameter_mm = data.pupil_diameter_mm;
        eye_openness = data.eye_openness;
        pupil_position_in_sensor_area = data.pupil_position_in_sensor_area;
    }
#elif USE_PICO_EYETRACK
    public LabLeftEyeData(Pvr_UnitySDKAPI.EyeTrackingData data)
    {
        // 不確定串的值是不是對的
        gaze_origin_mm = data.leftEyeGazePoint.ToPos();
        gaze_direction_normalized = data.leftEyeGazeVector.ToPos();
        pupil_diameter_mm = data.leftEyePupilDilation;
        eye_openness = data.leftEyeOpenness;
        pupil_position_in_sensor_area = data.leftEyePositionGuide; 
    }
#endif

    public LabLeftEyeData(
        Vector3 gaze_origin_mm, 
        Vector3 gaze_direction_normalized, 
        float pupil_diameter_mm, 
        float eye_openness, 
        Vector2 pupil_position_in_sensor_area)
    {
        this.gaze_origin_mm = gaze_origin_mm.ToPos();
        this.gaze_direction_normalized = gaze_direction_normalized.ToPos();
        this.pupil_diameter_mm = pupil_diameter_mm;
        this.eye_openness = eye_openness;
        this.pupil_position_in_sensor_area = pupil_position_in_sensor_area;
    }
}
