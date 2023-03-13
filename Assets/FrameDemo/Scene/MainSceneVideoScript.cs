using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class MainSceneVideoScript : MonoBehaviour
{
    public Text hint;
    public VideoClip problemClip;
    public VideoPlayer problemPlayer;
    public VideoClip[] easy_problem;
    public VideoClip[] medium_problem;
    public VideoClip[] hard_problem;
    static int difficulty;
    static bool gameIsPaused = true;

    // all of the UIs
    public GameObject EntryUI;
    public GameObject GameUI;
    public GameObject RuleUI;

    
    void Start()
    {
        GameUI.SetActive(false);
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // if(difficulty == 1){
            
        // }else if(difficulty == 2){
            
        // }else{
            
        // }
        // string hintText = problemClip.ToString().Replace("(UnityEngine.VideoClip)", "");
        // hint.text = hintText;
        // problemPlayer.clip = problemClip;
    }

    public void easy(){
        difficulty = 1;
        EntryUI.SetActive(false);
        GameUI.SetActive(true);
        Time.timeScale = 1f;
        int index = Random.Range(0, easy_problem.Length);
        problemClip = easy_problem[index];
        string hintText = problemClip.ToString().Replace("(UnityEngine.VideoClip)", "");
        hint.text = hintText;
        problemPlayer.clip = problemClip;
    }
    public void medium(){
        difficulty = 2;
        EntryUI.SetActive(false);
        GameUI.SetActive(true);
        Time.timeScale = 1f;
        int index = Random.Range(0, medium_problem.Length);
        problemClip = medium_problem[index];
        string hintText = problemClip.ToString().Replace("(UnityEngine.VideoClip)", "");
        hint.text = hintText;
        problemPlayer.clip = problemClip;
    }
    public void hard(){
        difficulty = 3;
        EntryUI.SetActive(false);
        GameUI.SetActive(true);
        Time.timeScale = 1f;
        int index = Random.Range(0, hard_problem.Length);
        problemClip = hard_problem[index];
        string hintText = problemClip.ToString().Replace("(UnityEngine.VideoClip)", "");
        hint.text = hintText;
        problemPlayer.clip = problemClip;
    }
}
