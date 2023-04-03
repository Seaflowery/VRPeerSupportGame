using System;
using Mirror;
using UnityEngine;

public class NetworkSpawn: NetworkBehaviour
{
    public GameObject syncObject;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("connected to server!");
        NetworkServer.Spawn(syncObject);
    }
}