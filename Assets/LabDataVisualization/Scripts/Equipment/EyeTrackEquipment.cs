/// 不要整份替代掉你的 code! 可能有加料過 (儲存特定資料之類的)
/// Modded by JCxYIS
/// 20220517

// #define USE_SRANIPAL
// #define USE_PICO_EYETRACK
// #define USE_PICOXR_EYETRACK

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LabVisualization;
using LabVisualization.EyeTracing;
using System;
using LabData;
using TestGameFrame; // 要加這行才能讀到Test_EnemyTask

#if USE_SRANIPAL
using ViveSR.anipal.Eye;
#elif USE_PICO_EYETRACK
using Pvr_UnitySDKAPI;
#elif USE_PICOXR_EYETRACK
using Unity.XR.PXR;
#endif


public class EyeTrackEquipment : MonoSingleton<EyeTrackEquipment>, IEquipment, IEyeTracingPos
{
#if USE_SRANIPAL
    public SRanipal_Eye_Framework Sranipal { get; set; }
    SingleEyeData LeftData { get; set; }
    SingleEyeData RightData { get; set; }
    SingleEyeData Combined { get; set; }
    private readonly GazeIndex[] GazePriority = new GazeIndex[] { GazeIndex.COMBINE, GazeIndex.LEFT, GazeIndex.RIGHT };
    private FocusInfo FocusInfo;
#elif USE_PICO_EYETRACK
    EyeTrackingGazeRay picoGazeRay;
#endif

    private bool IsOpenEye = false;

    public Vector2 EyeData { get; set; }

    string FocusName { get; set; }

    public bool IsStopEyeData = false;

    private bool _isHasFocused = false;

