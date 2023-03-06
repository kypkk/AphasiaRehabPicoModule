using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class audioProblemController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource startSource;
    public AudioClip start;
    public AudioClip[] problem;
    private AudioClip problemClip;
    public Text hint; 

    void Start()
    {
        playStart();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(playProblem());
        }
    }

    public void playStart()
    {

        startSource.clip = start;
        startSource.Play();
        StartCoroutine(playProblem());
    }
    public IEnumerator playProblem()
    {
        int index = Random.Range(0, problem.Length);
        problemClip = problem[index];
        
        audioSource.clip = problemClip;
        yield return new WaitForSeconds(22.0f);
        hint.text = problemClip.ToString();
        for(int i = 0; i < 3; i++)
        {
            audioSource.Play();
            yield return new WaitForSeconds(4.0f);
        }
        


        
        
        Debug.Log(audioSource.time);
    }
    public void correct(){
        hint.text = "恭喜答對了";
    }
    public void wrong(){
        hint.text = "答錯了喔";
    }
}