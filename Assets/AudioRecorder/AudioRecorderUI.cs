using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
// using DG.Tweening;
//using Unity.XR.PXR;

public class AudioRecorderUI : MonoBehaviour
{
    public static AudioRecorderUI Instance {get; private set;}
    [SerializeField] Text _recordingNotif;
    //[SerializeField] Text _debugMsg;
    [SerializeField] CanvasGroup _canvasGroup;

    // bool isLongPress = false;
    public bool EnableVoiceControl = false;
    private bool record = false;

    
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        record = false;
     }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

        if (PicoInput.GetButtonDown(PicoInput.PicoButton.A) || Input.GetKeyDown(KeyCode.A))
        {

            if (!record)
            {

                GameEventCenter.DispatchEvent("StartRecord");
                _recordingNotif.text = "錄音中... (A 取消)";
                _recordingNotif.color = Color.red;
                record = true;
            }
            else
            {
                GameEventCenter.DispatchEvent("StopRecord");
                _recordingNotif.text = "錄音 (A鍵啟動)";
                _recordingNotif.color = Color.black;
                record = false;
            }
        }
    }
}