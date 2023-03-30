using System;
using UnityEngine;
using Mirror;

public class NetworkLauncher: NetworkManager
{
    public override void OnStartServer()
    {
        Debug.Log("get started server");
    }

    public override void OnStopServer()
    {
        Debug.Log(("server stop"));
    }

    public override void OnClientConnect()
    {
        Debug.Log("Client connect");
    }

    public override void OnClientDisconnect()
    {
        Debug.Log("Client disconnect");
    }
}