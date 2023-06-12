﻿using System.Collections;
using UnityEngine;

public class StartGame: MonoBehaviour
{
    public GameObject startButton;
    public GameObject startPanel;
    public AudioSource audioSource;

    public void Start()
    {
        StartCoroutine(PlayAudio());
    }

    IEnumerator PlayAudio()
    {
        yield return new WaitForSeconds(5);
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        startButton.SetActive(true);
        startPanel.SetActive(true);
    }
}