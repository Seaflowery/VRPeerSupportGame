using System;
using UnityEngine;
using System.Collections;

public class DancingManager: MonoBehaviour
{
    private const int CountdownNum = 3;
    private const int MagicPointNum = 2;
    public AudioSource description;
    public AudioSource bgm;
    public AudioSource pass;
    public AudioSource fail;
    public AudioSource[] countdown = new AudioSource[CountdownNum];
    public GameObject[] magicPoints = new GameObject[MagicPointNum];
    public GameObject fireplaceAnchor;
    public GameObject startButton;
    private Timer _countdownTimer = new Timer(1);
    private Timer _magicTimer = new Timer(4);
    private int totalNum;
    private int hitNum;
    
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
        startButton.SetActive(false);
        StartCoroutine(DescriptionAudioPlay());
    }

    public void OnHit()
    {
        ++hitNum;
    }

    IEnumerator DescriptionAudioPlay()
    {
        description.Play();
        yield return new WaitForSeconds(description.clip.length);
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        int cnt = 0;
        while (cnt < CountdownNum)
        {
            _countdownTimer.Tick(TimeManager.DeltaTime);
            if (_countdownTimer.TimeOut)
            {
                countdown[cnt].Play();
                cnt++;
                _countdownTimer.Reset();
            }
    
            yield return null;
        }
        
        bgm.Play();
        StartCoroutine(RenderGame());
    }


    IEnumerator RenderGame()
    {
        int pointNum = 0;
        magicPoints[pointNum].SetActive(true);
        totalNum = 1;
        while (bgm.isPlaying)
        {
            _magicTimer.Tick(TimeManager.DeltaTime);
            if (_magicTimer.TimeOut)
            {
                magicPoints[pointNum].SetActive(false);
                pointNum = (pointNum + 1) % MagicPointNum;
                magicPoints[pointNum].SetActive(true);
                totalNum++;
                _magicTimer.Reset();
            }
            
            yield return null;
        }

        foreach (GameObject magicPoint in magicPoints)
        {
            magicPoint.SetActive(false);
        }

        if (Pass())
        {
            pass.Play();
            fireplaceAnchor.SetActive(true);
        }
        else
        {
            fail.Play();
            yield return new WaitForSeconds(fail.clip.length);
            hitNum = 0;
            startButton.SetActive(true);
            StopAllCoroutines();
        }
        
    }

    private bool Pass()
    {
        float hitRate = (hitNum * 1F) / totalNum;
        return hitRate > 0.8;
    }
}