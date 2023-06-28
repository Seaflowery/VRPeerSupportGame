using UnityEngine;
using Mirror;
using System.IO;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class MicrophoneManager : NetworkBehaviour
{

    private AudioClip audioClip;
    private const int FREQUENCY = 44100;
    private const int SECONDS = 3;
    private float interval = 3.0f;
    private float timer;
    private float[] audioArray;
    private float[] destArray;
    private float[] chunk;
    public bool isConnected = false;
    public XRController controller;
    bool triggerValue;

    [Command(requiresAuthority = false)]
    private void CmdSendVoiceData(float[] audioData, int seq)
    {
        Debug.Log("CmdSendVoiceData");
        RpcReceiveVoiceData(audioData, seq);
    }

    [ClientRpc]
    private void RpcReceiveVoiceData(float[] audioData, int seq)
    {
        Array.Copy(audioData, 0, destArray, seq * FREQUENCY, FREQUENCY);
        // Convert received audio data to audio clip
        if (seq == SECONDS - 1)
        {
            // Log.Instance.CmdLog("Get audio data");
            AudioClip receivedAudioClip = AudioClip.Create("audio", destArray.Length, 1, FREQUENCY, false);
            receivedAudioClip.SetData(destArray, 0);
            
            // Play the received audio clip
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = receivedAudioClip;
            audioSource.Play();
            
            // AudioSource.PlayClipAtPoint(receivedAudioClip, Vector3.zero); 
        }
    }

    private void Start()
    {
        timer = interval;
        audioArray = new float[FREQUENCY * SECONDS];
        destArray = new float[FREQUENCY * SECONDS];
        chunk = new float[FREQUENCY];
        // StartRecording();
        // Initialize the microphone
        if (Microphone.devices.Length > 0)
        {
            string deviceName = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No microphone found!");
        }
    }

    private void Update()
    {
        if (isConnected && !isServer)
        {
            bool previousTriggerValue = triggerValue;
            // previousTriggerValue = false;
            // triggerValue = true;
            controller.inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out triggerValue);
            if (!previousTriggerValue && triggerValue)
            {
                Log.Instance.CmdLog("trigger pressed");
                StartRecording();
            }

            if (triggerValue)
            {
                if (timer <= 0)
                {
                    Debug.Log(audioArray.Length);
                    SendVoiceData();
                    StopRecording();
                    StartRecording();
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }
            if (previousTriggerValue && !triggerValue)
            {
                SendVoiceData();
                StopRecording();
            }
        }
    }
    
    void SendVoiceData()
    {
        // Convert audio clip to float array
        audioClip.GetData(audioArray, 0);
        for (int i = 0; i < SECONDS; i++)
        {
            Array.Copy(audioArray, i * FREQUENCY, chunk, 0, FREQUENCY);
            CmdSendVoiceData(chunk, i); 
        }
    }

    void StartRecording()
    {
        audioClip = Microphone.Start(null, false, SECONDS, FREQUENCY);
    }
    
    void StopRecording()
    {
        timer = interval;
        Microphone.End(null);
    }

    
}
