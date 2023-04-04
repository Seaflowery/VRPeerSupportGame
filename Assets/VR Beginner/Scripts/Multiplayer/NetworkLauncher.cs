﻿using System;
using UnityEngine;
using Mirror;

public class NetworkLauncher: NetworkManager
{
    public GameObject syncObject;
    public AudioSource qwq;
    public override void OnStartServer()
    {
        Debug.Log("get started server");
        NetworkServer.Spawn(syncObject);
        Debug.Log("succeed!!!");
    }

    public override void OnStopServer()
    {
        Debug.Log(("server stop"));
    }

    public override void OnClientConnect()
    {
        qwq.Play();
        Debug.Log("Client connect");
    }

    public override void OnClientDisconnect()
    {
        Debug.Log("Client disconnect");
    }
}