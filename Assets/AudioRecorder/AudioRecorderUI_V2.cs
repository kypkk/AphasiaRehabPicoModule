using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using DG.Tweening;
//using Unity.XR.PXR;

public class AudioRecorderUI_V2 : MonoBehaviour
{
    public static AudioRecorderUI_V2
        Instance
    { get; private set; }
    [SerializeField] Text _recordingNotif;
    //[SerializeField] Text _debugMsg;
    [SerializeField] CanvasGroup _canvasGroup;

    // bool isLongPress = false;
    public bool EnableVoiceControl = false;
    private bool record = false;
    public string Question;


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
        // AudioRecorder.StartRecording();
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
                _recordingNotif.text = "������... (A ����)";
                _recordingNotif.color = Color.red;
                record = true;
            }
            else
            {
                GameEventCenter.DispatchEvent("StopRecord");
                _recordingNotif.text = "���� (A��Ұ�)";
                _recordingNotif.color = Color.black;
                record = false;
            }
        }
    }
}