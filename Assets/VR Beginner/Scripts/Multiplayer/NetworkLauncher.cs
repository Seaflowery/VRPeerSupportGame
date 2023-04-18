using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkLauncher: NetworkManager
{
    // public GameObject syncObject;
    private string _IPAddress = "172.25.99.89";
    public List<GameObject> authorizeObjects;
    
    public void OnPressStartClient()
    {
        networkAddress = _IPAddress;
        singleton.StartClient();
    }
    public override void OnStartServer()
    {
        Debug.Log("get started server");
        NetworkServer.SpawnObjects();
        Debug.Log("succeed!!!");
        
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("add player");
        // Assign authority to client for all networked objects spawned on the server
        foreach (GameObject obj in authorizeObjects)
        {
            NetworkServer.Spawn(obj);
            AuthorityManager.Instance.OnStartAuthorize(conn, obj);
        }
    }


    public override void OnStopServer()
    {
        Debug.Log(("server stop"));
    }

    public override void OnClientConnect()
    {
        Debug.Log("Client connect");
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            NetworkClient.Ready();
            if (NetworkClient.localPlayer == null)
            {
                NetworkClient.AddPlayer();
            }
        }
    }
    
  

    public override void OnClientDisconnect()
    {
        Debug.Log("Client disconnect");
    }
}