using System;
using UnityEngine;
using System.Collections;
using Mirror;

public class DancingManager: NetworkBehaviour
{
    private const int CountdownNum = 3;
    private const int MagicPointNum = 2;
    public AudioSource description;
    public AudioSource bgm;
    public AudioSource pass;
    public AudioSource fail;
    public AudioSource ding;
    public AudioSource[] countdown = new AudioSource[CountdownNum];
    public GameObject[] magicPoints = new GameObject[MagicPointNum];
    public GameObject fireplaceAnchor;
    public GameObject startButton;
    private Timer _countdownTimer = new Timer(1);
    private Timer _magicTimer = new Timer(4);
    private int totalNum;
    private int hitNum;
    int pointNum = 0;
    
    public void Start()
    {
        hitNum = 0;
    }


    public void OnLeaveDancingAnchor()
    {
        StopAllCoroutines();
    }
    
    public void OnEnterDancingAnchor()
    {
        if (!isServer)
            CmdStartGame();
    }
    
    [Command(requiresAuthority = false)]
    void CmdStartGame()
    {
        RpcStartGame();
        startButton.SetActive(false);
        Debug.Log("Start dancing!!!!");
        StartCoroutine(DescriptionAudioPlay());
    }
    
    [ClientRpc]
    void RpcStartGame()
    {
        startButton.SetActive(false);
    }

    public void OnHit()
    {
        CmdHit();
    }
    
    [Command(requiresAuthority = false)]
    void CmdHit()
    {
        Debug.Log("hit!!!");
        ++hitNum;
        RpcHit();
    }
    
    [ClientRpc]
    void RpcHit()
    {
        ding.Play();
        magicPoints[pointNum].SetActive(false);
    }

    IEnumerator DescriptionAudioPlay()
    {
        description.Play();
        RpcPlayDescription();
        yield return new WaitForSeconds(description.clip.length);
        StartCoroutine(StartGame());
    }
    
    [ClientRpc]
    void RpcPlayDescription() 
    {
        description.Play();
    }

    IEnumerator StartGame()
    {
        int cnt = 0;
        while (cnt < CountdownNum)
        {
            _countdownTimer.Tick(TimeManager.DeltaTime);
            if (_countdownTimer.TimeOut)
            {
                RpcPlayCountdown(cnt);
                // countdown[cnt].Play();
                cnt++;
                _countdownTimer.Reset();
            }
    
            yield return null;
        }
        
        bgm.Play();
        RpcPlayBgm();
        StartCoroutine(RenderGame());
    }
    
    [ClientRpc]
    void RpcPlayBgm()
    {
        bgm.Play();
    }
    
    [ClientRpc]
    void RpcPlayCountdown(int cnt)
    {
        countdown[cnt].Play();
    }

    [ClientRpc]
    void RpcSetMagicPoint(int pointNum, bool active)
    {
        magicPoints[pointNum].SetActive(active);
    }
    

    IEnumerator RenderGame()
    {
        magicPoints[pointNum].SetActive(true);
        RpcSetMagicPoint(pointNum, true);
        totalNum = 1;
        while (bgm.isPlaying)
        {
            _magicTimer.Tick(TimeManager.DeltaTime);
            if (_magicTimer.TimeOut)
            {
                // magicPoints[pointNum].SetActive(false);
                RpcSetMagicPoint(pointNum, false);
                pointNum = (pointNum + 1) % MagicPointNum;
                magicPoints[pointNum].SetActive(true);
                RpcSetMagicPoint(pointNum, true);
                totalNum++;
                _magicTimer.Reset();
            }
            
            yield return null;
        }

        for (int i = 0; i < MagicPointNum; i++)
        {
            magicPoints[i].SetActive(false);
            RpcSetMagicPoint(i, false);
        }

        if (Pass())
        {
            // pass.Play();
            RpcPlayPass();
            fireplaceAnchor.SetActive(true);
        }
        else
        {
            // fail.Play();
            RpcPlayFail();
            yield return new WaitForSeconds(fail.clip.length);
            hitNum = 0;
            startButton.SetActive(true);
            StopAllCoroutines();
        }
        
    }
    
    
    
    [ClientRpc]
    void RpcPlayPass()
    {
        pass.Play();
        fireplaceAnchor.SetActive(true);
    }
    
    [ClientRpc]
    void RpcPlayFail()
    {
        fail.Play();
        startButton.SetActive(true);
    }

    private bool Pass()
    {
        float hitRate = (hitNum * 1F) / totalNum;
        return hitRate > 0.8;
    }
}