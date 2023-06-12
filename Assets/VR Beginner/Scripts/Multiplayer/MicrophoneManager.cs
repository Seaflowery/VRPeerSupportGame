using UnityEngine;
using Mirror;

public class MicrophoneManager : NetworkBehaviour
{
    const int FREQUENCY = 44100;
    AudioClip mic;
    int lastPos, pos;
    
    void Start()
    {
        mic = Microphone.Start(null, true, 1, FREQUENCY);
        AudioSource audio = GetComponent<AudioSource>();
        audio.loop = true;
        // audio.clip = mic;
        while (!(Microphone.GetPosition(null) > 0)) { }
        audio.Play();
    }

    
    void Update()
    {
        pos = Microphone.GetPosition(null);
        if (pos > lastPos)
        {
            if (!isServer)
                Log.Instance.CmdLog("innnnnnnnnnnnnnn!");
            float[] samples = new float[(pos - lastPos) * mic.channels];
            mic.GetData(samples, lastPos);
            AudioSource audioSource = GetComponent<AudioSource>();
            
            // Debug.Log(audio.clip);
            audioSource.clip = AudioClip.Create("MicrophoneClip", samples.Length, mic.channels, FREQUENCY, false);
            audioSource.clip.SetData(samples, 0);
    
            // audio.clip.SetData(samples, lastPos);
            if (!isServer)
            {
                Log.Instance.CmdLog("qwq!");
                CmdSendAudio(samples);
            }
        }
        lastPos = pos;
    }
    
    [Command(requiresAuthority = false)]
    void CmdSendAudio(float[] samples)
    {
        Debug.Log("sending audio");
        RpcReceiveAudio(samples);
    }
    
    [ClientRpc(includeOwner = false)]
    void RpcReceiveAudio(float[] samples)
    {
        AudioSource audio = GetComponent<AudioSource>();
        // audio.clip.SetData(samples, 0);
        // audio.Play();
        Log.Instance.CmdLog("audio received");
    }
    
}