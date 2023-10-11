// Saving part is from https://github.com/ah1053/UnityRecorder/blob/main/Recorder/Recorder.cs

using System;
using System.IO;
using System.Text;
using UnityEngine;
using TestGameFrame;

/// <summary>
/// Add this component to a GameObject to Record Mic Input 
/// </summary>
// [RequireComponent(typeof(AudioSource))]
public class AudioRecorder : MonoBehaviour
{
    // static AudioSource audioSource;
    private static AudioRecorder _instance;
    private static AudioRecorder Instance
    {
        get 
        {
            if(_instance) return _instance;
            GameObject g = new GameObject();
            g.AddComponent<AudioSource>();
            return g.AddComponent<AudioRecorder>();
        }
    }
    private static AudioClip _audioClip;    

    /// <summary>
    /// Max recording length (in seconds)
    /// </summary>
    private const int MAX_RECORD_LENGTH = 900;

    private static string _deviceName = null;

    /// <summary>
    /// The unscaled time we started recording
    /// </summary>
    private static float _startRecordTime = 0;

    /// <summary>
    /// Is it recording?
    /// </summary>
    public static bool IsRecording => _startRecordTime > 0;

    /* -------------------------------------------------------------------------- */

    void Awake()
    {
        // audioSource = GetComponent<AudioSource>();   
        DontDestroyOnLoad(gameObject);
        if(_instance)
            Destroy(gameObject);
        else
            _instance = this;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Start recording
    /// </summary>
    /// <param name="deviceName"></param>
    public static void StartRecording(string deviceName = null)
    {
        // Errors
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("[AudioRecorder] No Microphone Available!!");
            return;            
        }
        if (IsRecording)
        {
            Debug.LogError("[AudioRecorder] Is Already Recording!");
            return;            
        }
        
        // Device name
        if(deviceName == null)
            _deviceName = Microphone.devices[0];

        // Get Recording Freq
        Microphone.GetDeviceCaps(_deviceName, out int minFreq, out int maxFreq);
        maxFreq = Mathf.Max(maxFreq, 44100); // capped at 44100

        // Microphone.Start()
        // deviceName	The name of the device. (Pass null to use default mic)
        // loop      	Indicates whether the recording should continue recording if lengthSec is reached, and wrap around and record from the beginning of the AudioClip.
        // lengthSec	Is the length of the AudioClip produced by the recording.
        // frequency	The sample rate of the AudioClip produced by the recording.
        _audioClip = Microphone.Start(_deviceName, true, MAX_RECORD_LENGTH, maxFreq);
        _startRecordTime = Time.unscaledTime;

        print("[AudioRecorder] Start Recording");
    }

    /// <summary>
    /// End recording
    /// </summary>
    /// <returns></returns>
    public static void StopRecording()
    {
        if(Microphone.GetPosition(_deviceName) < 0 || !IsRecording) 
        { 
            Debug.LogError("[AudioRecorder] Error Stop Recording");
            return;
        }

        print("[AudioRecorder] Stop Recording");
        Microphone.End(_deviceName);      
        _startRecordTime = 0;  
    }

    
    /// <summary>
    /// Save the recorded clip
    /// Default path:  {Application.persistentDataPath}/AudioRecorder/
    /// </summary>
    /// <returns>Saved file path</returns>
    public static string SaveRecordedClip(string saveDirectory = "")
    {
        return SaveRecordedClip(_startRecordTime, saveDirectory);
    }

    /// <summary>
    /// Trim & Save the recorded clip
    /// Default path:  {Application.persistentDataPath}/AudioRecorder/
    /// </summary>
    /// <param name="saveDirectory">Saved file path</param>
    /// <param name="startTime">audio start time (of Time.unscaledTime)</param>
    /// <returns></returns>
    public static string SaveRecordedClip(float startTime, string saveDirectory = "")
    {
        // Make path
        if(string.IsNullOrEmpty(saveDirectory))
            saveDirectory = Path.Combine(Application.persistentDataPath, "AudioRecorder");
        if(!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }
        // do write
       
        string filePath = Path.Combine(saveDirectory, "AphasiaVRPico_AphasiaSayItVR_" + "1169_record" +".wav");  
        WriteWAVFile(_audioClip, filePath, startTime-_startRecordTime, Time.unscaledTime-_startRecordTime);
        _startRecordTime = 0;
        Debug.Log("[AudioRecorder] Recorded File Saved Successfully at: " + filePath);        
        return filePath;
    }


