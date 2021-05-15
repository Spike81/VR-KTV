using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneClass : MonoBehaviour
{
    string[] micDevicesName;
    AudioSource aud;
    string microphoneName;
    // Start is called before the first frame update
    void Start()
    {
        aud = GetComponent<AudioSource>();
        micDevicesName = Microphone.devices;
        Debug.Log(micDevicesName[0]);
        microphoneName = micDevicesName[0];
        aud.clip = Microphone.Start(microphoneName, true, 100, 48000);       

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            aud.timeSamples = Microphone.GetPosition(microphoneName) -500;
            aud.Play();
        }
   
    }
}
