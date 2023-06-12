using UnityEngine;
using Mirror;
using System.IO;
using System;

public class MicrophoneManager : NetworkBehaviour
{
    private AudioClip audioClip;

    [Command]
    private void CmdSendVoiceData(byte[] audioData)
    {
        RpcReceiveVoiceData(audioData);
    }

    [ClientRpc]
    private void RpcReceiveVoiceData(byte[] audioData)
    {
        // Convert received audio data to audio clip
        AudioClip receivedAudioClip = ConvertByteArrayToAudioClip(audioData);

        // Play the received audio clip
        AudioSource.PlayClipAtPoint(receivedAudioClip, Vector3.zero);
    }

    private void Start()
    {
        // Initialize the microphone
        if (Microphone.devices.Length > 0)
        {
            string deviceName = Microphone.devices[0];
            audioClip = Microphone.Start(deviceName, true, 10, AudioSettings.outputSampleRate);
        }
        else
        {
            Debug.LogError("No microphone found!");
        }
    }

    private void Update()
    {
        if (isLocalPlayer && Microphone.IsRecording(null))
        {
            // Get the microphone data and convert it to a byte array
            float[] samples = new float[audioClip.samples];
            audioClip.GetData(samples, 0);

            byte[] audioData = ConvertAudioClipToByteArray(samples);

            // Send the audio data to the server to be relayed to other clients
            CmdSendVoiceData(audioData);
        }
    }

    private byte[] ConvertAudioClipToByteArray(float[] samples)
    {
        int length = samples.Length * 4;
        byte[] byteArray = new byte[length];
        Buffer.BlockCopy(samples, 0, byteArray, 0, length);
        return byteArray;
    }

    private AudioClip ConvertByteArrayToAudioClip(byte[] byteArray)
    {
        int length = byteArray.Length / 4;
        float[] samples = new float[length];
        Buffer.BlockCopy(byteArray, 0, samples, 0, byteArray.Length);

        AudioClip audioClip = AudioClip.Create("ReceivedAudioClip", length, 1, AudioSettings.outputSampleRate, false);
        audioClip.SetData(samples, 0);
        return audioClip;
    }
}
