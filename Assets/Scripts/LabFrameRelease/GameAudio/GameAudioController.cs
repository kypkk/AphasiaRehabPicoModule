using System.Collections;
using System.Collections.Generic;
using LabData;
using UnityEngine;

/// <summary>
/// [Singleton]  遊戲音樂、音效控制
/// </summary>
public class GameAudioController : MonoSingleton<GameAudioController> 
{
    /// <summary>
    /// 重複撥放一段旋律
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
    public GameObject LoopPlay(AudioClip clip, float volume = 1f)
    {
        GameObject go = new GameObject("LoopPlay");
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
        return go;
    }

    /// <summary>
    /// 播放 3D 音效 (SFX)
    /// </summary>
    /// <param name="clip">音</param>
    /// <param name="target">位置</param>
    /// <param name="volume">音量</param>
    /// <param name="maxDistance">能傳到最遠距離，負值代表使用全域音效</param>
    public void PlayOneShot(AudioClip clip, GameObject target, float volume = 0.5f, float maxDistance = 3f)
    {
        AudioSource audioSource = target.GetComponent<AudioSource>() ?? target.AddComponent<AudioSource>();
        if(maxDistance > 0)
        {
            audioSource.spatialBlend = 1;
            audioSource.spread = 360;
            audioSource.maxDistance = maxDistance;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
        audioSource.volume = volume;
        // audioSource.clip = clip;
        // StartCoroutine(playCoroutine(audioSource));
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// 播放全域音效 (SFX)
    /// </summary>
    /// <param name="clip">音</param>
    /// <param name="volume"></param>
    /// <param name="maxDistance"></param>
    public void PlayOneShot(AudioClip clip, float volume = 0.5f)
    {
        GameObject go = new GameObject("PlayOneShotTemp");
        PlayOneShot(clip, go, volume, -1);
        Destroy(go, clip.length + .1f);
    }
   
}
