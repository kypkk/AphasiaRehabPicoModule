using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;


public class MainSceneVideoScript : MonoBehaviour
{
    // All audio UIs
    public Text hint;
    public VideoClip problemClip;
    public VideoPlayer problemPlayer;
    public VideoClip[] easy_problem;
    public VideoClip[] medium_problem;
    public VideoClip[] hard_problem;
    static int difficulty;

    // sound UIs
    public AudioSource yourturn;
    public AudioSource Hint;
    public AudioClip yourturnClip;
    public AudioClip CorrectClip;
    public AudioClip WrongClip;
    public int lastIndex = -1;
    public int index = -1;

    // all of the UIs
    public GameObject EntryUI;
    public GameObject GameUI;
    public GameObject RuleUI;
    public GameObject ResultUI;

    // check and cross buttons
    public Button check_button;
    public Button cross_button;

    // score
    int score = 0;
    int times = 0;
    string resultString;
    public Text resultText;

    // Seleted
    private string selected = "left";
    public Image target; // UI�~��
    private RectTransform target_pos; // UI�~�ت��ؼЦ�m
    public float transitionSpeed = 5f; // �ഫ���t��
    RectTransform check_btn;
    RectTransform cross_btn;

    float timer = 0;
    // float TimeUsed => TimeUsed==0?timer: timer - TimeUsed;
    float TimeUsed => timer;

    GameStageData RoundData;

    void Start()
    {

        // Android >11 需要跳轉外部管理權限 (檢查 AndroidManifest 有沒有 <uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" /> )
        if(new AndroidJavaClass("android.os.Environment").CallStatic<bool>("isExternalStorageManager") == false) 
        {
            AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject reqPermsIntent =new AndroidJavaObject("android.content.Intent", 
                new AndroidJavaClass("android.provider.Settings").GetStatic<string>("ACTION_MANAGE_APP_ALL_FILES_ACCESS_PERMISSION"),
                new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", "package:"+Application.identifier));
            currentActivity.Call("startActivity", reqPermsIntent);
        }   

        LabVisualization.VisualizationManager.Instance.VisulizationInit();
        LabVisualization.VisualizationManager.Instance.StartDataVisualization();

        AudioRecorderUI_V2.Instance.EnableVoiceControl = GameDataManager.FlowData.AutoRecordVoice;

        yourturn.clip = yourturnClip;
        Time.timeScale = 0f;

        target_pos = target.GetComponent<RectTransform>();
        check_btn = check_button.GetComponent<RectTransform>();
        cross_btn = cross_button.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;


        setChoice();


        if (PicoInput.GetButtonDown(PicoInput.PicoButton.TriggerR))
        {
            selectChoice();
        }
    }

    public void gameStart(){

        StartCoroutine(playAudio());
    }
    
    public void setChoice()
    {
        if (PicoInput.GetJoystickValue(1).x > 0)
        {
            Debug.Log("right");
            selected = "right";
            target_pos.position = Vector3.Lerp(cross_btn.position, target_pos.position, transitionSpeed * Time.deltaTime);
        }
        else if (PicoInput.GetJoystickValue(1).x < 0)
        {
            Debug.Log("left");
            selected = "left";
            target_pos.position = Vector3.Lerp(check_btn.position, target_pos.position, transitionSpeed * Time.deltaTime);
        }
    }

    public void selectChoice()
    {
        switch (selected)
        {
            case "right":
                OnWrongClick();
                break;
            case "left":
                OnCorrectClick();
                break;
        }
    }

    public IEnumerator playAudio(){
        target.gameObject.SetActive(false); // ���m�~��UI
        timer = 0;
        RuleUI.SetActive(false);
        GameUI.SetActive(true);
        check_button.gameObject.SetActive(false);
        //check_button.interactable = false;
        cross_button.gameObject.SetActive(false);
        //cross_button.interactable = false;
        
        Time.timeScale = 1f;

        yield return new WaitForSeconds(3f);

        switch (GameDataManager.FlowData.level)
        {
            case GameLevel.easy:
                while (index == lastIndex)
                {
                    index = Random.Range(0, easy_problem.Length);
                }
                lastIndex = index;
                problemClip = easy_problem[index];
                break;
            case GameLevel.normal:
                while (index == lastIndex)
                {
                    index = Random.Range(0, medium_problem.Length);
                }
                lastIndex = index;
                problemClip = medium_problem[index];
                break;
            case GameLevel.hard:
                while (index == lastIndex)
                {
                    index = Random.Range(0, hard_problem.Length);
                }
                lastIndex = index;
                problemClip = hard_problem[index];
                break;

        }

        string hintText = problemClip.ToString().Replace("(UnityEngine.VideoClip)", "");
        hint.text = hintText;
        problemPlayer.clip = problemClip;
        yield return new WaitForSeconds((float) problemPlayer.clip.length);
        yourturn.Play();
        target.gameObject.SetActive(true); // �~��UI
        check_button.gameObject.SetActive(true);
        cross_button.gameObject.SetActive(true);
    }

    public void OnCorrectClick(){
        GameStageData newRound = new GameStageData();
        newRound.IsCorrect = true;
        newRound.TimeUsed = TimeUsed - (float) problemClip.length;
        newRound.Question = problemClip.name;
        try
        {
            LabDataTestComponent.LabDataManager.SendData(newRound);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Lab data not init? " + e);
        }

        score += 1;
        times += 1;
        Hint.clip = CorrectClip;
        Hint.Play();


        if(times >= 10){
            resultString = score.ToString() + " / 10";
            GameUI.SetActive(false);
            ResultUI.SetActive(true);
            resultText.text = resultString;
            Time.timeScale = 0f;
        }else{
            StartCoroutine(playAudio());
        }
        
        
    }

    public void OnWrongClick(){
        GameStageData newRound = new GameStageData();
        newRound.IsCorrect = false;
        newRound.TimeUsed = TimeUsed - (float)problemClip.length;
        newRound.Question = problemClip.name;

        try
        {
            LabDataTestComponent.LabDataManager.SendData(newRound);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Lab data not init? " + e);
        }
        times += 1;
        Hint.clip = WrongClip;
        Hint.Play();
        if(times >= 10){
            resultString = score.ToString() + " / 10";
            GameUI.SetActive(false);
            ResultUI.SetActive(true);
            resultText.text = resultString;
            Time.timeScale = 0f;
        }else{
            StartCoroutine(playAudio());
        }

    }

}