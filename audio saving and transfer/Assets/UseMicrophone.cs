using UnityEngine;

using UnityEngine.UI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


[RequireComponent(typeof(AudioSource))]

// the skeleton see https://jingyan.baidu.com/article/f7ff0bfccaebe62e26bb1397.html

public class UseMicrophone : MonoBehaviour
{
    [DllImport("D:/Files/Learning Materials/Y3/Semester-2/CPT202/VR/audio saving and transfer/Library/Project1.dll", EntryPoint = "denoise")]
    static extern unsafe int denoise(char[] in_file, char[] out_file);

    const int HEADER_SIZE = 44;

    private AudioSource aud;

    public Text txt;

    public Button Startbtn;

    public Button Endbtn;

    public Button Playbtn;

    private bool isHaveMicrophone = false;

    private string[] devices;

    // Use this for initialization

    void Start()
    {

        aud = this.GetComponent<AudioSource>();

        Startbtn.onClick.AddListener(OnClickStartBtn);

        Endbtn.onClick.AddListener(OnClickEndBtn);

        Playbtn.onClick.AddListener(OnClickPlayBtn);

        //获取麦克风设备，判断蛇摆是否有麦克风

        devices = Microphone.devices;

        if (devices.Length > 0)

        {

            isHaveMicrophone = true;

            txt.text = "Microphone Checked:" + devices[0];

        }

        else
        {

            isHaveMicrophone = false;

            txt.text = "No Microphone";

        }

    }

    private void OnClickStartBtn()
    {

        if ((isHaveMicrophone == false) || (Microphone.IsRecording(devices[0])))
        {

            return;

        }

        //开始录音

        /*

         * public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency);

         * deviceName The name of the device.

         * loop Indicates whether the recording should continue recording if lengthSec is reached,

           and wrap around and record from the beginning of the AudioClip.

         * lengthSec Is the length of the AudioClip produced by the recording.

         * frequency The sample rate of the AudioClip produced by the recording.  

         */

        aud.clip = Microphone.Start(devices[0], true, 10, 44100);

    }


    private unsafe void OnClickEndBtn()

    {

        if ((isHaveMicrophone == false) || (!Microphone.IsRecording(devices[0])))

        {

            return;

        }

        //结束录音

        Microphone.End(devices[0]);

        Save("audio\\audio.wav", aud.clip); //将录音保存为mp3


        string in_file = "audio\\audio.wav";
        char[] in_files = in_file.ToCharArray();

        string out_files = "audio\\a.wav";
        char[] out_file = out_files.ToCharArray();

        denoise(in_files, out_file);

    }    

    private void OnClickPlayBtn()

    {

        if ((isHaveMicrophone == false) || (Microphone.IsRecording(devices[0])

            || aud.clip == null))

        {

            return;

        }

        //WWW www = new WWW("D:\\Files\\Learning Materials\\Y3\\Semester-2\\CPT202\\VR\\audio\\audio.wav");
        //var clipTemp = www.GetAudioClip();
        //aud.clip = clipTemp;

        //播放录音

        aud.Play();

    }

    // save see https://blog.csdn.net/ldy597321444/article/details/52857999

    public static bool Save(string filename, AudioClip clip)
    {

        Debug.Log(Application.persistentDataPath);
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }

#if UNITY_IPHONE
//		path_1 = Application.persistentDataPath;
		string filepath = Path.Combine(Application.persistentDataPath, filename);
#endif

#if UNITY_STANDALONE_WIN
		string filepath =  filename;
#endif
#if UNITY_ANDROID
 
		string filepath = Path.Combine(Application.persistentDataPath, filename);
#endif

        //		string filepath = Path.Combine(Application.persistentDataPath, filename);
        Debug.Log(filepath);

        // Make sure directory exists if user is saving to sub dir.
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));

        using (FileStream fileStream = CreateEmpty(filepath))
        {

            ConvertAndWrite(fileStream, clip);

            WriteHeader(fileStream, clip);
        }

        return true; // TODO: return false if there's a failure saving the file
    }
    static FileStream CreateEmpty(string filepath)
    {
        FileStream fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }
    static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {

        float[] samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];
        //converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

        Byte[] bytesData = new Byte[samples.Length * 2];
        //bytesData array is twice the size of
        //dataSource array because a float converted in Int16 is 2 bytes.

        int rescaleFactor = 32767; //to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }
    static void WriteHeader(FileStream fileStream, AudioClip clip)
    {

        int hz = clip.frequency;
        int channels = clip.channels;
        int samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);

        //		fileStream.Close();
    }

}

