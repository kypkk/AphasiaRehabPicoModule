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

    // all of the UIs
    public GameObject EntryUI;
    public GameObject GameUI;
    public GameObject RuleUI;

    // score
    int score = 0;
    int times = 0;

    void Start()
    {
        yourturn.clip = yourturnClip;
        nextTask.clip = nextTaskClip;
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void easy(){
        difficulty = 1;
        EntryUI.SetActive(false);
        RuleUI.SetActive(true);
    }
    public void medium(){
        difficulty = 2;
        EntryUI.SetActive(false);
        RuleUI.SetActive(true);
    }
    public void hard(){
        difficulty = 3;
        EntryUI.SetActive(false);
        RuleUI.SetActive(true);
    }

    public void gameStart(){

        StartCoroutine(playAudio());

        
    }

    

    public IEnumerator playAudio(){
        RuleUI.SetActive(false);
        GameUI.SetActive(true);
        Time.timeScale = 1f;

        yield return new WaitForSeconds(3f);
        if(difficulty == 1){
            int index = Random.Range(0, easy_problem.Length);
            problemClip = easy_problem[index];
        }else if(difficulty == 2){
            int index = Random.Range(0, medium_problem.Length);
            problemClip = medium_problem[index];
        }else{
            int index = Random.Range(0, hard_problem.Length);
            problemClip = hard_problem[index];
        }

        string hintText = problemClip.ToString().Replace("(UnityEngine.VideoClip)", "");
        hint.text = hintText;
        problemPlayer.clip = problemClip;
        yield return new WaitForSeconds(15f);
        yourturn.Play();
        
            
        
        

    }

    public void OnCorrectClick(){
        score += 1;
        times += 1;
        Hint.clip = CorrectClip;
        Hint.Play();
        if(times >= 10){

        }else{
            StartCoroutine(playAudio());
        }
        
        
    }

    public void OnWrongClick(){
        times += 1;
        Hint.clip = WrongClip;
        Hint.Play();
        if(times >= 10){

        }else{
            StartCoroutine(playAudio());
        }

    }

}

// 輪到你說了
// 下一題
// 正確與否的按鈕
// score