    /// <summary>
    /// Write AudioClip to a wav file
    /// --WAV file format from http://soundfile.sapp.org/doc/WaveFormat/
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="filePath"></param>
    /// <param name="startTime">time offset</param>
    /// <param name="endTime">time duration</param>
    public static void WriteWAVFile(AudioClip clip, string filePath, float startTime=0, float endTime=0)
    {        
        if(endTime <= 0)
            endTime = clip.length;

        // Create the file.
        using (Stream fs = File.Create(filePath))
        {
            int frequency = clip.frequency;
            int numOfChannels = clip.channels;
            int samples = Mathf.FloorToInt((endTime - startTime) * frequency); //clip.samples;
            float[] data = new float[samples];
            Debug.Log($"[AudioRecorder] Saving Clip: length {samples}/{clip.samples} (Time: from {startTime} to {endTime})"); 

            // Header
            fs.Seek(0, SeekOrigin.Begin);
            // Chunk ID
            byte[] riff = Encoding.ASCII.GetBytes("RIFF");
            fs.Write(riff, 0, 4);
            // ChunkSize
            byte[] chunkSize = BitConverter.GetBytes((44 + samples) - 8); // 44: Header size
            fs.Write(chunkSize, 0, 4);
            // Format
            byte[] wave = Encoding.ASCII.GetBytes("WAVE");
            fs.Write(wave, 0, 4);
            // Subchunk1ID
            byte[] fmt = Encoding.ASCII.GetBytes("fmt ");
            fs.Write(fmt, 0, 4);
            // Subchunk1Size
            byte[] subChunk1 = BitConverter.GetBytes(16);
            fs.Write(subChunk1, 0, 4);
            // AudioFormat
            byte[] audioFormat = BitConverter.GetBytes(1);
            fs.Write(audioFormat, 0, 2);
            // NumChannels
            byte[] numChannels = BitConverter.GetBytes(numOfChannels);
            fs.Write(numChannels, 0, 2);
            // SampleRate
            byte[] sampleRate = BitConverter.GetBytes(frequency);
            fs.Write(sampleRate, 0, 4);
            // ByteRate
            byte[] byteRate = BitConverter.GetBytes(frequency * numOfChannels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
            fs.Write(byteRate, 0, 4);
            // BlockAlign
            ushort blockAlign = (ushort)(numOfChannels * 2);
            fs.Write(BitConverter.GetBytes(blockAlign), 0, 2);
            // BitsPerSample
            ushort bps = 16;
            byte[] bitsPerSample = BitConverter.GetBytes(bps);
            fs.Write(bitsPerSample, 0, 2);
            // Subchunk2ID
            byte[] datastring = Encoding.ASCII.GetBytes("data");
            fs.Write(datastring, 0, 4);
            // Subchunk2Size
            byte[] subChunk2 = BitConverter.GetBytes(samples * numOfChannels * 2);
            fs.Write(subChunk2, 0, 4);

            // Data            
            // The samples are floats ranging from -1.0f to 1.0f, representing the data in the audio clip
            clip.GetData(data, Mathf.FloorToInt(startTime * frequency)); 
            
            // 
            // short[] intData = new short[clipData.Length];
            byte[] bytesData = new byte[samples*2];
            for (int i = 0; i < samples; i++)
            {
                short sh = (short)(data[i] * 32767); // convertionFactor
                byte[] byteArr = new byte[2];
                byteArr = BitConverter.GetBytes(sh);
                byteArr.CopyTo(bytesData, i*2);
            }

            fs.Write(bytesData, 0, bytesData.Length);
        }
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Get current microphone volume in relative scale dB
    /// </summary>
    public static float GetCurrentVolume()
    {
        if(Microphone.GetPosition(_deviceName) < 0) 
        { 
            throw new Exception("[AudioRecorder] Error Fetching Volume");
        }

        int samples = 128;
        float[] data = new float[samples];
        int micPosition = Microphone.GetPosition(_deviceName)-(samples+1);
        _audioClip.GetData(data, micPosition);
        
        // Getting (Sum of Squares) on the last 128 samples
        float sos = 0;
        for (int i = 0; i < samples; i++) {
            sos += data[i] * data[i];
        }
        float rms = Mathf.Sqrt(sos/samples);
        return 20 * Mathf.Log10(rms / 0.01f);  // 0.01f: reference value
    }

}
