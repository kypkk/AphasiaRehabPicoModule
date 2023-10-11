using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using LabData;
using System.IO;


namespace aphasiaFrame
{
    public class TestMicro : MonoBehaviour
    {

        private bool micConnected = false;
        private int minFreq, maxFreq;
        public AudioClip RecordedClip;
        public AudioSource audioSource;
        //public Text Toast;
        private string fileName;
        private byte[] data;
        // Start is called before the first frame update

        
        void Start()
        {

            GameEventCenter.AddEvent("StartRecord", StartRecord);
            GameEventCenter.AddEvent("StopRecord", Stop);
            if (Microphone.devices.Length <= 0)
            {
                //GameEventCenter.DispatchEvent<string>("ShowMessage", "[Intialize] No MicroPhone device");
                //EyeTrackGazeRay.Mess = "[Intialize] No MicroPhone device";

                Debug.Log("[Intialize] No MicroPhone device");
            }
            else
            {
                //EyeTrackGazeRay.Mess = "[Intialize] Microphone Name" + Microphone.devices[0].ToString();

                micConnected = true;

                Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
                if (minFreq == 0 && maxFreq == 0)
                {
                    maxFreq = 44100;
                }

            }
        }
        /// <summary>
        /// Start Reconding
        /// </summary>
        public void StartRecord()
        {
            if (micConnected)
            {

                if (!Microphone.IsRecording(null))
                {
                    RecordedClip = Microphone.Start(null, false, 60, maxFreq);
                    //EyeTrackGazeRay.Mess = "[Start]Start Recording";
                }
                else
                {
                    //EyeTrackGazeRay.Mess = "[Start]Recording , don't repeat press button!!";
                }
            }
            else
            {

                //EyeTrackGazeRay.Mess = "[Start] Please check your microphone device!!";
            }
        }

        /// <summary>
        /// Stop record
        /// </summary>
        public void Stop()
        {
            data = GetRealAudio(ref RecordedClip);
            Microphone.End(null);
            Debug.Log("[Recording Stop]"+data.Length);
            //EyeTrackGazeRay.Mess = "[Stop] Stop";
            if(data.Length >=30000)
                Save();
            //AudioRecorder.Mess = "[Stop] Recording ENd";
        }

        //
        public void Playing()
        {
            if (!Microphone.IsRecording(null))
            {
                audioSource.clip = RecordedClip;
                audioSource.Play();
                //GameEventCenter.DispatchEvent<string>("ShowMessage", "[Playing]  Playing your recording!!");
               // EyeTrackGazeRay.Mess = "[Playing]  Playing your recording!!";
            }/*
            else
            {

                //GameEventCenter.DispatchEvent<string>("ShowMessage", "[Playing] Recording !!");
                EyeTrackGazeRay.Mess = "[Playing] Recording !!";
            }

            */
        }

        private string CreateFolder(string floderName, bool isNew) {
            if (Directory.Exists(floderName))
            {
                if (isNew)
                {
                    var tempPath = floderName + "_" + DateTime.Now.ToString();
                    Directory.CreateDirectory(tempPath);
                    return tempPath;

                }

                //GameEventCenter.DispatchEvent<string>("ShowMessage", "Folder Has Existed!");
                //EyeTrackGazeRay.Mess = "Folder Has Existed!";
                return floderName;
            }
            else
            {
                //Directory.CreateDirectory(floderName);
                //Debug.Log("Success Create: " + floderName);
                return floderName;
            }
        }

        public void Save()
        {
            Microphone.End(null);
            if (!Microphone.IsRecording(null))
            {
                fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                if (!fileName.ToLower().EndsWith(".wav"))
                {
                    fileName += ".wav";
                }
                string folder = Path.Combine(LabTools.DataPath, "Output/" + GameDataManager.FlowData.UserId + "_record");
                //GameEventCenter.DispatchEvent<string>("ShowMessage", "[Micro Save Path]" + folder);
                //EyeTrackGazeRay.Mess = "[Micro Save Path]" + folder;
                //Debug.Log("[Micro Save Path]" +folder);
                LabTools.CreatSaveDataFolder(folder);
                //CreateFolder(folder, false);
#if UNITY_ANDROID
                string path = Path.Combine(folder,fileName);
#else
                string path = Path.Combine(folder,fileName);
#endif
                Debug.Log(path);
                using (FileStream fs = CreateEmpty(path))
                {
                    fs.Write(data, 0, data.Length);
                    WriteHeader(fs, RecordedClip);
                }
            }
            else
            {
                //GameEventCenter.DispatchEvent<string>("ShowMessage", "Recording, Please stop your recording!");
                //EyeTrackGazeRay.Mess = "Recording, Please stop your recording!";

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// 
        public static byte[] GetRealAudio(ref AudioClip recordClip)
        {
            int position = Microphone.GetPosition(null);
            if (position <= 0 || position > recordClip.samples)
            {
                position = recordClip.samples;
               //EyeTrackGazeRay.Mess = "[Get Real Audio] Audio Position";
            }

            float[] soundata = new float[position * recordClip.channels];
            recordClip.GetData(soundata, 0);
            recordClip = AudioClip.Create(recordClip.name, position, recordClip.channels, recordClip.frequency, false);
            recordClip.SetData(soundata, 0);

            int rescaleFactor = 32767;
            byte[] outData = new byte[soundata.Length * 2];

            for (int i = 0; i < soundata.Length; i++)
            {
                short temshort = (short)(soundata[i] * rescaleFactor);
                byte[] tempData = BitConverter.GetBytes(temshort);
                outData[i * 2] = tempData[0];
                outData[i * 2 + 1] = tempData[1];
            }
            string tmp = "[Get Audio Recrod] Pos = " + position.ToString() + " outData, leng=" + outData.Length.ToString();
            //EyeTrackGazeRay.Mess = tmp;
            return outData;
        }

        public static void WriteHeader(FileStream stream, AudioClip clip)
        {
            int hz = clip.frequency;
            int channels = clip.channels;
            int samples = clip.samples;


            stream.Seek(0, SeekOrigin.Begin);
            Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
            stream.Write(riff, 0, 4);

            Byte[] chunkSize = BitConverter.GetBytes(stream.Length - 8);
            stream.Write(chunkSize, 0, 4);

            Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
            stream.Write(wave, 0, 4);

            Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
            stream.Write(fmt, 0, 4);

            Byte[] subChunk1 = BitConverter.GetBytes(16);
            stream.Write(subChunk1, 0, 4);

            UInt16 one = 1;

            Byte[] audioFormat = BitConverter.GetBytes(one);
            stream.Write(audioFormat, 0, 2);

            Byte[] numChannels = BitConverter.GetBytes(channels);
            stream.Write(numChannels, 0, 2);

            Byte[] sampleRate = BitConverter.GetBytes(hz);
            stream.Write(sampleRate, 0, 4);


            Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2);
            stream.Write(byteRate, 0, 4);

            UInt16 blockAlign = (ushort)(channels * 2);
            stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

            UInt16 bps = 16;
            Byte[] bitsPerSample = BitConverter.GetBytes(bps);
            stream.Write(bitsPerSample, 0, 2);

            Byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
            stream.Write(dataString, 0, 4);


            Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
            stream.Write(subChunk2, 0, 4);

        }

        private FileStream CreateEmpty(string filepath)
        {
            FileStream fileStream = new FileStream(filepath, FileMode.Create);
            byte emptyByte = new byte();
            for (int i = 0; i < 44; i++)
                fileStream.WriteByte(emptyByte);
            return fileStream;
        }

    }
}