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


    
    void Start()
    {
        string hintText = problemClip.ToString().Replace("(UnityEngine.VideoClip)", "").Replace("1-","");
        hint.text = hintText;
        problemPlayer.clip = problemClip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
