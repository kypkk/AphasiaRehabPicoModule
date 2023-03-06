using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainSceneVideoScript : MonoBehaviour
{
    public Text hint; 
    public VideoClip problemClip;
    
    void Start()
    {
        string hintText = problemClip.ToString().Replace("(UnityEngine.VideoClip)", "").Replace("1-","");
        hint.text = hintText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
