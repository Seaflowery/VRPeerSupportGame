﻿using System.Collections;
using UnityEngine;

public class StartGame: MonoBehaviour
{
    public GameObject startButton;
    public GameObject startPanel;
    public AudioSource audioSource;

    public void OnCollision()
    {
        StartCoroutine(PlayAudio());
    }

    IEnumerator PlayAudio()
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        startButton.SetActive(true);
        startPanel.SetActive(true);
    } 
}