    /*
     * var r1 = PXR_EyeTracking.GetCombineEyeGazePoint(out Vector3 combinedEyeGazePoint);
            var r2 = PXR_EyeTracking.GetLeftEyeGazeOpenness(out float leftEyeopenness);
            var r3 = PXR_EyeTracking.GetRightEyeGazeOpenness(out float rightEyeOpenness);
            var r4 = PXR_EyeTracking.GetLeftEyePositionGuide(out Vector3 leftEyePosition);
            var r5 = PXR_EyeTracking.GetRightEyePositionGuide(out Vector3 rightEyePosition);
            var r6 = PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 combinedEyeGazeVector);
    */
    public void EquipmentInit()
    {

        Debug.Log("[EyeTrackEquipment] EquipmentInit");
        IsStopEyeData = false;
#if USE_SRANIPAL
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING)
        {
             
            if (Sranipal == null)
            {
                Sranipal = gameObject.AddComponent<SRanipal_Eye_Framework>();
            }
            Sranipal = SRanipal_Eye_Framework.Instance;
            Sranipal.StartFramework();
            IsOpenEye = true;
        }
#elif USE_PICO_EYETRACK
        // FindObjectOfType<Pvr_UnitySDKEyeManager>().SetEyeTrackingMode();
#endif
    }

    public void EquipmentStart()
    {
        Debug.Log("[EyeTrackEquipment] EquipmentStart");
#if USE_PICOXR_EYETRACK
        Debug.Log("USE_PICOXR_EYETRACK In");
#endif
        StartCoroutine(UpdateData());
    }

    public void EquipmentStop()
    {
        IsStopEyeData = true;
        if (IsOpenEye)
        {
#if USE_SRANIPAL
            Sranipal.StopFramework();
#endif
            IsOpenEye = false;
        }
    }

    public Func<Vector2> GetEyeTracingPos()
    {
        return () => EyeData;
    }

    private IEnumerator UpdateData()
    {
        /*
        Vector3 combinedEyeGazePoint;
        float leftEyeopenness;
        float rightEyeOpenness;
        Vector3 leftEyePosition;
        Vector3 rightEyePosition;
        Vector3 combinedEyeGazeVector;
        */
        yield return new WaitForSeconds(0.5f); // wait for init
        while (true)
        {
            if (IsStopEyeData)
            {
                yield break;
            }

#if USE_SRANIPAL || USE_PICO_EYETRACK || USE_PICOXR_EYETRACK
            Ray GazeRay = new Ray();

#if USE_SRANIPAL
            if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
            {
                VerboseData data;
                if (SRanipal_Eye.GetVerboseData(out data) &&
                    data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY) &&
                    data.right.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_DIRECTION_VALIDITY)
                    )
                {
                    // 紀錄左眼、右眼、雙眼數據                    
                    GameDataManager.LabDataManager.SendData(new LabLeftEyeData(data.left));
                    GameDataManager.LabDataManager.SendData(new LabRightEyeData(data.right));
                    GameDataManager.LabDataManager.SendData(new LabCombinedEyeData(data.combined.eye_data));
                    
                    SRanipal_Eye.Focus(GazePriority[0], out GazeRay, out FocusInfo, float.MaxValue);
#elif USE_PICO_EYETRACK
            // 視線
            bool result = Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingGazeRay(ref picoGazeRay);
            if(result)
            {
                // 紀錄左眼、右眼、雙眼數據
                EyeTrackingData picoEyeData = new EyeTrackingData();
                Pvr_UnitySDKAPI.System.UPvr_getEyeTrackingData(ref picoEyeData);
                print(picoEyeData.ToJson());
                GameDataManager.LabDataManager.SendData(new LabLeftEyeData(picoEyeData));
                GameDataManager.LabDataManager.SendData(new LabRightEyeData(picoEyeData));
                GameDataManager.LabDataManager.SendData(new LabCombinedEyeData(picoEyeData));

                GazeRay = new Ray(picoGazeRay.Origin, picoGazeRay.Direction);
                if(true)
                {
#elif USE_PICOXR_EYETRACK
            // 請看 https://developer.pico-interactive.com/document/doc : 根本亂寫。             
            // PXR_EyeTracking.GetLeftEyePoseStatus(out uint status) &&
            // PXR_EyeTracking.GetRightEyePoseStatus(out uint status) &&
            // PXR_EyeTracking.GetCombinedEyePoseStatus(out uint status) &&
            // PXR_EyeTracking.GetLeftEyeGazePoint(out Vector3 leftEyeGazePoint) &&  
            // PXR_EyeTracking.GetRightEyeGazePoint(out Vector3 rightEyeGazePoint) &&  // 以上兩個 API 消失了QQ
            var r1 = PXR_EyeTracking.GetCombineEyeGazePoint(out Vector3 combinedEyeGazePoint);
            var r2 = PXR_EyeTracking.GetLeftEyeGazeOpenness(out float leftEyeopenness);
            var r3 = PXR_EyeTracking.GetRightEyeGazeOpenness(out float rightEyeOpenness);
            var r4 = PXR_EyeTracking.GetLeftEyePositionGuide(out Vector3 leftEyePosition);
            var r5 = PXR_EyeTracking.GetRightEyePositionGuide(out Vector3 rightEyePosition);
            var r6 = PXR_EyeTracking.GetCombineEyeGazeVector(out Vector3 combinedEyeGazeVector);
            var r7 = PXR_EyeTracking.GetHeadPosMatrix(out Matrix4x4 headPosMatrix);
            var xrEyeResult = r1 && r2 && r3 && r4 && r5 && r6 && r7;
            Debug.Log($"[XREYE] Result={xrEyeResult} combinedEyeGazePoint: " + combinedEyeGazePoint + "combinedEyeGazeVector: " + combinedEyeGazeVector + "leftEyePosition: " + leftEyePosition + " | rightEyePosition: " + rightEyePosition + " | leftEyeopenness: " + leftEyeopenness + " | rightEyeOpenness: " + rightEyeOpenness);
            print($"headPosMatrix={headPosMatrix}");
            if (xrEyeResult)
            {
                // 紀錄左眼、右眼、雙眼數據
                GameDataManager.LabDataManager.SendData(new LabLeftEyeData(Vector3.zero, Vector3.zero, -1, leftEyeopenness, leftEyePosition));
                GameDataManager.LabDataManager.SendData(new LabRightEyeData(Vector3.zero, Vector3.zero, -1, leftEyeopenness, rightEyePosition));
                GameDataManager.LabDataManager.SendData(new LabCombinedEyeData(combinedEyeGazePoint, combinedEyeGazeVector, -1, -1, Vector3.zero));
                GameDataManager.LabDataManager.SendData(new HeadPosData(headPosMatrix.GetColumn(0), headPosMatrix.GetColumn(1), headPosMatrix.GetColumn(2), headPosMatrix.GetColumn(3)));

                // 因為 combined Eye Gaze Vector 的角度是相對的，所以要加上頭部旋轉角度
                GazeRay = new Ray(Camera.main.transform.position, Camera.main.transform.rotation * combinedEyeGazeVector);
                if (true)
                {
#endif
                    // 三維座標轉二維 並記錄二維座標數據
                    float x = GazeRay.direction.x / GazeRay.direction.z;
                    float y = GazeRay.direction.y / GazeRay.direction.z;
                    GameDataManager.LabDataManager.SendData(new Lab2DEyeData(x, y));

                    // 讓泡泡固定z=1
                    Vector3 ray = new Vector3(x, y, 1);
                    try
                    {
                        // bubble本身是世界座標系
                        // FindObjectOfType<Test_MainScneRes>().eyeBubble.transform.localPosition = ray;
                        // Debug.Log("bubble position: " + FindObjectOfType<Test_MainScneRes>().eyeBubble.transform.localPosition);
                    }
                    catch (Exception e)
                    {
                        // print("No Bubble");
                    }

                    //不一定有focus
                    try
                    {
                        FocusName = "";
                        string focusPoint = "";
                        string focusNormal = "";
                        // 注視點物體名稱
#if USE_SRANIPAL
                        FocusName = FocusInfo.collider.gameObject.name;
                        focusPoint = FocusInfo.point.ToString();
                        focusNormal = FocusInfo.normal.ToString();
#elif USE_PICO_EYETRACK || USE_PICOXR_EYETRACK
                        RaycastHit hit;
                        Physics.Raycast(GazeRay, out hit, 20);
                        FocusName = hit.transform.name;
                        focusPoint = hit.point.ToString();
                        focusNormal = hit.normal.ToString();
                        Debug.Log("FocusName: " + FocusName);
#endif

                        // Debug.Log("注視 : " + FocusName + "\n" 
                        //         + "3D注視點  : " + focusPoint + "\n" 
                        //         + "注視法向量 : " + focusNormal);
                        GameDataManager.LabDataManager.SendData(new EyeFocusData(FocusName, focusPoint, focusNormal));

                        // 眼動計時器
                        GameEventCenter.DispatchEvent("EyeFocusTimer", FocusName);

                        // 眼動判斷 -> 加入視覺提示或是語音提示

                        GameEventCenter.DispatchEvent("ShowEyeFocus", FocusName);
                        GameEventCenter.DispatchEvent("GetEyeContact", FocusName);
                        GameEventCenter.DispatchEvent("GetFocusName", FocusName);

                    }
                    catch (Exception e)
                    {
                        // Debug.Log("No Focus");
                    }

                    #region 改之前
                    /*
                    EyeData = data.left.pupil_position_in_sensor_area;
                    LeftData = data.left;
                    RightData = data.right;
                    Combined = data.combined.eye_data;
                    //Debug.Log("Left:" + data.left.pupil_position_in_sensor_area + "~O~O~" + "Right:" + data.right.pupil_position_in_sensor_area);
                    Ray GazeRay = new Ray();
                    if (SRanipal_Eye.Focus(GazePriority[0], out GazeRay, out FocusInfo, float.MaxValue))
                    {
                        FocusName = FocusInfo.collider.gameObject.name;                   
                        //GameEventCenter.DispatchEvent("ShowEyeFocus", FocusName);
                        //GameEventCenter.DispatchEvent("GetEyeContact", FocusName);
                        //GameEventCenter.DispatchEvent("GetFocusName", FocusName);

                        var eyepositiondata = new EyePositonData() //記錄eyedata
                        {
                            X = FocusInfo.point.x,
                            Y = FocusInfo.point.y,
                            Z = FocusInfo.point.z,
                            FocusObject = FocusName,
                            Pupil_Diameter_Left = LeftData.pupil_diameter_mm,
                            Eye_Openness_Left = LeftData.eye_openness,
                            Pupil_Diameter_Right = RightData.pupil_diameter_mm,
                            Eye_Openness_Right = RightData.eye_openness,
                            Pupil_Diameter_Combined = Combined.pupil_diameter_mm,
                            Eye_Openness_Combined = Combined.eye_openness
                        };
                        GameDataManager.LabDataManager.SendData(eyepositiondata);
                        //Debug.Log("FocusInfo:" + FocusName + " At (" + FocusInfo.point.x + "," + FocusInfo.point.y + "," + FocusInfo.point.z + ")");
                        //Debug.Log("PupilSize :" + data.left.pupil_diameter_mm);                       
                    }
                    */
                    #endregion
                }
            }



            #region 存資料不能寫這RRR

            #endregion

            yield return new WaitForFixedUpdate();
#endif
        }
    }

    public string GetCurrentFocus()
    {
        return FocusName;
    }
}
